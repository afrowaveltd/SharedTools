using Microsoft.AspNetCore.Mvc.Rendering;

namespace Afrowave.SharedTools.Docs.Services;

/// <summary>
/// Provides methods for generating options lists, such as binary options or language selection options, for use in user
/// interfaces or other selection scenarios.
/// </summary>
/// <remarks>This service leverages localization, logging, and language services to generate options dynamically.
/// It supports both synchronous and asynchronous methods for retrieving options, with localization applied to ensure
/// the options are presented in the appropriate language.</remarks>
/// <param name="localizer"></param>
/// <param name="logger"></param>
/// <param name="libreTranslateService"></param>
/// <param name="languagesService"></param>

public class OptionsService(IStringLocalizer<OptionsService> localizer,
	ILogger<OptionsService> logger,
	ILibreTranslateService libreTranslateService,
	ILanguageService languagesService) : IOptionsService
{
	private readonly IStringLocalizer<OptionsService> _localizer = localizer;
	private readonly ILogger<OptionsService> _logger = logger;
	private readonly ILibreTranslateService _libreTranslateService = libreTranslateService;
	private readonly ILanguageService _languagesService = languagesService;

	private readonly string _cssFolderPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory
	[..AppDomain.CurrentDomain.BaseDirectory
	 .IndexOf("bin")], "wwwroot", "css");

	// Synchronous methods
	/// <summary>
	/// Generates a list of binary options represented as select list items.
	/// </summary>
	/// <remarks>The text for the options is localized using the provided localization resources.</remarks>
	/// <param name="selected">A boolean value indicating whether the options should be pre-selected.  If <see langword="true"/>, both options
	/// will be marked as selected; otherwise, neither will be selected.</param>
	/// <returns>A list of <see cref="SelectListItem"/> objects representing binary options ("Yes" and "No").</returns>
	public List<SelectListItem> BinaryOptions(bool selected = true)
	{
		return
			[
				new SelectListItem
				{
					Value = "true",
					Text = _localizer["Yes"],
					Selected = selected
				},
				new SelectListItem
				{
					Value = "false",
					Text = _localizer["No"],
					Selected = selected
				}
			];
	}

	/// <summary>
	/// Asynchronously retrieves a list of language options formatted as <see cref="SelectListItem"/> objects.
	/// </summary>
	/// <remarks>If the underlying language retrieval operation fails, an empty list is returned.</remarks>
	/// <param name="selected">The language code to mark as selected in the resulting list. If the code matches a language in the list, its <see
	/// cref="SelectListItem.Selected"/> property will be set to <see langword="true"/>.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="SelectListItem"/>
	/// objects, each representing a language. The list is sorted by the display text of the languages.</returns>
	public List<SelectListItem> GetLanguagesOptionsAsync(string selected)
	{
		var codes = _languagesService.GetRequiredLanguagesAsync();
		List<SelectListItem> languages = [];
		if(codes.Success == false)
		{
			return [];
		}
		else
		{
			foreach(Language language in codes.Data ?? [])
			{
				languages.Add(new SelectListItem
				{
					Selected = language.Code == selected,
					Value = language.Code,
					Text = language.Native
				});
			}
		}
		languages = [.. languages.OrderBy(l => l.Text)];

		return languages;
	}

	/// <summary>
	/// Retrieves a list of themes formatted as selectable items, with an optional preselected theme.
	/// </summary>
	/// <remarks>This method generates a list of themes suitable for use in dropdowns or other selection controls.
	/// If no themes are available, a default "dark" theme is added to the list.</remarks>
	/// <param name="selected">The theme to mark as selected in the resulting list. If <paramref name="selected"/> is null or does not match any
	/// theme, no item will be preselected.</param>
	/// <returns>A list of <see cref="SelectListItem"/> objects representing the available themes. If no themes are available, the
	/// list will contain a single item with the value "dark" preselected.</returns>
	public List<SelectListItem> GetThemes(string? selected)
	{
		List<SelectListItem> result = new();
		var themes = GetThemesList();
		if(themes == null || themes.Count == 0)
		{
			result.Add(new SelectListItem { Text = "dark", Value = "dark", Selected = true });
		}
		else
		{
			foreach(var theme in themes)
			{
				result.Add(new SelectListItem
				{
					Text = theme,
					Value = theme,
					Selected = theme == selected
				});
			}
		}
		return result;
	}

	// Asynchronous methods

	// private methods
	private List<string> GetThemesList()
	{
		if(!Directory.Exists(_cssFolderPath))
		{
			throw new DirectoryNotFoundException($"The folder '{_cssFolderPath}' does not exist.");
		}

		string[] themeFiles = Directory.GetFiles(_cssFolderPath, "*-theme.css", SearchOption.TopDirectoryOnly);

		List<string> themeNames = [.. themeFiles
					 .Select(file => System.IO.Path.GetFileNameWithoutExtension(file)) // Extract file name without extension
					 .Select(fileName =>
					 {
						 string[] parts = fileName.Split('_', 2);
						 return fileName.Replace("-theme", "");
					 })];
		return themeNames;
	}
}