namespace Afrowave.SharedTools.Localization.Common.Models.Backend
{
	public class Behavior
	{
		public bool UsesKeyAsDefaultValue { get; set; } = false;
		public bool HasInternalCache { get; set; } = false;
		public bool SupportsCacheMisses { get; set; } = false;
		public bool SupportsManualCacheCleaner { get; set; } = false;
		public bool CanDetectExternalChanges { get; set; } = false;
		public bool SupportsAutoReload { get; set; } = false;
		public bool SupportsLogging { get; set; } = false;
		public int ExpectedResponseTimeMs { get; set; } = 500;
	}
}