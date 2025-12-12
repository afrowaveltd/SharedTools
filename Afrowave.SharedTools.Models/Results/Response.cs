using System;

namespace Afrowave.SharedTools.Models.Results
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
      /// If <see cref="HasData"/> is false, the value should be treated as "not provided".
      /// </summary>
      public T Data { get; set; } = default!;

      /// <summary>
      /// True if <see cref="Data"/> was explicitly provided.
      /// Helps distinguish "no data" from default(T) being a valid value.
      /// </summary>
      public bool HasData { get; set; } = false;

      // ---------------------------------------------------------------------
      // Backward-compatible factories (kept as-is, but improved internally)
      // ---------------------------------------------------------------------

      /// <summary>
      /// Creates a successful response with data and a message.
      /// </summary>
      public static Response<T> SuccessResponse(T data, string message)
          => new Response<T>
          {
             Success = true,
             Warning = false,
             Data = data,
             HasData = true,
             Message = message ?? string.Empty
          };

      /// <summary>
      /// Creates a successful response with a warning, data, and a warning message.
      /// </summary>
      public static Response<T> SuccessWithWarning(T data, string warningMessage)
          => new Response<T>
          {
             Success = true,
             Warning = true,
             Data = data,
             HasData = true,
             Message = warningMessage ?? string.Empty
          };

      /// <summary>
      /// Creates a failed response with a message.
      /// </summary>
      public static Response<T> Fail(string message)
          => new Response<T>
          {
             Success = false,
             Warning = false,
             Data = default!,
             HasData = false,
             Message = message ?? string.Empty
          };

      /// <summary>
      /// Creates a failed response from an exception.
      /// </summary>
      public static Response<T> Fail(Exception ex)
          => new Response<T>
          {
             Success = false,
             Warning = false,
             Data = default!,
             HasData = false,
             Message = ex?.Message ?? "Unknown error"
          };

      /// <summary>
      /// Creates a successful response with no data and no message.
      /// </summary>
      public static Response<T> EmptySuccess()
          => new Response<T>
          {
             Success = true,
             Warning = false,
             Data = default!,
             HasData = false,
             Message = string.Empty
          };

      // ---------------------------------------------------------------------
      // Result-like helpers (unified syntax with Result: Ok/Fail)
      // ---------------------------------------------------------------------

      /// <summary>
      /// Creates a successful response without data.
      /// </summary>
      public static Response<T> Ok() => EmptySuccess();

      /// <summary>
      /// Creates a successful response without data, but with a message.
      /// </summary>
      public static Response<T> Ok(string message)
          => new Response<T>
          {
             Success = true,
             Warning = false,
             Data = default!,
             HasData = false,
             Message = message ?? string.Empty
          };

      /// <summary>
      /// Creates a successful response with data.
      /// </summary>
      public static Response<T> Ok(T data)
          => new Response<T>
          {
             Success = true,
             Warning = false,
             Data = data,
             HasData = true,
             Message = string.Empty
          };

      /// <summary>
      /// Creates a successful response with data and a message.
      /// </summary>
      public static Response<T> Ok(T data, string message)
          => new Response<T>
          {
             Success = true,
             Warning = false,
             Data = data,
             HasData = true,
             Message = message ?? string.Empty
          };

      /// <summary>
      /// Creates a successful response with a warning message (no data).
      /// Note: cannot be named "Warning" because of the existing <see cref="Warning"/> property.
      /// </summary>
      public static Response<T> OkWithWarning(string warningMessage)
          => new Response<T>
          {
             Success = true,
             Warning = true,
             Data = default!,
             HasData = false,
             Message = warningMessage ?? string.Empty
          };

      /// <summary>
      /// Creates a successful response with data and a warning message.
      /// </summary>
      public static Response<T> OkWithWarning(T data, string warningMessage)
          => new Response<T>
          {
             Success = true,
             Warning = true,
             Data = data,
             HasData = true,
             Message = warningMessage ?? string.Empty
          };

      /// <summary>
      /// Creates a failed response without a message.
      /// </summary>
      public static Response<T> Fail()
          => new Response<T>
          {
             Success = false,
             Warning = false,
             Data = default!,
             HasData = false,
             Message = string.Empty
          };
   }

   /// <summary>
   /// Represents a non-generic response containing success flag and message only.
   /// Use this when an operation does not need to return data.
   /// </summary>
   public class Response
   {
      public bool Success { get; set; }
      public string Message { get; set; } = string.Empty;

      public static Response Ok() => new Response { Success = true };

      public static Response Ok(string message) => new Response { Success = true, Message = message ?? string.Empty };

      public static Response Fail() => new Response { Success = false };

      public static Response Fail(string message) => new Response { Success = false, Message = message ?? string.Empty };
   }
}