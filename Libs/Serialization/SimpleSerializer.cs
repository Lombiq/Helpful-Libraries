using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace Piedone.HelpfulLibraries.Serialization
{
    /// <summary>
    /// Simplified serialization to/from strings. Uses DataContractSerializer under the hood, therefore classes 
    /// (and their members)of objects used to serialize should be decorated with the appropriate attributes, like
    /// [DataContract] (for classes) and [DataMember] (for properties).
    /// </summary>
    [OrchardFeature("Piedone.HelpfulLibraries.Serialization")]
    public class SimpleSerializer : ISimpleSerializer
    {
        public string Serialize<T>(T obj)
        {
            string serialization;

            using (var sw = new StringWriter())
            {
                using (var writer = new XmlTextWriter(sw))
                {
                    var serializer = new DataContractSerializer(obj.GetType());
                    serializer.WriteObject(writer, obj);
                    writer.Flush();
                    serialization = sw.ToString();
                }
            }

            return serialization;
        }

        public T Deserialize<T>(string serialization)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var doc = new XmlDocument();
            doc.LoadXml(serialization);
            var reader = new XmlNodeReader(doc.DocumentElement);
            return (T)serializer.ReadObject(reader);
        }
    }
}
