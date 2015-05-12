using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
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

        void IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotSupportedException();
        }

        void IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
        {
            foreach (XAttribute attribute in this._document.Root.Attributes())
                writer.WriteAttributeString(attribute.Name.LocalName, attribute.Name.NamespaceName, attribute.Value);

            foreach (XNode node in this._document.Root.Nodes())
                node.WriteTo(writer);
        }

        #endregion
    }
}
