using System;

namespace Afrowave.SharedTools.Ftp.Options
{
	/// <summary>
	/// Provides configuration options for retry policies applied to FTP operations.
	/// </summary>
	/// <remarks>
	/// Use these settings to control how transient errors are retried, including the number of attempts,
	/// initial delay, exponential backoff factor, and optional jitter to reduce coordinated retries.
	/// </remarks>
	public sealed class RetryPolicyOptions
	{
		/// <summary>
		/// Gets or sets the maximum number of retry attempts for a failed operation. Default is 3.
		/// </summary>
		public int MaxRetries { get; set; } = 3;

		/// <summary>
		/// Gets or sets the base delay used before the first retry. Default is 250 ms.
		/// </summary>
		public TimeSpan BaseDelay { get; set; } = TimeSpan.FromMilliseconds(250);

		/// <summary>
		/// Gets or sets the factor by which the delay increases after each attempt. Default is 2.0.
		/// </summary>
		public double BackoffFactor { get; set; } = 2.0;

		/// <summary>
		/// Gets or sets a value indicating whether to apply random jitter to the computed delay. Default is true.
		/// </summary>
		public bool Jitter { get; set; } = true;
	}
}