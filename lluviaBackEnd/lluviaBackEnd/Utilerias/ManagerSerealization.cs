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
        public static void Serealizar(T value)
        {
            XmlSerializer xsSubmit = new XmlSerializer(value.GetType());
            //var subReq = new MyObject();
            var xml = "";
            using (var sww = new StreamWriter("factura.xml"))
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    xsSubmit.Serialize(writer, value);
                    writer.Flush();
                    xml = sww.ToString(); // Your XML
                }
            }
        }
    }
}