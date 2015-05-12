using System;
using System.Linq;
using System.Xml.Serialization;

namespace Erwine.Leonard.T.LoggingModule
{
    [Serializable]
    [XmlRoot("ExceptionEvent")]
    public class ExceptionEventInfo : EventInfoBase
    {
        public string Message { get; set; }

        [XmlElement("Exception")]
        public ExceptionDetail ExceptionObj { get; set; }

        [XmlElement("Data")]
        public object[] Data { get; set; }

        public ExceptionEventInfo() { }

        public static ExceptionEventInfo Create(Exception exception, string message = null)
        {
            return new ExceptionEventInfo
            {
                ExceptionObj = ExceptionDetail.Create(exception),
                Message = message
            };
        }

        public static ExceptionEventInfo Create(Exception exception, string message, object data0, params object[] nData)
        {
            ExceptionEventInfo result = ExceptionEventInfo.Create(exception, message);
            result.Data = (nData == null) ? new object[] { data0 } : (new object[] { data0 }).Concat(nData).ToArray();
            return result;
        }

        public static ExceptionEventInfo Create(Exception exception, params object[] data)
        {
            ExceptionEventInfo result = ExceptionEventInfo.Create(exception, null as string);
            result.Data = data;
            return result;
        }
    }
}
