using Afrowave.SharedTools.Localization.Common.Communication;
using Afrowave.SharedTools.Localization.Common.Models;
using Afrowave.SharedTools.Localization.Common.Models.Enums;
using Afrowave.SharedTools.Localization.Common.Options;
using Afrowave.SharedTools.Localization.Communication;
using System;

namespace Afrowave.SharedTools.Localization.Common.Builders
{
	/// <summary>
	/// Fluent builder for constructing PluginManifest instances.
	/// </summary>
	public sealed class PluginBuilder
	{
		private readonly PluginManifest _manifest = new PluginManifest();

		/// <summary>
		/// Sets the type of the plugin and returns the updated <see cref="PluginBuilder"/> instance.
		/// </summary>
		/// <param name="type">The type of the plugin to set. This value determines the plugin's behavior and classification.</param>
		/// <returns>The current <see cref="PluginBuilder"/> instance with the specified plugin type applied.</returns>
		public PluginBuilder WithType(PluginType type)
		{
			_manifest.Type = type;
			return this;
		}

		/// <summary>
		/// Sets the metadata for the plugin and returns the updated <see cref="PluginBuilder"/> instance.
		/// </summary>
		/// <param name="configure">An action to configure the metadata of the plugin.</param>
		/// <returns>The current <see cref="PluginBuilder"/> instance with the specified metadata applied.</returns>
		public PluginBuilder WithMetadata(Action<Metadata> configure)
		{
			configure?.Invoke(_manifest.Metadata);
			return this;
		}

		/// <summary>
		/// Sets the capabilities for the plugin and returns the updated <see cref="PluginBuilder"/> instance.
		/// </summary>
		/// <param name="configure">An action to configure the capabilities of the plugin.</param>
		/// <returns>The current <see cref="PluginBuilder"/> instance with the specified capabilities applied.</returns>
		public PluginBuilder WithCapabilities(Action<Capabilities> configure)
		{
			configure?.Invoke(_manifest.Capabilities);
			return this;
		}

		/// <summary>
		/// Configures the behavior of the plugin using the specified action.
		/// </summary>
		/// <param name="configure">An action that takes a <see cref="Behavior"/> object, allowing the caller to modify the plugin's behavior. The
		/// <paramref name="configure"/> parameter cannot be null.</param>
		/// <returns>The current <see cref="PluginBuilder"/> instance, enabling method chaining.</returns>
		public PluginBuilder WithBehavior(Action<Behavior> configure)
		{
			configure?.Invoke(_manifest.Behavior);
			return this;
		}

		/// <summary>
		/// Sets the status of the plugin and returns the updated <see cref="PluginBuilder"/> instance.
		/// </summary>
		/// <param name="status">The status to assign to the plugin. This value determines the current operational state of the plugin.</param>
		/// <returns>The updated <see cref="PluginBuilder"/> instance, allowing for method chaining.</returns>
		public PluginBuilder WithStatus(Status status)
		{
			_manifest.Status = status;
			return this;
		}

		/// <summary>
		/// Adds the specified option to the plugin configuration.
		/// </summary>
		/// <remarks>This method updates the plugin's configuration by associating the provided option with its key.
		/// If an option with the same key already exists, it will be replaced.</remarks>
		/// <typeparam name="T">The type of the option, which must derive from <see cref="PluginOptionSet"/>.</typeparam>
		/// <param name="option">The option to add to the plugin configuration. Cannot be <see langword="null"/>.</param>
		/// <returns>The current <see cref="PluginBuilder"/> instance, allowing for method chaining.</returns>
		public PluginBuilder WithOption<T>(T option) where T : PluginOptionSet
		{
			if(option != null)
			{
				_manifest.Options[option.Key] = option;
			}
			return this;
		}

		/// <summary>
		/// Configures the default handshake for the plugin using the specified action.
		/// </summary>
		/// <remarks>Use this method to modify the default handshake settings for the plugin. The provided <see
		/// cref="Handshake"/> object represents the default handshake configuration, which can be customized as
		/// needed.</remarks>
		/// <param name="configure">An action that takes a <see cref="Handshake"/> object, allowing customization of the default handshake. If
		/// <paramref name="configure"/> is <see langword="null"/>, no configuration is applied.</param>
		/// <returns>The current <see cref="PluginBuilder"/> instance, enabling method chaining.</returns>
		public PluginBuilder WithDefaultHandshake(Action<Handshake> configure)
		{
			configure?.Invoke(_manifest.DefaultHandshake);
			return this;
		}

		/// <summary>
		/// Constructs and returns the plugin manifest.
		/// </summary>
		/// <remarks>The returned manifest contains all the data that has been configured for the plugin. Ensure that
		/// the manifest is fully configured before calling this method.</remarks>
		/// <returns>A <see cref="PluginManifest"/> instance representing the current state of the plugin configuration.</returns>
		public PluginManifest Build()
		{
			return _manifest;
		}
	}
}