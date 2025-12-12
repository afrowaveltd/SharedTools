using System;

namespace Afrowave.SharedTools.Models.Results
{
   /// <summary>
   /// Represents a boolean result of an operation without data.
   /// Unified with Response / Response&lt;T&gt; syntax (Ok/Fail/OkWithWarning).
   /// </summary>
   public class Result
   {
      /// <summary>
      /// Gets or sets a value indicating whether the operation was successful.
      /// </summary>
      public bool Success { get; set; } = true;

      /// <summary>
      /// Gets or sets a value indicating whether the operation succeeded with a warning.
      /// </summary>
      public bool Warning { get; set; } = false;

      /// <summary>
      /// Gets or sets the message associated with the result.
      /// </summary>
      public string Message { get; set; } = string.Empty;

      // ---------------------------------------------------------------------
      // Unified factories
      // ---------------------------------------------------------------------

      /// <summary>
      /// Creates a successful result with an empty message.
      /// </summary>
      public static Result Ok() => new Result { Success = true, Warning = false, Message = string.Empty };

      /// <summary>
      /// Creates a successful result with a message.
      /// </summary>
      public static Result Ok(string message)
          => new Result { Success = true, Warning = false, Message = message ?? string.Empty };

      /// <summary>
      /// Creates a successful result with a warning message.
      /// </summary>
      public static Result OkWithWarning(string warningMessage)
          => new Result { Success = true, Warning = true, Message = warningMessage ?? string.Empty };

      /// <summary>
      /// Creates a failed result with an empty message.
      /// </summary>
      public static Result Fail() => new Result { Success = false, Warning = false, Message = string.Empty };

      /// <summary>
      /// Creates a failed result with a message.
      /// </summary>
      public static Result Fail(string message)
          => new Result { Success = false, Warning = false, Message = message ?? string.Empty };

      /// <summary>
      /// Creates a failed result from an exception.
      /// </summary>
      public static Result Fail(Exception ex)
          => new Result { Success = false, Warning = false, Message = ex?.Message ?? "Unknown error" };
   }
}