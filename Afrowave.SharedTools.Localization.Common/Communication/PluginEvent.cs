using Afrowave.SharedTools.Localization.Common.Models.Enums;
using System;
using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Common.Communication
{
	/// <summary>
	/// Represents an event for the plugin communication system.
	/// </summary>
	public class PluginEvent
	{
		/// <summary>
		/// identifies the request uniquely. This is useful for tracking and correlating events.
		/// </summary>
		public string RequestId { get; set; } = Guid.NewGuid().ToString();

		/// <summary>
		/// Gets or sets the list of target identifiers.
		/// </summary>
		public List<string> TargetIds { get; set; } = new List<string>();

		/// <summary>
		/// Gets or sets the collection of target plugin types.
		/// </summary>
		public List<PluginType> TargetTypes { get; set; } = new List<PluginType>();

		/// <summary>
		/// Gets or sets the type of the event.
		/// </summary>
		public string EventType { get; set; } = string.Empty;

		/// <summary>
		/// Gets or sets the payload associated with the current operation.
		/// </summary>
		public object? Payload { get; set; }

		/// <summary>
		/// Gets or sets the timestamp indicating the date and time of the event.
		/// </summary>
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}