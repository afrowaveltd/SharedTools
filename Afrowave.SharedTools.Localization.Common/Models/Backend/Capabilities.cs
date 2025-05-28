namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	public class Capabilities
	{
		public bool CanRead { get; set; } = true;
		public bool CanWrite { get; set; } = false;
		public bool CanBulkRead { get; set; } = false;
		public bool CanBulkWrite { get; set; } = false;
		public bool CanDelete { get; set; } = false;
		public bool CanUpdate { get; set; } = false;
		public bool SupportsLanguageListing { get; set; } = false;
	}
}