using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using Erwine.Leonard.T.LoggingModule.ExtensionMethods;
using Erwine.Leonard.T.ExtensionMethods.AttributeTypes;
using System.Configuration;

namespace Erwine.Leonard.T.LoggingModule
{
    public class CustomTraceManagerModule : IHttpModule
    {
        private static object _syncRoot = new object();

        private static TraceSource _appTraceSource = null;

        public CustomTraceManagerModule() { }

        ~CustomTraceManagerModule()
        {
            this.Dispose(false);
        }

        #region IHttpModule Members

        private HttpApplication _context = null;

        public void Init(HttpApplication context)
        {
            lock (CustomTraceManagerModule._syncRoot)
            {
                if (this._context != null)
                    this._UnloadContext();

                this._context = context;

                if (CustomTraceManagerModule._appTraceSource == null)
                {
                    string sourceName;

                    try
                    {
                        sourceName = context.GetType().Namespace;
                    }
                    catch
                    {
                        sourceName = null;
                    }

                    CustomTraceManagerModule._appTraceSource = new TraceSource((String.IsNullOrWhiteSpace(sourceName)) ? context.GetType().Name : sourceName);
                }

                context.BeginRequest += this.Application_BeginRequest;
                context.AcquireRequestState += this.Application_AcquireRequestState;
                context.AuthenticateRequest += this.Application_AuthenticateRequest;
                context.AuthorizeRequest += this.Application_AuthorizeRequest;
                context.EndRequest += this.Application_EndRequest;
                context.Error += this.Application_Error;
                context.PostAcquireRequestState += this.Application_PostAcquireRequestState;
                context.PostAuthenticateRequest += this.Application_PostAuthenticateRequest;
                context.PostAuthorizeRequest += this.Application_PostAuthorizeRequest;
                context.PostRequestHandlerExecute += this.Application_PostRequestHandlerExecute;
                context.PreRequestHandlerExecute += this.Application_PreRequestHandlerExecute;
                
            }
        }

        private void _UnloadContext()
        {
            this._context.BeginRequest -= this.Application_BeginRequest;
            this._context.AcquireRequestState -= this.Application_AcquireRequestState;
            this._context.AuthenticateRequest -= this.Application_AuthenticateRequest;
            this._context.AuthorizeRequest -= this.Application_AuthorizeRequest;
            this._context.EndRequest -= this.Application_EndRequest;
            this._context.Error -= this.Application_Error;
            this._context.PostAcquireRequestState -= this.Application_PostAcquireRequestState;
            this._context.PostAuthenticateRequest -= this.Application_PostAuthenticateRequest;
            this._context.PostAuthorizeRequest -= this.Application_PostAuthorizeRequest;
            this._context.PostRequestHandlerExecute -= this.Application_PostRequestHandlerExecute;
            this._context.PreRequestHandlerExecute -= this.Application_PreRequestHandlerExecute;
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

            lock (CustomTraceManagerModule._syncRoot)
            {
                if (CustomTraceManagerModule._appTraceSource != null)
                {
                    CustomTraceManagerModule._appTraceSource.Flush();
                    CustomTraceManagerModule._appTraceSource.Close();
                    CustomTraceManagerModule._appTraceSource = null;
                }
            }
        }

        #endregion

        #region Application Event Handlers

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            // HttpContext.Current.Request
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
                CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, ExceptionEventInfo.Create(exception, message));
        }

        public static void TraceCritical(TraceEventId eventId, string message, Exception exception, object data0, params object[] nData)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, ExceptionEventInfo.Create(exception, message, data0, nData));
        }

        public static void TraceCritical(TraceEventId eventId, Exception exception, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Critical, eventId, ExceptionEventInfo.Create(exception, data));
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
                CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, ExceptionEventInfo.Create(exception, message));
        }

        public static void TraceError(TraceEventId eventId, string message, Exception exception, object data0, params object[] nData)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, ExceptionEventInfo.Create(exception, message, data0, nData));
        }

        public static void TraceError(TraceEventId eventId, Exception exception, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Error, eventId, ExceptionEventInfo.Create(exception, data));
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
                CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, ExceptionEventInfo.Create(exception, message));
        }

        public static void TraceWarning(TraceEventId eventId, string message, Exception exception, object data0, params object[] nData)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, ExceptionEventInfo.Create(exception, message, data0, nData));
        }

        public static void TraceWarning(TraceEventId eventId, Exception exception, params object[] data)
        {
            CustomTraceManagerModule.TraceData(TraceEventType.Warning, eventId, ExceptionEventInfo.Create(exception, data));
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

        public static void TraceEvent(TraceEventType eventType, TraceEventId eventId)
        {
            lock (CustomTraceManagerModule._syncRoot)
            {
                if (CustomTraceManagerModule._appTraceSource != null)
                    CustomTraceManagerModule._appTraceSource.TraceEvent(eventType, (int)eventId);
            }
        }

        public static void TraceEvent(TraceEventType eventType, TraceEventId eventId, string message)
        {
            lock (CustomTraceManagerModule._syncRoot)
            {
                if (CustomTraceManagerModule._appTraceSource != null)
                    CustomTraceManagerModule._appTraceSource.TraceEvent(eventType, (int)eventId, message);
            }
        }

        public static void TraceEvent(TraceEventType eventType, TraceEventId eventId, string format, params object[] args)
        {
            lock (CustomTraceManagerModule._syncRoot)
            {
                if (CustomTraceManagerModule._appTraceSource != null)
                    CustomTraceManagerModule._appTraceSource.TraceEvent(eventType, (int)eventId, format, args);
            }
        }

        public static void TraceData(TraceEventType eventType, TraceEventId eventId, object data)
        {
            lock (CustomTraceManagerModule._syncRoot)
            {
                if (CustomTraceManagerModule._appTraceSource != null)
                    CustomTraceManagerModule._appTraceSource.TraceData(eventType, (int)eventId, data);
            }
        }

        public static void TraceData(TraceEventType eventType, TraceEventId eventId, params object[] data)
        {
            lock (CustomTraceManagerModule._syncRoot)
            {
                if (CustomTraceManagerModule._appTraceSource != null)
                    CustomTraceManagerModule._appTraceSource.TraceData(eventType, (int)eventId, data);
            }
        }

        public static void TraceTransfer(TraceEventId eventId, string message, Guid relatedActivityId)
        {
            lock (CustomTraceManagerModule._syncRoot)
            {
                if (CustomTraceManagerModule._appTraceSource != null)
                    CustomTraceManagerModule._appTraceSource.TraceTransfer((int)eventId, message, relatedActivityId);
            }
        }

        #endregion
    }
}
