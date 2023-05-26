using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace ECDataAPI.Common
{
    public  static class ExtensionMethods
    {
        public static string ObjectToXml(this object _object)
        {
            string xmlStr = string.Empty;

            XmlWriterSettings setting = new XmlWriterSettings()
            {
                Indent = false,
                //  OmitXmlDeclaration = true, // uncomment to remove xmlns namespace declarations
                NewLineChars = string.Empty,
                NewLineHandling = NewLineHandling.Entitize
            };

            using (StringWriter strWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(strWriter, setting))
                {
                    XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);

                    XmlSerializer serializer = new XmlSerializer(_object.GetType());
                    serializer.Serialize(xmlWriter, _object, namespaces);
                    xmlStr = strWriter.ToString();
                   // xmlWriter.Close();
                }
                //strWriter.Close();
            }
            return xmlStr;
        }


        /// <summary>
        /// Deserialize string to object Of Type O
        /// </summary>
        /// <typeparam name="O"></typeparam>
        /// <param name="sXmlText"></param>
        /// <returns>Success: Object of Type O 
        ///   Failure: default value of O</returns>
        
        public static O FromXmlStringToObject<O>(this string sXmlText)
        {
            return (O)new XmlSerializer(typeof(O)).Deserialize(new StringReader(sXmlText));
        }

        public static string ReplaceCharacters(this string s, char[] separators, string newVal)
        {
            string[] temp;
            temp = s.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            return String.Join(newVal, temp);
        }
    }
}