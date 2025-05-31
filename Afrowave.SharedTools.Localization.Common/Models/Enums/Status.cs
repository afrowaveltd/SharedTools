/// <summary>
/// Represents the current runtime state of a plugin.
/// </summary>
public enum Status
{
	/// <summary>Plugin is fully initialized and ready.</summary>
	Ready = 0,

	/// <summary>Plugin is disabled and should not be used.</summary>
	Disabled = 1,

	/// <summary>Plugin is in error state and cannot operate normally.</summary>
	Error = 2,

	/// <summary>Plugin is operating in degraded mode (partial functionality).</summary>
	Degraded = 3,

	/// <summary>Plugin is initializing or performing startup handshake.</summary>
	Initializing = 4,

	/// <summary>Plugin is paused – temporarily not active.</summary>
	Paused = 5
}