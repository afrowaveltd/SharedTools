using System;
using System.Collections.Generic;

namespace Afrowave.SharedTools.Models.Communication
{
   /// <summary>
   /// Non-generic request envelope.
   /// Use when you only need routing/action + metadata, without a typed payload.
   /// </summary>
   public class Request
   {
      /// <summary>
      /// Correlation ID for tracing a request across systems.
      /// </summary>
      public Guid Id { get; set; } = Guid.NewGuid();

      /// <summary>
      /// When the request was created (UTC).
      /// </summary>
      public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;

      /// <summary>
      /// Action/operation identifier (e.g. "localization.resolve", "docs.translate").
      /// Keep this stable; versioning can be included in the string if needed.
      /// </summary>
      public string Action { get; set; } = string.Empty;

      /// <summary>
      /// Optional sender identifier (service/plugin/instance name).
      /// </summary>
      public string? Sender { get; set; }

      /// <summary>
      /// Optional target identifier (service/plugin/instance name).
      /// </summary>
      public string? Target { get; set; }

      /// <summary>
      /// Optional metadata headers (trace id, locale, feature flags, etc.).
      /// </summary>
      public Dictionary<string, string>? Meta { get; set; }

      // ---------------------------------------------------------------------
      // Factories (unified style)
      // ---------------------------------------------------------------------

      /// <summary>
      /// Creates a <see cref="Request"/> with the specified action.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <returns>A new <see cref="Request"/> instance.</returns>
      public static Request Create(string action)
          => new Request { Action = action ?? string.Empty };

      /// <summary>
      /// Creates a <see cref="Request"/> with the specified action and sender/target identifiers.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <param name="sender">Optional sender identifier.</param>
      /// <param name="target">Optional target identifier.</param>
      /// <returns>A new <see cref="Request"/> instance.</returns>
      public static Request Create(string action, string? sender, string? target = null)
          => new Request { Action = action ?? string.Empty, Sender = sender, Target = target };

      /// <summary>
      /// Creates a <see cref="Request"/> with the specified action, metadata, and sender/target identifiers.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <param name="meta">Optional metadata headers.</param>
      /// <param name="sender">Optional sender identifier.</param>
      /// <param name="target">Optional target identifier.</param>
      /// <returns>A new <see cref="Request"/> instance.</returns>
      public static Request Create(string action, Dictionary<string, string>? meta, string? sender = null, string? target = null)
          => new Request { Action = action ?? string.Empty, Meta = meta, Sender = sender, Target = target };
   }

   /// <summary>
   /// Generic request envelope with typed body.
   /// </summary>
   /// <typeparam name="T">Type of the request payload.</typeparam>
   public class Request<T> : Request
   {
      /// <summary>
      /// Request payload.
      /// If <see cref="HasBody"/> is false, the value should be treated as "not provided".
      /// </summary>
      public T Body { get; set; } = default!;

      /// <summary>
      /// True if <see cref="Body"/> was explicitly provided.
      /// Helps distinguish "no body" from <c>default(T)</c> being a valid value.
      /// </summary>
      public bool HasBody { get; set; } = false;

      // ---------------------------------------------------------------------
      // Factories (unified style)
      // ---------------------------------------------------------------------

      /// <summary>
      /// Creates a typed <see cref="Request{T}"/> without a body.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <returns>A new <see cref="Request{T}"/> instance.</returns>
      public new static Request<T> Create(string action)
          => new Request<T> { Action = action ?? string.Empty, HasBody = false, Body = default! };

      /// <summary>
      /// Creates a typed <see cref="Request{T}"/> with a body.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <param name="body">Typed payload.</param>
      /// <returns>A new <see cref="Request{T}"/> instance.</returns>
      public static Request<T> Create(string action, T body)
          => new Request<T> { Action = action ?? string.Empty, HasBody = true, Body = body };

      /// <summary>
      /// Creates a typed <see cref="Request{T}"/> with a body and sender/target identifiers.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <param name="body">Typed payload.</param>
      /// <param name="sender">Optional sender identifier.</param>
      /// <param name="target">Optional target identifier.</param>
      /// <returns>A new <see cref="Request{T}"/> instance.</returns>
      public static Request<T> Create(string action, T body, string? sender, string? target = null)
          => new Request<T> { Action = action ?? string.Empty, Sender = sender, Target = target, HasBody = true, Body = body };

      /// <summary>
      /// Creates a typed <see cref="Request{T}"/> with a body, metadata, and sender/target identifiers.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <param name="body">Typed payload.</param>
      /// <param name="meta">Optional metadata headers.</param>
      /// <param name="sender">Optional sender identifier.</param>
      /// <param name="target">Optional target identifier.</param>
      /// <returns>A new <see cref="Request{T}"/> instance.</returns>
      public static Request<T> Create(string action, T body, Dictionary<string, string>? meta, string? sender = null, string? target = null)
          => new Request<T> { Action = action ?? string.Empty, Meta = meta, Sender = sender, Target = target, HasBody = true, Body = body };

      /// <summary>
      /// Creates a typed <see cref="Request{T}"/> without a body but with optional sender/target identifiers.
      /// </summary>
      /// <param name="action">Operation identifier.</param>
      /// <param name="sender">Optional sender identifier.</param>
      /// <param name="target">Optional target identifier.</param>
      /// <returns>A new <see cref="Request{T}"/> instance.</returns>
      public static Request<T> CreateNoBody(string action, string? sender = null, string? target = null)
          => new Request<T> { Action = action ?? string.Empty, Sender = sender, Target = target, HasBody = false, Body = default! };
   }
}