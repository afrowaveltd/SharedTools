namespace Afrowave.SharedTools.Localization.Common.Options
{
	/// <summary>
	/// Base class for strongly-typed plugin option models.
	/// Each implementation must define a unique Key.
	/// </summary>
	public abstract class PluginOptionSet
	{
		/// <summary>
		/// Unique key used to identify the option set (e.g. "SignalR", "Cache", etc.)
		/// </summary>
		public abstract string Key { get; }
	}
}