using Afrowave.SharedTools.Localization.Common.Builders;
using Afrowave.SharedTools.Localization.Common.Models.Enums;
using Afrowave.SharedTools.Localization.Communication;
using Afrowave.SharedTools.Localization.Plugin.DataAccessor.JsonLocal.Options;

namespace Afrowave.SharedTools.Localization.Plugin.DataAccessor.JsonLocal.Manifest
{
	/// <summary>
	/// Provides the PluginManifest for the JSON Local plugin.
	/// </summary>
	public static class JsonLocalManifestProvider
	{
		/// <summary>
		/// Creates a new instance of a <see cref="PluginManifest"/> configured for handling JSON translation files  from a
		/// local project folder with automatic folder detection.
		/// </summary>
		/// <remarks>This method initializes a <see cref="PluginManifest"/> with predefined metadata, capabilities,
		/// and options  tailored for working with raw JSON data. The manifest includes support for automatic detection of
		/// external  changes and specifies the expected data format as raw JSON.</remarks>
		/// <returns>A fully configured <see cref="PluginManifest"/> instance representing a plugin for accessing JSON translation
		/// files from a local folder.</returns>
		public static PluginManifest Create()
		{
			return new PluginBuilder()
				 .WithType(PluginType.DataAccessor)
				 .WithMetadata(meta =>
				 {
					 meta.Name = "JSON Local";
					 meta.Description = "Provides raw JSON translation files from a local project folder, with automatic folder detection.";
					 meta.Version = "1.0.0";
					 meta.Author = "Afrowave Team";
					 meta.License = "MIT";
					 meta.Tags.Add("json");
					 meta.Tags.Add("file");
					 meta.Tags.Add("autodetect");
				 })
				 .WithCapabilities(cap =>
				 {
					 cap.ExpectedDataFormat = DataFormat.RawJson;
					 cap.CanDetectExternalChanges = true;
					 // další capabilities podle potřeby
				 })
				 .WithOption(new JsonLocalOptions())
				 .Build();
		}
	}
}