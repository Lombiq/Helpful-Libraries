using System;
using Orchard;

namespace Piedone.HelpfulLibraries.Serialization
{
    /// <summary>
    /// Easy object serialization and deserialization
    /// </summary>
    public interface ISimpleSerializer : ISingletonDependency
    {
        /// <summary>
        /// Serializes an object to string. Note that since the method uses DataContractSerializer under the hood classes
        /// and their members should be decorated with the appropriate attributes, like [DataContract] (for classes) and 
        /// [DataMember] (for properties).
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The string serialization of the object</returns>
        [Obsolete("Use XmlSerialize instead (it's the same, just renamed)")]
        string Serialize<T>(T obj);

        /// <summary>
        /// Deserializes an object previously serialized with Serialize()
        /// </summary>
        /// <typeparam name="T">The type of the object that was serialized</typeparam>
        /// <param name="serialization">String serialization of the object</param>
        /// <returns>The deserialized object</returns>
        [Obsolete("Use XmlDeserialize instead (it's the same, just renamed)")]
        T Deserialize<T>(string serialization);

        /// <summary>
        /// Serializes an object to an XML string. Note that since the method uses DataContractSerializer under the hood classes
        /// and their members should be decorated with the appropriate attributes, like [DataContract] (for classes) and 
        /// [DataMember] (for properties).
        /// </summary>
        /// <typeparam name="T">The type of the object to serialize</typeparam>
        /// <param name="obj">The object to serialize</param>
        /// <returns>The XML string serialization of the object</returns>
        string XmlSerialize<T>(T obj);

        /// <summary>
        /// Deserializes an object previously serialized with XmlSerialize()
        /// </summary>
        /// <typeparam name="T">The type of the object that was serialized</typeparam>
        /// <param name="serialization">XML string serialization of the object</param>
        /// <returns>The deserialized object</returns>
        T XmlDeserialize<T>(string serialization);
    }
}
