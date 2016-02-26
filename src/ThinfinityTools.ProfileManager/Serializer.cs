using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace ThinfinityTools.ProfileManager
{
    public class Serializer
    {
        /// <summary>
        /// This function returns the serialized XML as a string
        /// </summary>
        /// <typeparam name="T">The object type to serialize.</typeparam>
        /// <param name="t">The instance of the object.</param>
        /// <param name="inOmitXmlDeclaration"></param>
        /// <param name="inNameSpaces"></param>
        /// <param name="inEncoding"></param>
        public static string SerializeToXml<T>(T t, bool inOmitXmlDeclaration = false, XmlSerializerNamespaces inNameSpaces = null, Encoding inEncoding = null)
        {
            var ns = inNameSpaces;
            if (ns == null)
            {
                ns = new XmlSerializerNamespaces();
                ns.Add("", "");
            }
            var serializer = new XmlSerializer(t.GetType());
            var textWriter = (TextWriter)new StringWriter();
            if (inEncoding != null && inEncoding.Equals(Encoding.UTF8))
                textWriter = new Utf8StringWriter();
            var xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings { OmitXmlDeclaration = inOmitXmlDeclaration });
            serializer.Serialize(xmlWriter, t, ns);
            return textWriter.ToString();
        }

        public static void SerializeToXml<T>(T t, string outFilename, bool inOmitXmlDeclaration = false, XmlSerializerNamespaces inNameSpaces = null, Encoding inEncoding = null)
        {
            MakeDirectoryPath(outFilename);
            var ns = inNameSpaces;
            if (ns == null)
            {
                ns = new XmlSerializerNamespaces();
                ns.Add("", "");
            }
            var serializer = new XmlSerializer(t.GetType());
            var textWriter = (TextWriter)new StreamWriter(outFilename);
            if (inEncoding != null && inEncoding.Equals(Encoding.UTF8))
                textWriter = new Utf8StreamWriter(outFilename);
            var xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings { OmitXmlDeclaration = inOmitXmlDeclaration });
            serializer.Serialize(xmlWriter, t, ns);
            textWriter.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="outFilename"></param>
        private static void MakeDirectoryPath(string outFilename)
        {
            var dir = Path.GetDirectoryName(outFilename);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

        /// <summary>
        /// This function deserializes the XML file passed in.
        /// </summary>
        /// <typeparam name="T">The object type to serialize.</typeparam>
        /// <param name="inFilename">The file or full path to the file.</param>
        /// <returns>The object that was deserialized from xml.</returns>
        public static T DeserializeFromXml<T>(String inFilename)
        {
            if (string.IsNullOrWhiteSpace(inFilename))
            {
                return default(T);
            }
            // Wait 1 second if file doesn't exist, in case we are waiting on a
            // separate thread and beat it here.
            if (!File.Exists(inFilename))
                Thread.Sleep(1000);

            // File should exist by now.
            if (File.Exists(inFilename))
            {
                var deserializer = new XmlSerializer(typeof(T));
                var textReader = (TextReader)new StreamReader(inFilename);
                var reader = new XmlTextReader(textReader);
                reader.Read();
                var retVal = (T)deserializer.Deserialize(reader);
                textReader.Close();
                return retVal;
            }
            throw new FileNotFoundException(inFilename);
        }

        #region UTF8StringWriter
        public sealed class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        public sealed class Utf8StreamWriter : StreamWriter
        {
            public Utf8StreamWriter(string file)
                : base(file)
            {
            }

            public override Encoding Encoding => Encoding.UTF8;
        }
        #endregion
    }
}
