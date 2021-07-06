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
    public class ManagerSerealization <T> where T : class 
    {
        public static void Serealizar(T value , string path)
        {
            XmlSerializer xsSubmit = new XmlSerializer(value.GetType());
            //var subReq = new MyObject();
            var xml = "";
            using (var sww = new StreamWriter(path+".xml"))
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, value);
                    writer.Flush();
                    xml = sww.ToString(); // Your XML
                }
            }
        }
        public static string SerealizarToString(T value)
        {

            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter writer = new StreamWriter(stream))
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(writer, value);

                return Encoding.UTF8.GetString(stream.ToArray());
                // I am not 100% sure if this can be optimized
                //httpContextBase.Response.BinaryWrite(stream.ToArray());
            }
        }

        public static  T DeserializeXMLStringToObject(string file)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));
            using (StringReader sr = new StringReader(file))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static T DeseralizarXMLFromPath(string archivo)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamReader reader = new StreamReader(archivo))
                {
                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}