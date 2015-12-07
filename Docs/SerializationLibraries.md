# Serialization Libraries Documentation



These deal with simple serialization tasks.


## ISimpleSerializer

Just take a dependency on ISimpleSerializer and use its methods:

- XmlSerialize() and XmlDeserialize() for serialization to/from XML strings
- JsonSerialize() and JsonDeserialize() for serialization to/from JSON strings
- Base64Serialize() and Base64Deserialize() for serialization to/from Base64 strings

Note that since DataContractSerializer and DataContractJsonSerializer are used under the hood for XML and JSON serialization you should decorate the serializable classes with the appropriate attributes.


## IdSerializer

This class provides simple methods for serializing a collection of numerical ids (like the ones Orchard content items have).

	var idsDefinition = IdSerializer.DeserializeIds(idsEnumerable);
	var idsEnumerable = IdSerializer.SerializeIds(idsDefinition);