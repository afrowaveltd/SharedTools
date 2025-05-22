namespace Afrowave.SharedTools.Models.Results
{
	/// <summary>
	/// Represents a boolean result of an operation without data.
	/// </summary>
	public class Result
	{
		/// <summary>
		/// Gets or sets a value indicating whether the operation was successful.
		/// </summary>
		public bool Success { get; set; } = true;

		/// <summary>
		/// Gets or sets the message associated with the result.
		/// </summary>
		public string Message { get; set; } = string.Empty;

		/// <summary>
		/// Creates a successful result with an optional message.
		/// </summary>
		/// <param name="message">The success message.</param>
		/// <returns>A successful <see cref="Result"/> instance.</returns>
		public static Result Ok(string message)
		{
			return new Result
			{
				Success = true,
				Message = message ?? string.Empty
			};
		}

		/// <summary>
		/// Creates a failed result with a message.
		/// </summary>
		/// <param name="message">The failure message.</param>
		/// <returns>A failed <see cref="Result"/> instance.</returns>
		public static Result Fail(string message)
		{
			return new Result
			{
				Success = false,
				Message = message ?? string.Empty
			};
		}

		/// <summary>
		/// Creates a successful result with an empty message.
		/// </summary>
		/// <returns>A successful <see cref="Result"/> instance.</returns>
		public static Result Ok()
		{
			return Ok(string.Empty);
		}
	}
}