using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Erwine.Leonard.T.LoggingModule
{
    [XmlRoot("TraceData", Namespace = "")]
    public class TraceDataObject : IXmlSerializable
    {
        private XDocument _document = null;

        public XDocument Document
        {
            get
            {
                if (this._document == null)
                    this.Initialize(null);

                return this._document;
            }
        }

        public TraceDataObject() { }

        public TraceDataObject(object value)
        {
            this.Initialize(value);
        }

        public override string ToString()
        {
            string value;

            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    this.Document.WriteTo(writer);
                    writer.Flush();
                }

                value = settings.Encoding.GetString(stream.ToArray());
            }

            return value;
        }

        private void Initialize(object value)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(stream, settings))
                {
                    writer.WriteStartDocument(true);
                    writer.WriteStartElement("TraceData");
                    
                    writer.WriteAttributeString("xmlns", "xsi", XNamespace.Xmlns.NamespaceName, "http://www.w3.org/2001/XMLSchema-instance");
                    writer.WriteAttributeString("xmlns", "ser", XNamespace.Xmlns.NamespaceName, "http://schemas.microsoft.com/2003/10/Serialization/");

                    TraceDataSerializer serializer = new TraceDataSerializer();
                    serializer.SerializeObject(writer, value);
                    writer.WriteEndElement();
                    writer.Flush();
                }

                stream.Seek(0L, SeekOrigin.Begin);

                using (XmlReader reader = XmlReader.Create(stream))
                    this._document = XDocument.Load(reader);
            }
        }

        #region IXmlSerializable Members

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader) { throw new NotSupportedException(); }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (XAttribute attribute in this._document.Root.Attributes())
                writer.WriteAttributeString(attribute.Name.LocalName, attribute.Name.NamespaceName, attribute.Value);

            foreach (XNode node in this._document.Root.Nodes())
                node.WriteTo(writer);
        }

        #endregion

        public static object EnsureSerializable(object data)
        {   
            if (data == null || data is TraceDataObject)
                return data;

            Type t = data.GetType();

            while (t.IsArray)
                t = t.GetElementType();
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                t = Nullable.GetUnderlyingType(t);

            if (t.IsPrimitive || typeof(TraceDataObject).IsAssignableFrom(t))
                return data;

            return new TraceDataObject(data);
        }

        internal static TraceDataObject CreateExceptionEventData(TraceEventId eventId, Exception exception, string message, params object[] data)
        {
            TraceDataSerializer serializer = new TraceDataSerializer();
            TraceDataObject result;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    Indent = true
                };

                using (XmlWriter writer = XmlWriter.Create(ms, settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("TraceData");

                    try
                    {
                        serializer.SerializeObject(writer, eventId);

                        if (message != null)
                        {
                            writer.WriteStartElement("Message");
                            if (message.Length > 0)
                                writer.WriteCData(message);
                            writer.WriteEndElement();
                        }

                        if (exception != null)
                            serializer.SerializeObject(writer, exception);

                        if (data != null)
                        {
                            writer.WriteStartElement("Data");
                            try
                            {
                                foreach (object o in data)
                                    serializer.SerializeObject(writer, o);
                            }
                            catch { throw; }
                            finally
                            {
                                writer.WriteEndElement();
                            }

                        }
                    }
                    catch { throw; }
                    finally
                    {
                        writer.WriteEndElement();
                        writer.WriteEndDocument();
                    }
                    
                    writer.Flush();
                }

                ms.Seek(0L, SeekOrigin.Begin);

                using (XmlReader reader = XmlReader.Create(ms))
                    result = new TraceDataObject { _document = XDocument.Load(reader) };
            }

            return result;
        }

        internal static TraceDataObject CreateExceptionEventData(TraceEventId eventId, Exception exception, object[] data)
        {
            return TraceDataObject.CreateExceptionEventData(eventId, exception, null as string, data);
        }
    }
}
