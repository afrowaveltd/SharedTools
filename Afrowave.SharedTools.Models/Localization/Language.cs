using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Afrowave.SharedTools.Models.Localization
{
	public class Language
	{
		/// <summary>
		/// Gets or sets the code associated with this object.
		/// </summary>
		[JsonPropertyName("code")]
		public string Code { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the name associated with this instance.
		/// </summary>
		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the native representation of the value as a string.
		/// </summary>
		[JsonPropertyName("native")]
		public string Native { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets a value indicating whether the content should be displayed in a right-to-left layout.
		/// </summary>
		[JsonPropertyName("rtl")]
		public bool Rtl { get; set; } = false;
	}
}