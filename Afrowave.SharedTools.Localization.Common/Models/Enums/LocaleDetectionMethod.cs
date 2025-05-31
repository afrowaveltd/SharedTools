namespace Afrowave.SharedTools.Localization.Common.Models.Enums
{
	/// <summary>
	/// Defines the methods available for detecting user language preference.
	/// </summary>
	public enum LocaleDetectionMethod
	{
		/// <summary>
		/// Detects the user language preference using query parameters.
		/// </summary>
		QueryParameter,

		/// <summary>
		/// Detects the user language preference using cookies.
		/// </summary>
		Cookie,

		/// <summary>
		/// Represents the header information for a request or response.
		/// </summary>
		/// <remarks>This class is typically used to store metadata associated with HTTP headers or similar
		/// constructs. It may include properties for key-value pairs, content type, or other header-specific
		/// details.</remarks>
		Header
	}
}