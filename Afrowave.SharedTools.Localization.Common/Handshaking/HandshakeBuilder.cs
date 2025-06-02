using Afrowave.SharedTools.Localization.Common.Logging;
using Afrowave.SharedTools.Localization.Common.Models;
using Afrowave.SharedTools.Localization.Common.Options;
using System;
using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Common.Handshaking
{
	/// <summary>
	/// Provides a fluent interface for configuring and building a <see
	/// cref="Afrowave.SharedTools.Localization.Common.Models.Handshake"/> object.
	/// </summary>
	/// <remarks>The <see cref="HandshakeBuilder"/> class allows developers to specify options and features for a
	/// handshake operation by chaining method calls. It supports enabling or excluding specific features, adding custom
	/// options, and configuring logging and SignalR settings. Once configured, the <see cref="Build"/> method generates
	/// the final <see cref="Afrowave.SharedTools.Localization.Common.Models.Handshake"/> object.</remarks>
	public sealed class HandshakeBuilder : IHandshakeOptionsConfigurator
	{
		private readonly Afrowave.SharedTools.Localization.Common.Models.Handshake _handshake = new Afrowave.SharedTools.Localization.Common.Models.Handshake();
		private readonly HashSet<string> _excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly List<PluginScopeExclusion> _exclusionsOnce = new List<PluginScopeExclusion>();
		private ILogSink? _logger;
		private LoggerVerbosity _verbosity = LoggerVerbosity.Info;

		/// <summary>
		/// Enables the read operation for the handshake and returns the updated builder.
		/// </summary>
		/// <remarks>This method sets the <see cref="Handshake.UseRead"/> property to <see langword="true"/>. Use this
		/// method to configure the handshake for read operations.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseRead()
		{ _handshake.UseRead = true; return this; }

		/// <summary>
		/// Enables the write operation for the handshake and returns the current builder instance.
		/// </summary>
		/// <remarks>This method sets the <see cref="Handshake.UseWrite"/> property to <see langword="true"/>. Use
		/// this method to configure the handshake for write operations.</remarks>
		/// <returns>The current <see cref="HandshakeBuilder"/> instance, allowing for method chaining.</returns>
		public HandshakeBuilder UseWrite()
		{ _handshake.UseWrite = true; return this; }

		/// <summary>
		/// Enables the update mode for the handshake and returns the current builder instance.
		/// </summary>
		/// <remarks>Setting the update mode allows the handshake to be modified during its lifecycle. This method is
		/// typically used in scenarios where dynamic updates to the handshake are required.</remarks>
		/// <returns>The current <see cref="HandshakeBuilder"/> instance, allowing for method chaining.</returns>
		public HandshakeBuilder UseUpdate()
		{ _handshake.UseUpdate = true; return this; }

		/// <summary>
		/// Enables the DELETE method for the handshake operation.
		/// </summary>
		/// <remarks>This method sets the handshake to use the DELETE HTTP method and returns the current instance of
		/// <see cref="HandshakeBuilder"/> to allow method chaining.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/> with the DELETE method enabled.</returns>
		public HandshakeBuilder UseDelete()
		{ _handshake.UseDelete = true; return this; }

		/// <summary>
		/// Enables the Bulk Read operation for the handshake and returns the current builder instance.
		/// </summary>
		/// <remarks>This method sets the <see cref="Handshake.UseBulkRead"/> property to <see langword="true"/>. Use this
		/// method to configure the handshake for bulk read operations.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseBulkRead()
		{ _handshake.UseBulkRead = true; return this; }

		/// <summary>
		/// Enables the Bulk Write operation for the handshake and returns the current builder instance.
		/// </summary>
		/// <remarks>This method sets the <see cref="Handshake.UseBulkWrite"/> property to <see langword="true"/>. Use this
		/// method to configure the handshake for bulk write operations.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseBulkWrite()
		{ _handshake.UseBulkWrite = true; return this; }

		/// <summary>
		/// Enables the Bulk Write operation for the handshake and returns the current builder instance.
		/// </summary>
		/// <remarks>This method sets the <see cref="Handshake.UseBulkWrite"/> property to <see langword="true"/>. Use this
		/// method to configure the handshake for bulk write operations.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseListLanguages()
		{ _handshake.UseListLanguages = true; return this; }

		/// <summary>
		/// Enables cache usage for the handshake operation.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseCache()
		{ _handshake.UseCache = true; return this; }

		/// <summary>
		/// Enables stream usage for the handshake operation.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseStream()
		{ _handshake.UseStream = true; return this; }

		/// <summary>
		/// Enables SignalR support for the handshake process and optionally configures SignalR options.
		/// </summary>
		/// <remarks>This method sets up SignalR integration for the handshake process. If no configuration delegate
		/// is  provided, default SignalR options will be used.</remarks>
		/// <param name="configure">An optional delegate to configure <see cref="SignalROptions"/>. If provided, the delegate is invoked with the
		/// SignalR options instance to allow customization.</param>
		/// <returns>A <see cref="HandshakeBuilder"/> instance, allowing further configuration of the handshake process.</returns>
		public HandshakeBuilder UseSignalR(Action<SignalROptions> configure = null)
		{
			_handshake.UseSignalR = true;
			configure?.Invoke(_handshake.GetOrCreateOption<SignalROptions>());
			return this;
		}

		/// <summary>
		/// Enables SignalR support for the handshake process and optionally configures SignalR options.
		/// </summary>
		/// <returns>A <see cref="HandshakeBuilder"/> instance, allowing further configuration of the handshake process.</returns>
		public HandshakeBuilder UseEvents()
		{ _handshake.UseEvents = true; return this; }

		/// <summary>
		/// Enables logging support for the handshake operation.
		/// </summary>
		/// <returns>A <see cref="HandshakeBuilder"/> instance, allowing further configuration of the handshake process.</returns>
		public HandshakeBuilder UseLogging()
		{ _handshake.UseLogging = true; return this; }

		/// <summary>
		/// Excludes the read operation from the handshake configuration.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to exclude the read operation,  preventing it from
		/// being used in the handshake process.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeRead()
		{ _excluded.Add(nameof(_handshake.UseRead)); return this; }

		/// <summary>
		/// Excludes the write operation from the handshake configuration
		/// </summary>
		/// <remarks>This method modifies the handshake builder to exclude the write operation,  preventing it from
		/// being used in the handshake process.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeWrite()
		{ _excluded.Add(nameof(_handshake.UseWrite)); return this; }

		/// <summary>
		/// Excludes the update functionality from the handshake process.
		/// </summary>
		/// <remarks>This method prevents the handshake from using the update functionality by marking it as excluded.
		/// Subsequent calls to the handshake builder will reflect this exclusion.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeUpdate()
		{ _excluded.Add(nameof(_handshake.UseUpdate)); return this; }

		/// <summary>
		/// Excludes the update functionality from the handshake process.
		/// </summary>
		/// <remarks>This method prevents the handshake from using the update functionality by marking it as excluded.
		/// Subsequent calls to the handshake builder will reflect this exclusion.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeDelete()
		{ _excluded.Add(nameof(_handshake.UseDelete)); return this; }

		/// <summary>
		/// Excludes the bulk read functionality from the handshake process.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeBulkRead()
		{ _excluded.Add(nameof(_handshake.UseBulkRead)); return this; }

		/// <summary>
		/// Excludes the bulk write operation from the handshake configuration.
		/// </summary>
		/// <remarks>This method modifies the handshake configuration to exclude the bulk write operation. It is
		/// typically used when bulk write functionality is not desired or supported.</remarks>
		/// <returns>The current <see cref="HandshakeBuilder"/> instance, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeBulkWrite()
		{ _excluded.Add(nameof(_handshake.UseBulkWrite)); return this; }

		/// <summary>
		/// Excludes the bulk write operation from the handshake configuration.
		/// </summary>
		/// <remarks>This method modifies the handshake configuration to exclude the bulk write operation. It is
		/// typically used when bulk write functionality is not desired or supported.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeListLanguages()
		{ _excluded.Add(nameof(_handshake.UseListLanguages)); return this; }

		/// <summary>
		/// Excludes the cache usage from the handshake configuration.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeCache()
		{ _excluded.Add(nameof(_handshake.UseCache)); return this; }

		/// <summary>
		/// Excludes the stream usage from the handshake configuration.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeStream()
		{ _excluded.Add(nameof(_handshake.UseStream)); return this; }

		/// <summary>
		/// Excludes the stream usage from the handshake configuration.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeSignalR()
		{ _excluded.Add(nameof(_handshake.UseSignalR)); return this; }

		/// <summary>
		/// Excludes the events usage from the handshake configuration.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeEvents()
		{ _excluded.Add(nameof(_handshake.UseEvents)); return this; }

		/// <summary>
		/// Excludes the logging support from the handshake configuration.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder ExcludeLogging()
		{ _excluded.Add(nameof(_handshake.UseLogging)); return this; }

		/// <summary>
		/// Excludes the logging support from the handshake configuration.
		/// </summary>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseAll()
		{
			return UseRead().UseWrite().UseUpdate().UseDelete()
				 .UseBulkRead().UseBulkWrite()
				 .UseListLanguages().UseCache().UseStream()
				 .UseSignalR().UseEvents().UseLogging();
		}

		/// <summary>
		/// Enables all functionalities for the handshake process.
		/// </summary>
		/// <param name="config"></param>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder UseAll(Action<IHandshakeOptionsConfigurator> config)
		{
			UseAll();
			config?.Invoke(this);
			return this;
		}

		/// <summary>
		/// Configures the handshake builder with default settings, including read, cache, and logging functionality.
		/// </summary>
		/// <remarks>This method applies a predefined set of configurations to the handshake builder, enabling read
		/// operations,  caching, and logging. It is a convenient way to initialize the builder with commonly used
		/// defaults.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with default configurations applied.</returns>
		public HandshakeBuilder UseDefaults()
		{
			return UseRead().UseCache().UseLogging();
		}

		/// <summary>
		/// Marks this plugin as the default for its type.
		/// </summary>
		public HandshakeBuilder UseAsDefault()
		{
			_handshake.UseAsDefault = true;
			return this;
		}

		/// <summary>
		/// Marks this plugin as a fallback if other plugins fail.
		/// </summary>
		public HandshakeBuilder UseAsFallback()
		{
			_handshake.UseAsFallback = true;
			return this;
		}

		/// <summary>
		/// Configures the handshake builder with default settings, including read, cache, and logging functionality.
		/// </summary>
		/// <param name="pluginId">Plugin Id</param>
		/// <param name="key">Optional key for the exclusion</param>
		/// <param name="reason">Reason for the exclusion</param>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with default configurations applied.</returns>

		public HandshakeBuilder ExcludeOnce(string pluginId, string key = null, string reason = null)
		{
			_exclusionsOnce.Add(new PluginScopeExclusion
			{
				PluginId = pluginId,
				ForKey = key,
				Reason = reason
			});
			return this;
		}

		/// <summary>
		/// Adds the specified plugin option to the handshake configuration.
		/// </summary>
		/// <typeparam name="T">The type of the plugin option, which must derive from <see cref="PluginOptionSet"/>.</typeparam>
		/// <param name="option">The plugin option to add. Must not be <see langword="null"/>.</param>
		/// <returns>The current <see cref="HandshakeBuilder"/> instance, allowing for method chaining.</returns>
		public HandshakeBuilder AddOption<T>(T option) where T : PluginOptionSet
		{
			if(option != null)
			{
				_handshake.Options[option.Key] = option;
			}
			return this;
		}

		/// <summary>
		/// Configures the handshake builder to use the specified logger and verbosity level.
		/// </summary>
		/// <param name="logger">The logging sink to which log messages will be written. Cannot be null.</param>
		/// <param name="verbosity">The verbosity level for log messages. Defaults to <see cref="LoggerVerbosity.Info"/>.</param>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		public HandshakeBuilder WithLogger(ILogSink logger, LoggerVerbosity verbosity = LoggerVerbosity.Info)
		{
			_logger = logger;
			_verbosity = verbosity;
			return this;
		}

		/// <summary>
		/// Builds and returns a <see cref="Afrowave.SharedTools.Localization.Common.Models.Handshake"/> object  with
		/// modifications applied based on excluded properties.
		/// </summary>
		/// <remarks>This method modifies the properties of the <see
		/// cref="Afrowave.SharedTools.Localization.Common.Models.Handshake"/>  instance by setting excluded boolean
		/// properties to <see langword="false"/>. Properties are excluded based on  the configuration provided prior to
		/// calling this method.</remarks>
		/// <returns>A <see cref="Afrowave.SharedTools.Localization.Common.Models.Handshake"/> object with the specified  exclusions
		/// applied.</returns>
		public Afrowave.SharedTools.Localization.Common.Models.Handshake Build()
		{
			// Apply exclusions
			foreach(var prop in _excluded)
			{
				var pi = typeof(Afrowave.SharedTools.Localization.Common.Models.Handshake).GetProperty(prop);
				if(pi != null && pi.CanWrite && pi.PropertyType == typeof(bool))
					pi.SetValue(_handshake, false);
			}

			return _handshake;
		}
	}
}