using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static  T DeserializeToObject(string file)
        {
            System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(typeof(T));

            using (StringReader sr = new StringReader(file))
            {
                return (T)ser.Deserialize(sr);
            }
        }

        public static T DeseralizarXML(string archivo)
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