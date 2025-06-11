namespace Afrowave.SharedTools.Docs.Models.Dto
{
	/// <summary>
	/// Results of an SMTP test operation.
	/// </summary>
	public class SmtpTestResult
	{
		/// <summary>
		/// Success of the SMTP test
		/// </summary>
		public bool Success { get; set; } = false;

		/// <summary>
		/// Log of the SMTP test
		/// </summary>
		public string Log { get; set; } = string.Empty;

		/// <summary>
		/// Error message
		/// </summary>
		public string? Error { get; set; }
	}
}