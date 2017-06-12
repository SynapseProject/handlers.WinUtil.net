using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Synapse.Handlers.WinUtil
{
    public class XmlHelpers
    {
        public static string Serialize<T>(object data, bool omitXmlDeclaration = true, bool omitXmlNamespace = true,
            bool indented = true, Encoding encoding = null)
        {
            if( string.IsNullOrWhiteSpace( data?.ToString() ) )
                return null;

            if( encoding == null )
                encoding = UnicodeEncoding.UTF8;

            XmlWriterSettings settings = new XmlWriterSettings()
            {
                OmitXmlDeclaration = omitXmlDeclaration,
                ConformanceLevel = ConformanceLevel.Auto,
                Encoding = encoding,
                CloseOutput = false,
                Indent = indented
            };

            XmlSerializer s = new XmlSerializer( typeof( T ) );
            StringBuilder buf = new StringBuilder();
            XmlWriter w = XmlWriter.Create( buf, settings );
            if( data is XmlDocument )
            {
                ((XmlDocument)data).Save( w );
            }
            else
            {
                if( omitXmlNamespace )
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add( "", "" );
                    s.Serialize( w, data, ns );
                }
                else
                {
                    s.Serialize( w, data );
                }
            }
            w.Close();

            return buf.ToString();
        }

        public static T Deserialize<T>(string s)
        {
            StringReader sr = new StringReader( s );
            return Deserialize<T>( sr );
        }

        public static T Deserialize<T>(TextReader reader)
        {
            XmlSerializer s = new XmlSerializer( typeof( T ) );
            return (T)s.Deserialize( reader );
        }
    }
}


