using System;
using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Common.Communication
{
	/// <summary>
	/// Describes metadata for a plugin: identity, origin and versioning.
	/// </summary>
	public sealed class Metadata
	{
		/// <summary>
		/// Unique plugin identifier for the current instance (auto-generated GUID).
		/// </summary>
		public string Id { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		/// Display name of the plugin.
		/// </summary>
		public string Name { get; set; } = string.Empty;

		/// <summary>
		/// Short description of the plugin’s purpose and behavior.
		/// </summary>
		public string Description { get; set; } = string.Empty;

		/// <summary>
		/// Version string of the plugin.
		/// </summary>
		public string Version { get; set; } = string.Empty;

		/// <summary>
		/// Author or maintainer of the plugin.
		/// </summary>
		public string Author { get; set; } = string.Empty;

		/// <summary>
		/// License type (e.g., MIT, Apache 2.0).
		/// </summary>
		public string License { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the collection of tags associated with the current object.
		/// </summary>
		public List<string> Tags { get; set; } = new List<string>();
	}
}