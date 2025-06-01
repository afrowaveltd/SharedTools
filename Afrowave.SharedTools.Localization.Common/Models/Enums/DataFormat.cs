namespace Afrowave.SharedTools.Localization.Common.Models.Enums
{
	/// <summary>
	/// Specifies the format in which data is represented.
	/// </summary>
	/// <remarks>This enumeration is used to define the structure or organization of data. It can be used in
	/// scenarios where data needs to be serialized, deserialized, or processed in a specific format.</remarks>
	public enum DataFormat
	{
		/// <summary>
		/// Specifies that the data is represented as raw JSON.
		/// </summary>
		RawJson = 0,

		/// <summary>
		/// Represents a dictionary structure where all keys are stored in a flat hierarchy.
		/// </summary>
		/// <remarks>This enumeration value is typically used to indicate that the dictionary does not have nested or
		/// hierarchical keys. All keys exist at the same level, making it suitable for scenarios where simplicity and direct
		/// access to keys are required.</remarks>
		FlatDictionary = 1,

		/// <summary>
		/// Represents a structured tree format for organizing hierarchical data.
		/// </summary>
		/// <remarks>This enumeration value is typically used to specify that data should be represented or processed
		/// in a structured tree format, which is suitable for scenarios involving parent-child relationships  or nested
		/// structures.</remarks>
		StructuredTree = 2
	}
}