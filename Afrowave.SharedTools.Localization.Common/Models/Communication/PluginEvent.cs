using Afrowave.SharedTools.Localization.Common.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Afrowave.SharedTools.Localization.Common.Models.Communication
{
	public class PluginEvent
	{
		public string RequestId { get; set; } = Guid.NewGuid().ToString();
		public List<string> TargetIds { get; set; } = new List<string>();
		public List<PluginType> TargetTypes { get; set; } = new List<PluginType>();
		public string EventType { get; set; } = string.Empty;
		public object? Payload { get; set; }
		public DateTime Timestamp { get; set; } = DateTime.UtcNow;
	}
}
