using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Erwine.Leonard.T.LoggingModule.ExtensionMethods
{
    public static class TraceEventTypeExtensions
    {
        public static string GetDescription(this TraceEventType traceEventType)
        {
            switch (traceEventType)
            {
                case TraceEventType.Critical:
                    return "Fatal error or application crash";
                case TraceEventType.Error:
                    return "Recoverable error";
                case TraceEventType.Warning:
                    return "Noncritical problem";
                case TraceEventType.Information:
                    return "Informational message";
                case TraceEventType.Verbose:
                    return "Debugging trace";
                case TraceEventType.Start:
                    return "Starting of a logical operation";
                case TraceEventType.Stop:
                    return "Stopping of a logical operation";
                case TraceEventType.Suspend:
                    return "Suspension of a logical operation";
                case TraceEventType.Resume:
                    return "Resumption of a logical operation";
                case TraceEventType.Transfer:
                    return "Changing of correlation identity";
            }

            return traceEventType.ToString("F");
        }

        public static string ToCategory(this TraceEventType traceEventType)
        {
            switch (traceEventType)
            {
                case TraceEventType.Critical:
                    return "Critical Error";
                case TraceEventType.Verbose:
                    return "Debug Message";
                case TraceEventType.Information:
                    return "Informational Message";
                case TraceEventType.Start:
                    return "Start Operation";
                case TraceEventType.Stop:
                    return "Stop Operation";
                case TraceEventType.Suspend:
                    return "Suspend Operation";
                case TraceEventType.Resume:
                    return "Resume Operation";
                case TraceEventType.Transfer:
                    return "Correlation change";
            }

            return traceEventType.ToString("F");
        }
    }
}
