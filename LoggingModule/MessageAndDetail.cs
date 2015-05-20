using System.Runtime.Serialization;

namespace Erwine.Leonard.T.LoggingModule
{
    [DataContract(Name = "Message", Namespace = "")]
    public class MessageAndDetail
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public string Detail { get; set; }

        public MessageAndDetail(string message, string detail)
        {
            this.Text = message;
            this.Detail = detail;
        }
    }
}
