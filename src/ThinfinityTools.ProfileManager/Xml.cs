using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace ThinfinityTools.ProfileManager
{
    public class Xml
    {
        public Xml(string text)
        {
            Text = text;
        }

        /// <summary>
        /// And enumeration to select UTF8 or UTF16 encoding. This is used because the defautl UTF8 
        /// and Unicode encoding types don't capitalize the UTF characters in the Xml declaration.
        /// </summary>
        public enum XmlEncoding
        {
            UTF8, // By being first it is the default
            UTF16
        };

        /// <summary>
        /// The original Xml text as is.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// The Xml in a single line (no new lines or carriage returns). The data is trimmed and no more than a single space anywhere.
        /// </summary>
        public string LinearizeXml
        {
            get { return _LinearizeXml ?? (_LinearizeXml = Clean(Document, LinearizedSettings)); }
        }
        private string _LinearizeXml;

        /// <summary>
        /// And XDocument representation of the Xml. It uses the Linearized Xml not the original text.
        /// </summary>
        public XDocument Document
        {
            get { return _Document ?? (_Document = XDocument.Parse(GetLinearizedXml(Text))); }
        }
        private XDocument _Document;

        /// <summary>
        /// The Xml with each element properly indented on a separate line. The data is trimmed and no more than a single space anywhere.
        /// </summary>
        public string PrettyXml
        {
            get { return _PrettyXml ?? (_PrettyXml = Clean(Document, PrettySettings)); }
        }
        private string _PrettyXml;

        /// <summary>
        /// An enum that specifies whether to use UTF-8 or Unicode (UTF-16).
        /// Changing the encoding shouldn't change the original Text but pretty and linearized 
        /// versions of the Xml should change as well as the stream.
        /// </summary>
        public XmlEncoding Encoding { get; set; }

        /// <summary>
        /// A method that outputs the Xml as a stream. It outputs using the correct Encoding.
        /// It isn't enough to write encoding="UTF-8" in the Xml declaration if the output file
        /// is still UTF-16. Botht the labeling and the actually bits in the file should match.
        /// </summary>
        /// <returns>A file stream in the configured encoding.</returns>
        public Stream ToStream()
        {
            return new MemoryStream(ToByteArray());
        }

        /// <summary>
        /// This creates a byte array using the correct encoding.
        /// 
        /// Note: Naturally, UTF-8 has half as manay bytes as UTF-16, however,
        /// if UTF-8 is n bytes, UTF-16 will be 2*N+2 bytes. This is because
        /// "UTF-8" is five characters and "UTF-16" is six characters. 
        /// So a 100 byte UTF-8 file would be 202 bytes in UTF-16. 
        /// </summary>
        /// <returns>A byte[] array of the Xml string in the configured encoding.</returns>
        public byte[] ToByteArray()
        {
            return GetEncoding().GetBytes(PrettyXml ?? "");
        }

        /// <summary>
        /// A method to get the current encoding based on the Enum value.
        /// </summary>
        /// <returns>The correct Encoding.</returns>
        private Encoding GetEncoding()
        {
            switch (Encoding)
            {
                case XmlEncoding.UTF8:
                    return XmlUTF8Encoding.Instance;
                case XmlEncoding.UTF16:
                    return XmlUnicode.Instance;
                default:
                    return XmlUnicode.Instance;
            }
        }

        /// <summary>
        /// XmlWriterSettings for linearized Xml.
        /// </summary>
        private XmlWriterSettings LinearizedSettings
        {
            get
            {
                return new XmlWriterSettings
                {
                    Encoding = GetEncoding(),
                    Indent = false,
                    NewLineOnAttributes = false
                };
            }
        }

        /// <summary>
        /// XmlWriterSettings for Pretty Xml.
        /// </summary>
        private XmlWriterSettings PrettySettings
        {
            get
            {
                return new XmlWriterSettings
                {
                    Encoding = GetEncoding(),
                    Indent = true,
                    IndentChars = string.IsNullOrEmpty(IndentCharacters) ? "  " : IndentCharacters,
                    NewLineOnAttributes = false,
                    NewLineHandling = NewLineHandling.Replace
                };
            }
        }

        /// <summary>
        /// The characters to use for indenting Pretty Xml
        /// </summary>
        public string IndentCharacters { get; set; }

        /// <summary>
        /// The method that uses XDocument to do make clean (pretty or linearized) Xml
        /// </summary>
        /// <param name="doc">The XDcoument version of the Xml.</param>
        /// <param name="settings">The configured XmlWriterSettings.</param>
        /// <returns>A pretty Xml string.</returns>
        private string Clean(XDocument doc, XmlWriterSettings settings)
        {
            var sb = new StringBuilder();
            var stringWriter = new StringWriterWithEncoding(sb, GetEncoding());
            using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                doc.Save(xmlWriter);
                xmlWriter.Flush();
                return sb.ToString();
            }
        }

        /// <summary>
        /// A method that uses Regex to linearize Xml. The regex replaces methods are used.
        /// </summary>
        /// <param name="text">The Xml text</param>
        /// <returns>Linearized Xml string.</returns>
        private string GetLinearizedXml(string text)
        {
            // Replace all white space with a single space
            var halfclean = Regex.Replace(text, @"\s+", " ", RegexOptions.Singleline);

            // Trim after >.
            var clean75 = Regex.Replace(halfclean, @">\s+", ">");

            // Trim before <
            var fullclean = Regex.Replace(clean75, @"\s+<", "<");

            return fullclean;
        }

        /// <summary>
        /// This clas allows for the Xml to be created with the Xml declaration saying UTF-8
        /// </summary>
        private sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding _Encoding;

            public StringWriterWithEncoding(StringBuilder builder, Encoding encoding)
                : base(builder)
            {
                _Encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return _Encoding; }
            }
        }

        /// <summary>
        /// This class makes the UTF-8 text in the Xml declaration show up capitalized.
        /// </summary>
        private sealed class XmlUTF8Encoding : UTF8Encoding
        {
            public override string WebName
            {
                get { return base.WebName.ToUpper(); }
            }

            public override string HeaderName
            {
                get { return base.HeaderName.ToUpper(); }
            }

            public override string BodyName
            {
                get { return base.BodyName.ToUpper(); }
            }

            public static XmlUTF8Encoding Instance
            {
                get { return _XmlUTF8Encoding ?? (_XmlUTF8Encoding = new XmlUTF8Encoding()); }
            }
            private static XmlUTF8Encoding _XmlUTF8Encoding;
        }

        /// <summary>
        /// This class makes the UTF-16 text in the Xml declaration show up capitalized.
        /// </summary>
        private sealed class XmlUnicode : UnicodeEncoding
        {
            public override string WebName
            {
                get { return base.WebName.ToUpper(); }
            }

            public override string HeaderName
            {
                get { return base.HeaderName.ToUpper(); }
            }

            public override string BodyName
            {
                get { return base.BodyName.ToUpper(); }
            }

            public static XmlUnicode Instance
            {
                get { return _XmlUnicode ?? (_XmlUnicode = new XmlUnicode()); }
            }
            private static XmlUnicode _XmlUnicode;
        }
    }
}