namespace Afrowave.SharedTools.Localization.Common.Communication
{
	/// <summary>
	/// Defines plugin behavior – modifiers for how active capabilities behave.
	/// </summary>
	public sealed class Behavior
	{
		/// <summary>
		/// If true, the plugin uses the key as fallback when no translation is found.
		/// </summary>
		public bool UseKeyAsFallback { get; set; } = false;

		/// <summary>
		/// Enables automatic reload when external changes are detected.
		/// </summary>
		public bool AutoReloadEnabled { get; set; } = false;

		/// <summary>
		/// Expected average response time in milliseconds.
		/// </summary>
		public int ExpectedResponseTimeMs { get; set; } = 500;
	}
}