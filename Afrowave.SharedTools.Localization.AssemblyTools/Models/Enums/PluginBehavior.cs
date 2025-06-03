using System;

namespace Afrowave.SharedTools.Localization.AssemblyTools.Models.Enums
{
	/// <summary>
	/// Defines the possible capabilities, behaviors and roles of plugins in the Afrowave.Localization system.
	/// Flags can be combined to describe complex roles and abilities.
	/// </summary>
	[Flags]
	public enum PluginBehavior
	{
		/// <summary>
		/// Represents the absence of a value or state.
		/// </summary>
		None = 0,

		// --- Common capabilities ---

		/// <summary>
		/// Indicates whether the object supports reading operations.
		/// </summary>
		CanRead = 1 << 0,

		/// <summary>
		/// Indicates whether the current object supports write operations.
		/// </summary>
		CanWrite = 1 << 1,

		/// <summary>
		/// Represents a flag indicating whether an entity can be updated.
		/// </summary>
		CanUpdate = 1 << 2,

		/// <summary>
		/// Represents the permission to delete items.
		/// </summary>
		/// <remarks>This flag can be used to indicate whether a user or entity has the ability to delete items. It is
		/// typically combined with other permission flags using bitwise operations.</remarks>
		CanDelete = 1 << 3,

		/// <summary>
		/// Indicates whether the data can be seeded.
		/// </summary>
		/// <remarks>This flag is typically used to determine if the current context or configuration supports seeding
		/// bulk of data into a database or other storage mechanism.</remarks>
		CanSeedData = 1 << 4,

		/// <summary>
		/// Indicates whether the entity has the ability to generate reports.
		/// </summary>
		/// <remarks>This flag can be used to determine if reporting functionality is enabled for the
		/// entity.</remarks>
		CanReport = 1 << 5,

		/// <summary>
		/// Indicates whether the system can automatically correct the associated value.
		/// </summary>
		/// <remarks>This flag is typically used to determine if automatic correction mechanisms, such as spell
		/// checking or formatting adjustments, are enabled for the associated operation or context.</remarks>
		CanAutoCorrect = 1 << 6,

		/// <summary>
		/// Represents the ability to moderate content or manage user interactions.
		/// </summary>
		/// <remarks>This flag is typically used to indicate whether a user or role has moderation privileges, such as
		/// the ability to delete translations, ban words, or manage community localization settings.</remarks>
		CanModerate = 1 << 7,

		/// <summary>
		/// Indicates whether the entity has the ability to schedule tasks or events.
		/// </summary>
		/// <remarks>This flag can be used to determine if scheduling functionality is enabled for the entity. It is
		/// typically combined with other flags in a bitwise operation.</remarks>
		CanSchedule = 1 << 8,

		/// <summary>
		/// Indicates whether the operation can be processed in batches.
		/// </summary>
		/// <remarks>This flag is typically used to determine if a process supports batch operations, allowing
		/// multiple items to be handled together for improved efficiency.</remarks>
		CanBatchProcess = 1 << 9,

		/// <summary>
		/// Indicates whether the entity has the ability to observe or monitor changes.
		/// </summary>
		/// <remarks>This flag is typically used to determine if an entity can track or respond to observable
		/// events.</remarks>
		CanObserve = 1 << 10,

		/// <summary>
		/// Indicates whether the entity can send notifications.
		/// </summary>
		/// <remarks>This flag is typically used to determine if an entity has the capability to send
		/// notifications.</remarks>
		CanNotify = 1 << 11,

		/// <summary>
		/// Specifies wherever the entity can require user input or interaction.
		/// </summary>
		SupportsHumanReview = 1 << 12,

		/// <summary>
		/// Specifies whether the entity can use internal caching mechanisms for performance optimization.
		/// </summary>
		CanUseInternalCache = 1 << 13,

		/// <summary>
		/// Indicates whether the cache plugin can be used.
		/// </summary>
		/// <remarks>This flag is typically used to determine if caching functionality is enabled or supported within
		/// the current context. It can be combined with other flags using bitwise operations.</remarks>
		CanUseCachePlugin = 1 << 14,

		// --- Transport/communication ---

		/// <summary>
		/// Defines whether the plugin can communicate with other systems or services using events.
		/// </summary>
		SupportsEvents = 1 << 15,

		/// <summary>
		/// Indicates whether the application supports SignalR functionality.
		/// </summary>
		/// <remarks>This flag can be used to determine if plugin supports SignalR features. SignalR is
		/// a library for real-time web functionality, such as live updates or notifications.</remarks>
		SupportsSignalR = 1 << 16,

		/// <summary>
		/// Indicates whether the plugin can be called remotely.
		/// </summary>
		/// <remarks>This flag can be used to determine if a feature or operation is capable of being executed
		/// remotely.</remarks>
		SupportsRemote = 1 << 17,

		// --- Diagnostics, admin, tracing ---
		/// <summary>
		/// Indicates whether the system supports tracking functionality.
		/// </summary>
		/// <remarks>This flag can be used to determine if tracking features, such as diagnostics or tracing,  are
		/// available in the current configuration.</remarks>
		SupportsTracking = 1 << 18,

		/// <summary>
		/// Indicates whether the system supports analytics functionality.
		/// </summary>
		/// <remarks>This flag can be used to determine if analytics features are available in the current
		/// environment.</remarks>
		SupportsAnalytics = 1 << 19,

		/// <summary>
		/// Indicates whether the application supports administrative tools.
		/// </summary>
		/// <remarks>This flag can be used to determine if administrative tools are available in the application. It
		/// is typically used in scenarios where elevated permissions or administrative features are required.</remarks>
		SupportsAdminTools = 1 << 20, // Supports admin/maintenance ops
	}
}