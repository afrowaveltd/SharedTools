using System;

namespace Afrowave.SharedTools.Localization.Common.Models.Enums
{
	[Flags]
	public enum PluginType
	{
		None = 0,
		Backend = 1,
		Translator = 2,
		Frontend = 4,
		Tool = 8,
		Cache = 16,
		Diagnostics = 32,
		Logger = 64
	}
}