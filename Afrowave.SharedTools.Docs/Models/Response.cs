﻿namespace Afrowave.SharedTools.Docs.Models
{
	/// <summary>
	/// Represents a standardized response that contains data and metadata about the operation.
	/// </summary>
	/// <typeparam name="T">The type of the returned data.</typeparam>
	public class Response<T>
	{
		/// <summary>
		/// Gets or sets a value indicating whether the operation was successful.
		/// </summary>
		public bool Success { get; set; } = true;

		/// <summary>
		/// Gets or sets a value indicating whether the response contains a warning.
		/// </summary>
		public bool Warning { get; set; } = false;

		/// <summary>
		/// Gets or sets the message associated with the response.
		/// </summary>
		public string Message { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the data returned by the operation.
		/// </summary>
		public T? Data { get; set; } = default;

		/// <summary>
		/// Creates a successful response with data and a message.
		/// </summary>
		/// <param name="data">The data to return.</param>
		/// <param name="message">The success message.</param>
		/// <returns>A successful <see cref="Response{T}"/> instance.</returns>
		public static Response<T> Successful(T data, string message)
		{
			return new Response<T>
			{
				Success = true,
				Data = data,
				Message = message ?? string.Empty
			};
		}

		/// <summary>
		/// Creates a failed response with a message.
		/// </summary>
		/// <param name="message">The failure message.</param>
		/// <returns>A failed <see cref="Response{T}"/> instance.</returns>
		public static Response<T> Fail(string message)
		{
			return new Response<T>
			{
				Success = false,
				Message = message ?? string.Empty
			};
		}

		/// <summary>
		/// Creates a successful response with no data and no message.
		/// </summary>
		/// <returns>A successful <see cref="Response{T}"/> instance.</returns>
		public static Response<T> SuccessEmty()
		{
			return new Response<T>
			{
				Success = true
			};
		}

		/// <summary>
		/// Creates a successful response with a warning, data, and a warning message.
		/// </summary>
		/// <param name="data">The data to return.</param>
		/// <param name="warningMessage">The warning message.</param>
		/// <returns>A successful <see cref="Response{T}"/> instance with a warning.</returns>
		public static Response<T> SuccessWithWarning(T data, string warningMessage)
		{
			return new Response<T>
			{
				Success = true,
				Warning = true,
				Data = data,
				Message = warningMessage ?? string.Empty
			};
		}

		/// <summary>
		/// Creates a failed response from an exception.
		/// </summary>
		/// <param name="ex">The exception that caused the failure.</param>
		/// <returns>A failed <see cref="Response{T}"/> instance.</returns>
		public static Response<T> Fail(Exception ex)
		{
			return new Response<T>
			{
				Success = false,
				Message = ex?.Message ?? "Unknown error"
			};
		}
	}
}