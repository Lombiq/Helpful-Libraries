using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orchard.Environment.Extensions;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace Piedone.HelpfulLibraries.Serialization.Helpers
{
    /// <summary>
    /// Simplified serialization to/from strings. Uses DataContractSerializer under the hood, therefore classes 
    /// (and their members)of objects used to serialize should be decorated with the appropriate attributes, like
    /// [DataContract] (for classes) and [DataMember] (for properties).
    /// </summary>
    [OrchardFeature("Piedone.HelpfulLibraries.Serialization")]
    public class Serializer
    {
        /// <summary>
        /// Serializes an object to string. Note that since the method uses DataContractSerializer under the hood classes
        /// and their members should be decorated with the appropriate attributes, like [DataContract] (for classes) and 
        /// [DataMember] (for properties).
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The string serialization of the object</returns>
        public static string Serialize<T>(T obj)
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

        /// <summary>
        /// Deserializes an object previously serialized with Serialize()
        /// </summary>
        /// <typeparam name="T">The type of the object that was serialized</typeparam>
        /// <param name="serialization">String serialization of the object</param>
        /// <returns>The deserialized object</returns>
        public static T Deserialize<T>(string serialization)
        {
            var serializer = new DataContractSerializer(typeof(T));
            var doc = new XmlDocument();
            doc.LoadXml(serialization);
            var reader = new XmlNodeReader(doc.DocumentElement);
            return (T)serializer.ReadObject(reader);
        }
    }
}
