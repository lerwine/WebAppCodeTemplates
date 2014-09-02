using System;
using System.ComponentModel;

namespace Erwine.Leonard.T.LoggingModule
{
    public enum TraceEventId
    {
        [Description("Other Event or Event ID Unknown.")]
        Other = 0,

        [Description("Module Initialization Started.")]
        Lifecycle_Module_InitializationStart,
        Lifecycle_Application_BeginRequest,
        Lifecycle_Application_AuthenticateRequest,
        Lifecycle_Application_PostAuthenticateRequest,
        Lifecycle_Application_AuthorizeRequest,
        Lifecycle_Application_PostAuthorizeRequest,
        Lifecycle_Application_AcquireRequestState,
        Lifecycle_Application_PostAcquireRequestState,
        Lifecycle_Session_Start,
        Lifecycle_Application_PreRequestHandlerExecute,
        Lifecycle_Page_PreInit,
        Lifecycle_Control_Init,
        Lifecycle_Page_Init,
        Lifecycle_Page_InitComplete,
        Lifecycle_Page_PreLoad,
        Lifecycle_Page_Load,
        Lifecycle_Control_Load,
        Lifecycle_Page_LoadComplete,
        Lifecycle_Page_DataBinding,
        Lifecycle_Page_PreRender,
        Lifecycle_Control_PreRender,
        Lifecycle_Page_PreRenderComplete,
        Lifecycle_Page_SaveStateComplete,
        Lifecycle_Control_Unload,
        Lifecycle_Page_AbortTransaction,
        Lifecycle_Page_CommitTransaction,
        Lifecycle_Page_Unload,
        Lifecycle_Page_Disposing,
        Lifecycle_Page_Disposed,
        Lifecycle_Application_PostRequestHandlerExecute,
        Lifecycle_Application_EndRequest,
        Application_Error
    }
}
