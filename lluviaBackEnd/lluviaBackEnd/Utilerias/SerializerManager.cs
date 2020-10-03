using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace lluviaBackEnd.Utilerias
{
    public static class SerializerManager<T> where T : class
    {

        public static T DeseralizarXML(string arhivo)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamReader reader = new StreamReader(arhivo))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T DeseralizarStringToObject(string value)
        {
            object result;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                using (TextReader reader = new StringReader(value))
                {
                    result = serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return (T)result;
        }

        public static void Serealizar(string nombreArchivo, T value)
        {
            try
            {
                int x = nombreArchivo.LastIndexOf(@"\");
                string pathDirectorio = nombreArchivo.Substring(0, x);
                if (!Directory.Exists(pathDirectorio))
                {
                    Directory.CreateDirectory(pathDirectorio);
                }
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                MemoryStream ms = new MemoryStream();
                XmlTextWriter xmlTextWriter = new XmlTextWriter(nombreArchivo, Encoding.UTF8);
                xmlTextWriter.Formatting = Formatting.Indented;
                serializer.Serialize(xmlTextWriter, value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string SerealizarObjtecToString(T value)
        {
            String result = string.Empty;
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StringWriter writer = new StringWriter())
                {
                    serializer.Serialize(writer, value);

                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }


    }

}