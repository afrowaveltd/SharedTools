using Afrowave.Localization.Common.Models.Plugin;
using Afrowave.SharedTools.Localization.AssemblyTools.Models.Plugin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Afrowave.SharedTools.Localization.AssemblyTools.Interfaces;

/// <summary>
/// Provides a safe and unified public interface for interacting with any plugin.
/// Dispatcher and tools may access plugin metadata, capabilities, and execute exposed commands.
/// </summary>
public interface IPluginApi
{
	/// <summary>
	/// Returns static plugin metadata.
	/// </summary>
	PluginManifest GetManifest();

	/// <summary>
	/// Returns declared runtime capabilities.
	/// </summary>
	PluginCapabilities GetCapabilities();

	/// <summary>
	/// Returns all supported commands.
	/// </summary>
	IReadOnlyList<PluginCommand> GetSupportedCommands();

	/// <summary>
	/// Returns current runtime status of the plugin.
	/// </summary>
	PluginRuntimeStatus GetStatus();

	/// <summary>
	/// Executes a named command synchronously with a validated non-null input.
	/// </summary>
	object ExecuteCommand(string name, object input);

	/// <summary>
	/// Executes a named command asynchronously with validated input and optional result callback.
	/// </summary>
	Task<object> ExecuteCommandAsync(string name, object input, Func<object, Task> callback);
}