using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using Erwine.Leonard.T.LoggingModule.ExtensionMethods;
using Erwine.Leonard.T.ExtensionMethods.AttributeTypes;

namespace Erwine.Leonard.T.LoggingModule
{
    public class CustomTraceManagerModule : IHttpModule
    {
        private static TraceSource _traceSource = null;

        public static TraceSource TraceSource
        {
            get
            {
                if (CustomTraceManagerModule._traceSource == null)
                    CustomTraceManagerModule._traceSource = new TraceSource(Properties.Settings.Default.TraceSource_Name, SourceLevels.All);

                return CustomTraceManagerModule._traceSource;
            }
        }

        public void Init(HttpApplication context)
        {
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

        void Application_BeginRequest(object sender, EventArgs e)
        {
            // HttpContext.Current.Request
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_BeginRequest);
        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_AuthenticateRequest);
        }

        void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostAuthenticateRequest);
        }

        void Application_AuthorizeRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_AuthorizeRequest);
        }

        void Application_PostAuthorizeRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostAuthorizeRequest);
        }

        void Application_AcquireRequestState(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_AcquireRequestState);
        }

        void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostAcquireRequestState);

            if (HttpContext.Current.Session.IsNewSession)
                CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Session_Start);
        }

        void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PreRequestHandlerExecute);
        }

        void Application_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_PostRequestHandlerExecute);
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Lifecycle_Application_EndRequest);
        }

        void Application_Error(object sender, EventArgs e)
        {
            CustomTraceManagerModule.TraceEvent(TraceEventType.Information, TraceEventId.Application_Error);
        }

        public static void TraceEvent(TraceEventType traceEventType, TraceEventId traceEventId)
        {
            CustomTraceManagerModule.TraceEvent(traceEventType, traceEventId, null);
        }

        public static void TraceEvent(TraceEventType traceEventType, TraceEventId traceEventId, string message)
        {
            throw new NotImplementedException();
        }

        public static void TraceEvent(TraceEventType traceEventType, TraceEventId traceEventId, string message, string detail)
        {
            string msg = (String.IsNullOrWhiteSpace(message)) ? String.Format("{0}: {1}", traceEventType.GetDescription(), traceEventId.GetEnumDescription()) : message;
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            HttpContext.Current.ApplicationInstance.BeginRequest -= this.Application_BeginRequest;
            HttpContext.Current.ApplicationInstance.AcquireRequestState -= this.Application_AcquireRequestState;
            HttpContext.Current.ApplicationInstance.AuthenticateRequest -= this.Application_AuthenticateRequest;
            HttpContext.Current.ApplicationInstance.AuthorizeRequest -= this.Application_AuthorizeRequest;
            HttpContext.Current.ApplicationInstance.EndRequest -= this.Application_EndRequest;
            HttpContext.Current.ApplicationInstance.Error -= this.Application_Error;
            HttpContext.Current.ApplicationInstance.PostAcquireRequestState -= this.Application_PostAcquireRequestState;
            HttpContext.Current.ApplicationInstance.PostAuthenticateRequest -= this.Application_PostAuthenticateRequest;
            HttpContext.Current.ApplicationInstance.PostAuthorizeRequest -= this.Application_PostAuthorizeRequest;
            HttpContext.Current.ApplicationInstance.PostRequestHandlerExecute -= this.Application_PostRequestHandlerExecute;
            HttpContext.Current.ApplicationInstance.PreRequestHandlerExecute -= this.Application_PreRequestHandlerExecute;
        }
    }
}
