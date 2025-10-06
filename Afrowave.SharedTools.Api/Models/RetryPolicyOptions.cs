using System;

namespace Afrowave.SharedTools.Api.Models
{
	/// <summary>
	/// Provides configuration options for retry policies, including the maximum number of retries, base delay, backoff
	/// factor, and jitter settings.
	/// </summary>
	/// <remarks>Use this class to customize the behavior of retry mechanisms for transient fault handling. The
	/// options allow control over how retries are performed, including exponential backoff and optional randomization
	/// (jitter) to reduce contention in distributed scenarios.</remarks>
	public sealed class RetryPolicyOptions
	{
		/// <summary>
		/// Gets or sets the maximum number of retry attempts for a failed operation.
		/// </summary>
		/// <remarks>Set this property to control how many times the operation will be retried before failing. A value
		/// less than zero disables retries.</remarks>
		public int MaxRetries { get; set; } = 3;

		/// <summary>
		/// Gets or sets the base delay interval used for retry operations.
		/// </summary>
		/// <remarks>The base delay determines the initial wait time before a retry attempt is made. Adjust this value
		/// to control the minimum delay between retries. This property is typically used in conjunction with exponential
		/// backoff strategies.</remarks>
		public TimeSpan BaseDelay { get; set; } = TimeSpan.FromSeconds(2);

		/// <summary>
		/// Gets or sets the multiplier applied to the delay between retry attempts.
		/// </summary>
		/// <remarks>A higher backoff factor increases the delay exponentially with each retry, which can help reduce
		/// load on external resources during repeated failures. Typical values are greater than 1.0.</remarks>
		public double BackoffFactor { get; set; } = 2.0;

		/// <summary>
		/// Gets or sets a value indicating whether random jitter is applied to retry delays.
		/// </summary>
		/// <remarks>Enabling jitter helps to prevent synchronized retries across multiple clients, reducing the
		/// likelihood of contention and improving overall system stability.</remarks>
		public bool Jitter { get; set; } = true;

		/// <summary>
		/// Initializes a new instance of the RetryPolicyOptions class with default values for retry configuration.
		/// </summary>
		/// <remarks>The default settings are: MaxRetries set to 3, BaseDelay set to 2 seconds, BackoffFactor set to
		/// 2.0, and Jitter enabled. These defaults provide a reasonable starting point for most retry scenarios, but can be
		/// customized as needed for specific use cases.</remarks>
		public RetryPolicyOptions()
		{
			MaxRetries = 3;
			BaseDelay = TimeSpan.FromSeconds(2);
			BackoffFactor = 2.0;
			Jitter = true;
		}
	}
}