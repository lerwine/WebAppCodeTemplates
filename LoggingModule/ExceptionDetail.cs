using System;
using System.Linq;
using System.Xml.Serialization;

namespace Erwine.Leonard.T.LoggingModule
{
    [Serializable]
    [XmlRoot("Exception")]
    public class ExceptionDetail : EventInfoBase
    {
        [XmlAttribute("Type")]
        public string ExceptionType { get; set; }

        [XmlAttribute("Source")]
        public string Source { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string TargetSite { get; set; }

        public string AsString { get; set; }

        [XmlElement("InnerException")]
        public ExceptionDetail[] InnerExceptions { get; set; }

        public static ExceptionDetail Create(Exception exception, int maxDepth = 8)
        {
            ExceptionDetail result = new ExceptionDetail
            {
                ExceptionType = (exception == null) ? null : exception.GetType().FullName,
            };

            if (exception == null)
                return result;

            try
            {
                result.Message = exception.Message;
            }
            catch { } // Ignore

            try
            {
                result.Source = exception.Source;
            }
            catch { } // Ignore

            try
            {
                result.StackTrace = exception.StackTrace;
            }
            catch { } // Ignore

            try
            {
                result.TargetSite = (exception.TargetSite == null) ? null : exception.TargetSite.ToString();
            }
            catch { } // Ignore

            try
            {
                result.AsString = (exception == null) ? null : exception.ToString();
            }
            catch { } // Ignore

            if (maxDepth < 1)
                return result;

            Exception[] innerExceptions;
            try
            {
                innerExceptions = (exception.InnerException == null) ? new Exception[0] : new Exception[] { exception.InnerException };
            }
            catch
            {
                innerExceptions = new Exception[0];
            }

            if (exception is AggregateException)
            {
                try
                {
                    AggregateException aggregateException = exception as AggregateException;
                    if (aggregateException.InnerExceptions != null)
                        innerExceptions = innerExceptions.Concat(aggregateException.InnerExceptions
                            .Where(e => e != null && !innerExceptions.Any(i => Object.ReferenceEquals(e, i)))).ToArray();
                }
                catch { } // Ignore
            }

            result.InnerExceptions = innerExceptions.Take(64).Select(e => ExceptionDetail.Create(e, maxDepth - 1)).ToArray();

            return result;
        }
    }
}
