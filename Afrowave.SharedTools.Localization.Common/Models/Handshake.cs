using Afrowave.SharedTools.Localization.Common.Options;
using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Common.Models
{
	/// <summary>
	/// Defines the runtime handshake request for initializing a plugin.
	/// It specifies which features should be enabled by the host.
	/// </summary>
	public sealed class Handshake
	{
		/// <summary>
		/// Gets or sets a value indicating whether the read operation should be enabled.
		/// </summary>
		public bool UseRead { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether write operations are enabled.
		/// </summary>
		public bool UseWrite { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether updates should be applied.
		/// </summary>
		public bool UseUpdate { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the delete operation should be used.
		/// </summary>
		public bool UseDelete { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether bulk read operations are enabled.
		/// </summary>
		public bool UseBulkRead { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether bulk write operations should be used.
		/// </summary>
		public bool UseBulkWrite { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the application should use a list of supported languages.
		/// </summary>
		public bool UseListLanguages { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether caching is enabled.
		/// </summary>
		public bool UseCache { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether the operation should use a stream for processing.
		/// </summary>
		public bool UseStream { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether SignalR is enabled for the application.
		/// </summary>
		public bool UseSignalR { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether events should be used to notify changes or actions.
		/// </summary>
		public bool UseEvents { get; set; } = false;

		/// <summary>
		/// Gets or sets a value indicating whether logging is enabled.
		/// </summary>
		public bool UseLogging { get; set; } = false;

		/// <summary>
		/// Plugin options passed to this handshake, indexed by key.
		/// </summary>
		public Dictionary<string, PluginOptionSet> Options { get; set; } = new Dictionary<string, PluginOptionSet>();

		/// <summary>
		/// Helper to retrieve existing or create new option of given type.
		/// </summary>
		public T GetOrCreateOption<T>() where T : PluginOptionSet, new()
		{
			var temp = new T();
			if(Options.TryGetValue(temp.Key, out var existing) && existing is T casted)
				return casted;

			Options[temp.Key] = temp;
			return temp;
		}
	}
}