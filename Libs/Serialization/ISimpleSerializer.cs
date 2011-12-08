using System;
using Orchard;

namespace Piedone.HelpfulLibraries.Serialization
{
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
        string Serialize<T>(T obj);

        /// <summary>
        /// Deserializes an object previously serialized with Serialize()
        /// </summary>
        /// <typeparam name="T">The type of the object that was serialized</typeparam>
        /// <param name="serialization">String serialization of the object</param>
        /// <returns>The deserialized object</returns>
        T Deserialize<T>(string serialization);
    }
}
