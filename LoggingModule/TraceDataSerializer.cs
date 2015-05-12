using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Erwine.Leonard.T.LoggingModule.ExtensionMethods;

namespace Erwine.Leonard.T.LoggingModule
{
    public class TraceDataSerializer
    {
        private Dictionary<Type, DataContractSerializer> _serializers = new Dictionary<Type, DataContractSerializer>();

        private DataContractSerializer GetDataContractSerializer(Type type)
        {
            DataContractSerializer serializer;

            lock (this._serializers)
            {
                if (this._serializers.ContainsKey(type))
                    serializer = this._serializers[type];
                else
                {
                    serializer = new DataContractSerializer(type);
                    this._serializers.Add(type, serializer);
                }
            }

            return serializer;
        }

        public void SerializeObject(XmlWriter writer, object obj)
        {
            if (obj != null)
            {
                if (obj is Exception)
                {
                    this.SerializeException(writer, obj as Exception, obj.GetType().ToXmlElementName());
                    return;
                }
                else if (obj is SqlError)
                {
                    this.SerializeSqlError(writer, obj as SqlError);
                    return;
                }
            }

            Type sourceType = (obj == null) ? typeof(object) : obj.GetType();
            Type[] knownTypes;
            if (obj != null && obj is IEnumerable)
                knownTypes = (obj as IEnumerable).GetAllElementTypes();
            else
                knownTypes = new Type[0];

            DataContractSerializer serializer;

            if (knownTypes.Length > 0)
                serializer = new DataContractSerializer(sourceType, knownTypes);
            else
                serializer = this.GetDataContractSerializer(sourceType);

            if (serializer != null)
            {
                try
                {
                    serializer.WriteObject(writer, obj);
                    if (knownTypes.Length > 0)
                    {
                        lock (this._serializers)
                        {
                            foreach (Type t in knownTypes.Where(k => !this._serializers.ContainsKey(k)))
                                this._serializers.Add(t, new DataContractSerializer(t));
                        }
                    }
                }
                catch
                {
                    if (knownTypes.Length == 0)
                    {
                        lock (this._serializers)
                        {
                            if (obj != null && !sourceType.Equals(typeof(object)) && this._serializers.ContainsKey(sourceType))
                                this._serializers.Remove(sourceType);
                        }
                    }
                    
                    serializer = null;
                }

                if (serializer != null)
                    return;
            }

            if (obj == null)
                TraceDataSerializer.WriteElement(writer, typeof(object));
            else if (obj is IEnumerable)
                this.SerializeEnumerable(writer, obj as IEnumerable);
            else
                TraceDataSerializer.WriteElement(writer, obj.GetType(), obj.ToString());
        }

        private static void WriteElement(XmlWriter writer, Type sourceType, string content = null)
        {
            TraceDataSerializer.WriteStartElement(writer, sourceType);
            if (content != null)
                writer.WriteCData(content);
            writer.WriteEndElement();
        }

        private static void WriteElement(XmlWriter writer, XName name, string content = null)
        {
            if (String.IsNullOrEmpty(name.NamespaceName))
                writer.WriteStartElement(name.LocalName);
            else
                writer.WriteStartElement(name.LocalName, name.NamespaceName);
            if (content != null)
                writer.WriteCData(content);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes start element to correspond with <paramref name="sourceType"/>.
        /// </summary>
        /// <param name="writer">Writer to write element to.</param>
        /// <param name="sourceType">Type of object being serialized.</param>
        public static void WriteStartElement(XmlWriter writer, Type sourceType)
        {
            writer.WriteStartElement(sourceType.ToXmlElementName());
            writer.WriteAttributeString("type", "urn:lte-temp", sourceType.ToCSharpTypeName());
        }

        /// <summary>
        /// Serialized an enumerable to an xml writer.
        /// </summary>
        /// <param name="writer">XML writer to write enumerable to.</param>
        /// <param name="enumerable">Enumerable object to be written.</param>
        public void SerializeEnumerable(XmlWriter writer, IEnumerable enumerable, int maxDepth = 24)
        {
            if (enumerable == null)
            {
                this.WriteObject(null, writer);
                return;
            }

            TraceDataSerializer.WriteStartElement(writer, enumerable.GetType());

            try
            {
                if (maxDepth < 1)
                    writer.WriteAttributeString("Count", enumerable.Cast<object>().Count().ToString());
                else
                {
                    foreach (object value in enumerable)
                    {
                        if (value == null)
                            this.WriteObject(value, writer);
                        else if (value is IEnumerable)
                            this.SerializeEnumerable(writer, value as IEnumerable, maxDepth - 1);
                        else if (value is Exception)
                            this.SerializeException(writer, value as Exception, value.GetType().ToXmlElementName(), maxDepth - 1);
                        else if (value is SqlError)
                            this.SerializeSqlError(writer, value as SqlError);
                        else
                        {
                            Type vType = value.GetType();
                            if (vType.IsEnum)
                                this.SerializeEnumValue(writer, value);
                            else if (vType.IsPrimitive || value is IXmlSerializable || vType.GetCustomAttributes(typeof(DataContractAttribute), true).OfType<DataContractAttribute>().Any())
                                this.WriteObject(value, writer);
                            else
                            {
                                try
                                {
                                    this.WriteObject(value, writer);
                                }
                                catch
                                {
                                    TraceDataSerializer.WriteElement(writer, value.GetType(), value.ToString());
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                writer.WriteEndElement();
            }
        }

        private void SerializeEnumValue(XmlWriter writer, object value)
        {
            // TODO: Need to handle flags
            writer.WriteStartElement(Enum.GetName(value.GetType(), value));
            writer.WriteAttributeString("Value", Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())).ToString());
            writer.WriteAttributeString("Type", value.GetType().ToCSharpTypeName());
            writer.WriteEndElement();
        }

        private void SerializeException(XmlWriter writer, SqlException exception, string name, int maxDepth = 24)
        {
            writer.WriteStartElement(name);
            try
            {
                this.WriteExceptionStart(writer, exception, name);
                TraceDataSerializer.TrySetElement(writer, XName.Get("TargetSite", ""), () => exception.Class.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("ErrorCode", ""), () => exception.ErrorCode.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("State", ""), () => exception.State.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("LineNumber", ""), () => exception.LineNumber.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("Number", ""), () => exception.Number.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("Procedure", ""), () => exception.Procedure);
                TraceDataSerializer.TrySetElement(writer, XName.Get("Server", ""), () => exception.Server);
                SqlError[] errors;
                try
                {
                    errors = (exception.Errors != null) ? exception.Errors.OfType<SqlError>().ToArray() : new SqlError[0];
                }
                catch
                {
                    errors = new SqlError[0];
                }

                foreach (SqlError e in errors)
                    this.SerializeSqlError(writer, e);

                this.WriteExceptionEnd(writer, exception, name, maxDepth);
            }
            catch
            {
                throw;
            }
            finally
            {
                writer.WriteEndElement();
            }
        }

        private void SerializeException(XmlWriter writer, Exception exception, string name, int maxDepth = 24)
        {
            if (exception != null)
            {
                if (exception is SqlException)
                {
                    this.SerializeException(writer, exception as SqlException, name, maxDepth);
                    return;
                }
            }

            writer.WriteStartElement(name);
            try
            {
                this.WriteExceptionStart(writer, exception, name);
                this.WriteExceptionEnd(writer, exception, name, maxDepth);
            }
            catch
            {
                throw;
            }
            finally
            {
                writer.WriteEndElement();
            }
        }

        private void WriteExceptionEnd(XmlWriter writer, Exception exception, string name, int maxDepth)
        {
            TraceDataSerializer.TrySetElement(writer, XName.Get("TargetSite", ""), () => exception.TargetSite.ToString());
            TraceDataSerializer.TrySetElement(writer, XName.Get("StackTrace", ""), () => exception.StackTrace);

            List<Exception> innerExceptions = new List<Exception>();
            try
            {
                if (exception.InnerException != null)
                    innerExceptions.Add(exception.InnerException);
            }
            catch
            {
                // Safe to ignore
            }
            if (exception is AggregateException)
            {
                try
                {
                    AggregateException aggregateException = exception as AggregateException;
                    if (aggregateException.InnerExceptions != null)
                    {
                        foreach (Exception e in aggregateException.InnerExceptions)
                        {
                            if (e != null && !innerExceptions.Any(i => Object.ReferenceEquals(e, i)))
                                innerExceptions.Add(e);
                        }
                    }
                }
                catch
                {
                    // Safe to ignore
                }
            }

            if (innerExceptions.Count == 0)
                return;

            if (maxDepth > 0)
            {
                foreach (Exception exc in innerExceptions)
                    this.SerializeException(writer, exc, "InnerException", maxDepth - 1);
                return;
            }

            writer.WriteStartElement("InnerException");
            writer.WriteAttributeString("Count", innerExceptions.Count.ToString());
            writer.WriteEndElement();
        }

        private static void TrySetElement(XmlWriter writer, XName n, Func<string> func)
        {
            string s;
            try
            {
                s = func();
            }
            catch
            {
                s = null;
            }

            if (s == null)
                return;

            if (String.IsNullOrEmpty(n.NamespaceName))
                writer.WriteElementString(n.LocalName, s);
            else
                writer.WriteElementString(n.LocalName, n.NamespaceName, s);
        }

        private void WriteExceptionStart(XmlWriter writer, Exception exception, string name)
        {
            writer.WriteAttributeString("Type", exception.GetType().ToCSharpTypeName());

            TraceDataSerializer.TrySetElement(writer, XName.Get("Message", ""), () => exception.Message);
            TraceDataSerializer.TrySetElement(writer, XName.Get("Source", ""), () => exception.Source);
        }

        private void SerializeSqlError(XmlWriter writer, SqlError sqlError)
        {
            writer.WriteStartElement("Error");
            try
            {
                TraceDataSerializer.TrySetElement(writer, XName.Get("Message", ""), () => sqlError.Message);
                TraceDataSerializer.TrySetElement(writer, XName.Get("Source", ""), () => sqlError.Source);
                TraceDataSerializer.TrySetElement(writer, XName.Get("TargetSite", ""), () => sqlError.Class.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("State", ""), () => sqlError.State.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("LineNumber", ""), () => sqlError.LineNumber.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("Number", ""), () => sqlError.Number.ToString());
                TraceDataSerializer.TrySetElement(writer, XName.Get("Procedure", ""), () => sqlError.Procedure);
                TraceDataSerializer.TrySetElement(writer, XName.Get("Server", ""), () => sqlError.Server);

            }
            catch
            {
                throw;
            }
            finally
            {
                writer.WriteEndElement();
            }
        }

        public void WriteObject(object obj, XmlWriter writer)
        {
            DataContractSerializer serializer = new DataContractSerializer((obj == null) ? typeof(object) : obj.GetType());
            serializer.WriteObject(writer, obj);
        }

        private static XmlWriterSettings _EnsureXmlWriterSettings(XmlWriterSettings settings)
        {
            if (settings == null)
            {
                return new XmlWriterSettings
                {
                    CloseOutput = false,
                    Encoding = Encoding.UTF8,
                    Indent = true
                };
            }

            if (settings.Encoding != null && !settings.CloseOutput)
                return settings;

            return new XmlWriterSettings
            {
                CheckCharacters = settings.CheckCharacters,
                CloseOutput = false,
                ConformanceLevel = settings.ConformanceLevel,
                Encoding = (settings.Encoding == null) ? Encoding.UTF8 : settings.Encoding,
                Indent = settings.Indent,
                IndentChars = settings.IndentChars,
                NamespaceHandling = settings.NamespaceHandling,
                NewLineChars = settings.NewLineChars,
                NewLineHandling = settings.NewLineHandling,
                NewLineOnAttributes = settings.NewLineOnAttributes,
                OmitXmlDeclaration = settings.OmitXmlDeclaration
            };
        }

        //public void SerializeToStream(object obj, Stream stream, XmlWriterSettings settings = null)
        //{
        //    using (XmlWriter writer = XmlWriter.Create(stream, TraceDataSerializer._EnsureXmlWriterSettings(settings)))
        //    {
        //        this.WriteObject(obj, writer);
        //        writer.Flush();
        //    }
        //}

        //public string SerializeToString(object obj, XmlWriterSettings settings = null)
        //{
        //    string result;

        //    using (MemoryStream stream = new MemoryStream())
        //    {
        //        XmlWriterSettings xmlWriterSettings = TraceDataSerializer._EnsureXmlWriterSettings(settings);
        //        this.SerializeToStream(obj, stream, xmlWriterSettings);
        //        result = xmlWriterSettings.Encoding.GetString(stream.ToArray());
        //    }

        //    return result;
        //}

    }
}
