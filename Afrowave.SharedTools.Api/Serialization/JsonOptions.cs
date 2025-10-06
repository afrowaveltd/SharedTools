using System.Text.Json;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Api.Serialization
{
	/// <summary>
	/// Provides a set of default options for JSON serialization and deserialization using <see
	/// cref="System.Text.Json.JsonSerializerOptions"/>.
	/// </summary>
	/// <remarks>The default options use camel case property naming, skip comments when reading, allow trailing
	/// commas, and ignore properties with null values when writing. Indented formatting is disabled. These settings are
	/// suitable for most common scenarios where compact, camel-cased JSON is preferred.</remarks>
	public static class JsonOptions
	{
		/// <summary>
		/// Gets the default configuration options for JSON serialization and deserialization.
		/// </summary>
		/// <remarks>The returned options use camel case property naming, skip comments during reading, allow trailing
		/// commas, ignore null values when writing, and do not write indented JSON. These defaults are suitable for most
		/// scenarios where compact, standard-compliant JSON is required.</remarks>
		public static JsonSerializerOptions Default
		{
			get
			{
				var o = new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
					WriteIndented = false,
					AllowTrailingCommas = true,
					ReadCommentHandling = JsonCommentHandling.Skip
				};
				o.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
				return o;
			}
		}
	}
}