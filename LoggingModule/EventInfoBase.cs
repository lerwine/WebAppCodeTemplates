using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Erwine.Leonard.T.LoggingModule
{
    public abstract class EventInfoBase
    {
        public override string ToString()
        {
            string result;

            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings settings= new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    XmlSerializer serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(writer, this);
                    writer.Flush();
                }

                result = settings.Encoding.GetString(stream.ToArray());
            }

            return result;
        }
    }
}
