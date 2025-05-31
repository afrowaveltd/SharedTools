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
		public bool UseRead { get; set; } = false;
		public bool UseWrite { get; set; } = false;
		public bool UseUpdate { get; set; } = false;
		public bool UseDelete { get; set; } = false;
		public bool UseBulkRead { get; set; } = false;
		public bool UseBulkWrite { get; set; } = false;
		public bool UseListLanguages { get; set; } = false;
		public bool UseCache { get; set; } = false;
		public bool UseStream { get; set; } = false;
		public bool UseSignalR { get; set; } = false;
		public bool UseEvents { get; set; } = false;
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