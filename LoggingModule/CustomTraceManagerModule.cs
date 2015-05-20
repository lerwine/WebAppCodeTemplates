using System;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Erwine.Leonard.T.LoggingModule.ExtensionMethods;

namespace Erwine.Leonard.T.LoggingModule
{
    public class CustomTraceManagerModule : IHttpModule
    {
        private object _syncRoot = new object();

        private HttpApplication _context = null;

        public TraceSource TraceSource { get; private set; }

        public CustomTraceManagerModule() { }

        ~CustomTraceManagerModule()
        {
            object syncRoot;

            System.Threading.Thread.BeginCriticalRegion();

            try
            {
                syncRoot = this._syncRoot;
                System.Threading.Thread.EndCriticalRegion();
            }
            catch
            {
                System.Threading.Thread.EndCriticalRegion();
                throw;
            }

            if (syncRoot != null)
                this.Dispose(false);
        }

        public static CustomTraceManagerModule GetCurrent()
        {
            return HttpContext.Current.ApplicationInstance.Modules.OfType<CustomTraceManagerModule>().FirstOrDefault();
        }
        
        #region IHttpModule Members

        public void Init(HttpApplication context)
        {
            if (this._syncRoot == null)
                throw new ObjectDisposedException(this.GetType().FullName);

            lock (this._syncRoot)
            {
                if (this._context != null)
                    this._DetachContext();

                this._context = context;

                string sourceName;

                try
                {
                    sourceName = context.GetType().Namespace;
                }
                catch
                {
                    sourceName = null;
                }

                this.TraceSource = new TraceSource((String.IsNullOrWhiteSpace(sourceName)) ? context.GetType().Name : sourceName);

                context.BeginRequest += this.Application_BeginRequest;
                context.AuthenticateRequest += this.Application_AuthenticateRequest;
                context.PostAuthenticateRequest += this.Application_PostAuthenticateRequest;
                context.AuthorizeRequest += this.Application_AuthorizeRequest;
                context.PostAuthorizeRequest += this.Application_PostAuthorizeRequest;
                context.AcquireRequestState += this.Application_AcquireRequestState;
                context.PostAcquireRequestState += this.Application_PostAcquireRequestState;
                context.PreRequestHandlerExecute += this.Application_PreRequestHandlerExecute;
                context.PostRequestHandlerExecute += this.Application_PostRequestHandlerExecute;
                context.EndRequest += this.Application_EndRequest;
                context.Error += this.Application_Error;
            }
        }

        private void _DetachContext()
        {
            if (this._context == null)
                return;

            this._context.BeginRequest -= this.Application_BeginRequest;
            this._context.AuthenticateRequest -= this.Application_AuthenticateRequest;
            this._context.PostAuthenticateRequest -= this.Application_PostAuthenticateRequest;
            this._context.AuthorizeRequest -= this.Application_AuthorizeRequest;
            this._context.PostAuthorizeRequest -= this.Application_PostAuthorizeRequest;
            this._context.AcquireRequestState -= this.Application_AcquireRequestState;
            this._context.PostAcquireRequestState -= this.Application_PostAcquireRequestState;
            this._context.PreRequestHandlerExecute -= this.Application_PreRequestHandlerExecute;
            this._context.PostRequestHandlerExecute -= this.Application_PostRequestHandlerExecute;
            this._context.EndRequest -= this.Application_EndRequest;
            this._context.Error -= this.Application_Error;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing)
                return;

            object syncRoot;

            System.Threading.Thread.BeginCriticalRegion();

            try
            {
                syncRoot = this._syncRoot;
                this._syncRoot = null;
                System.Threading.Thread.EndCriticalRegion();
            }
            catch
            {
                System.Threading.Thread.EndCriticalRegion();
                throw;
            }

            if (syncRoot == null)
                throw new ObjectDisposedException(this.GetType().FullName);

            lock (syncRoot)
            {
                if (this._context != null)
                    this._DetachContext();

                this._context = null;
            }
        }

        #endregion

        #region Application Event Handlers

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_BeginRequest);
        }

        private void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_AuthenticateRequest);
        }

        private void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostAuthenticateRequest);
        }

        private void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_AuthorizeRequest);
        }

        private void Application_PostAuthorizeRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostAuthorizeRequest);
        }

        private void Application_AcquireRequestState(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_AcquireRequestState);
        }

        private void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostAcquireRequestState);
        }

        private void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PreRequestHandlerExecute);
        }

        private void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostRequestHandlerExecute);
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_EndRequest);
        }

        private void Application_Error(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Application_Error);
        }

        #endregion

        #region Event tracing members

        #region Instance

        private void _TraceData(TraceEventType traceEventType, TraceEventId traceEventId, object data)
        {
            if (this.TraceSource == null)
                throw new ObjectDisposedException(this.GetType().FullName);

            lock (this._syncRoot)
                this.TraceSource.TraceData(traceEventType, (int)(traceEventId), TraceDataObject.EnsureSerializable(data));
        }

        private void _TraceData(TraceEventType traceEventType, TraceEventId traceEventId, params object[] data)
        {
            if (this.TraceSource == null)
                throw new ObjectDisposedException(this.GetType().FullName);

            lock (this._syncRoot)
                this.TraceSource.TraceData(traceEventType, (int)(traceEventId), (data == null) ? new object[0] : data.Select(d => TraceDataObject.EnsureSerializable(data)).ToArray());
        }

        private void _TraceEvent(TraceEventType traceEventType, TraceEventId traceEventId, string message)
        {
            if (this.TraceSource == null)
                throw new ObjectDisposedException(this.GetType().FullName);

            lock (this._syncRoot)
            {
                if (message == null)
                    this.TraceSource.TraceEvent(traceEventType, (int)(traceEventId));
                else
                    this.TraceSource.TraceEvent(traceEventType, (int)(traceEventId), message);
            }
        }

        private void _TraceTransfer(TraceEventId traceEventId, string message, Guid relatedActivityId)
        {
            if (this.TraceSource == null)
                throw new ObjectDisposedException(this.GetType().FullName);

            lock (this._syncRoot)
                this.TraceSource.TraceTransfer((int)(traceEventId), message, relatedActivityId);
        }

        #endregion

        #region Critical

        public static void TraceCritical(TraceEventId eventId)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Critical, eventId);
        }

        public static void TraceCritical(TraceEventId eventId, string message, Exception exception = null)
        {
            if (exception == null)
                CustomTraceManagerModule.TraceEvent(TraceEventType.Critical, eventId, message);
            else
                CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, message));
        }

        public static void TraceCritical(TraceEventId eventId, string message, Exception exception, object data0, params object[] nData)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, message, (nData == null) ? new object[] { data0 } : (new object[] { data0 }).Concat(nData).ToArray()));
        }

        public static void TraceCritical(TraceEventId eventId, Exception exception, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, data));
        }

        public static void TraceCritical(TraceEventId eventId, object data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, data);
        }

        public static void TraceCritical(TraceEventId eventId, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, data);
        }

        #endregion

        #region Error

        public static void TraceError(TraceEventId eventId)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Error, eventId);
        }

        public static void TraceError(TraceEventId eventId, string message, Exception exception = null)
        {
            if (exception == null)
                CustomTraceManagerModule.TraceEvent(TraceEventType.Error, eventId, message);
            else
                CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, message));
        }

        public static void TraceError(TraceEventId eventId, string message, Exception exception, object data0, params object[] nData)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, message, (nData == null) ? new object[] { data0 } : (new object[] { data0 }).Concat(nData).ToArray()));
        }

        public static void TraceError(TraceEventId eventId, Exception exception, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, data));
        }

        public static void TraceError(TraceEventId eventId, object data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, data);
        }

        public static void TraceError(TraceEventId eventId, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, data);
        }

        #endregion

        #region Warning

        public static void TraceWarning(TraceEventId eventId)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Warning, eventId);
        }

        public static void TraceWarning(TraceEventId eventId, string message, Exception exception = null)
        {
            if (exception == null)
                CustomTraceManagerModule.TraceEvent(TraceEventType.Warning, eventId, message);
            else
                CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, message));
        }

        public static void TraceWarning(TraceEventId eventId, string message, Exception exception, object data0, params object[] nData)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, message, (nData == null) ? new object[] { data0 } : (new object[] { data0 }).Concat(nData).ToArray()));
        }

        public static void TraceWarning(TraceEventId eventId, Exception exception, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, TraceDataObject.CreateExceptionEventData(eventId, exception, data));
        }

        public static void TraceWarning(TraceEventId eventId, object data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, data);
        }

        public static void TraceWarning(TraceEventId eventId, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, data);
        }

        #endregion

        #region Information

        public static void TraceInformation(TraceEventId eventId)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, eventId);
        }

        public static void TraceInformation(TraceEventId eventId, string message)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, eventId, message);
        }

        public static void TraceInformation(TraceEventId eventId, object data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Information, eventId, data);
        }

        public static void TraceInformation(TraceEventId eventId, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Information, eventId, data);
        }

        #endregion

        #region Verbose

        public static void TraceVerbose(TraceEventId eventId)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Verbose, eventId);
        }

        public static void TraceVerbose(TraceEventId eventId, string message)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Verbose, eventId, message);
        }

        public static void TraceVerbose(TraceEventId eventId, object data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Verbose, eventId, data);
        }

        public static void TraceVerbose(TraceEventId eventId, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Verbose, eventId, data);
        }

        #endregion

        public static void TraceData(TraceEventType traceEventType, TraceEventId traceEventId, object data)
        {
            CustomTraceManagerModule module = CustomTraceManagerModule.GetCurrent();
            if (module != null && module.TraceSource != null)
                module._TraceData(traceEventType, traceEventId, data);
            else
                CustomTraceManagerModule._TraceEventLL(traceEventType, traceEventId, data);
        }

        public static void TraceData(TraceEventType traceEventType, TraceEventId traceEventId, params object[] data)
        {
            CustomTraceManagerModule module = CustomTraceManagerModule.GetCurrent();
            if (module != null && module.TraceSource != null)
                module._TraceData(traceEventType, traceEventId, data);
            else
                CustomTraceManagerModule._TraceEventLL(traceEventType, traceEventId, data);
        }

        public static void TraceEvent(TraceEventType traceEventType, TraceEventId traceEventId)
        {
            CustomTraceManagerModule.TraceEvent(traceEventType, traceEventId, traceEventId.GetEnumDescription());
        }

        public static void TraceEvent(TraceEventType traceEventType, TraceEventId traceEventId, string message)
        {
            CustomTraceManagerModule module = CustomTraceManagerModule.GetCurrent();
            if (module != null && module.TraceSource != null)
                module._TraceEvent(traceEventType, traceEventId, message);
            else
                CustomTraceManagerModule._TraceEventLL(traceEventType, traceEventId, message);
        }

        private static void _TraceEventLL(TraceEventType traceEventType, TraceEventId traceEventId, string message)
        {
            switch (traceEventType)
            {
                case TraceEventType.Critical:
                    Trace.TraceError(String.Format("Critical - ID {0}: {1}", traceEventId, message));
                    break;
                case TraceEventType.Error:
                    Trace.TraceError(String.Format("ID {0}: {1}", traceEventId, message));
                    break;
                case TraceEventType.Warning:
                    Trace.TraceWarning(String.Format("ID {0}: {1}", traceEventId, message));
                    break;
                case TraceEventType.Information:
                    Trace.TraceInformation(String.Format("ID {0}: {1}", traceEventId, message));
                    break;
                default:
                    Trace.WriteLine(String.Format("ID {0}: {1}", traceEventId, message), traceEventType.ToString("F"));
                    break;
            }
        }

        private static void _TraceEventLL(TraceEventType traceEventType, TraceEventId traceEventId, params object[] data)
        {
            CustomTraceManagerModule._TraceEventLL(traceEventType, traceEventId, (new TraceDataObject(data)).ToString());
        }

        public static void TraceEvent(TraceEventType traceEventType, TraceEventId traceEventId, string message, string detail)
        {
            string m = message, d = null;
            if (String.IsNullOrWhiteSpace(m))
            {
                if (String.IsNullOrWhiteSpace(detail))
                {
                    CustomTraceManagerModule.TraceEvent(traceEventType, traceEventId);
                    return;
                }

                m = detail;
            }
            else
                d = detail;

            if (String.IsNullOrWhiteSpace(d))
                CustomTraceManagerModule.TraceEvent(traceEventType, traceEventId, m);
            else
                CustomTraceManagerModule.TraceData(traceEventType, traceEventId, new MessageAndDetail(m, d));
        }

        public static void TraceTransfer(TraceEventId traceEventId, Guid relatedActivityId)
        {
            CustomTraceManagerModule.TraceTransfer(traceEventId, null, relatedActivityId);
        }

        public static void TraceTransfer(TraceEventId traceEventId, string message, Guid relatedActivityId)
        {
            CustomTraceManagerModule module = CustomTraceManagerModule.GetCurrent();
            if (module != null && module.TraceSource != null)
                module._TraceTransfer(traceEventId, (String.IsNullOrWhiteSpace(message)) ? traceEventId.GetEnumDescription() : message, relatedActivityId);
            else
                CustomTraceManagerModule._TraceEventLL(TraceEventType.Transfer, traceEventId, String.Format("{0} ({1})", 
                    (String.IsNullOrWhiteSpace(message)) ? traceEventId.GetEnumDescription() : message, relatedActivityId));
        }

        #endregion
    }
}
