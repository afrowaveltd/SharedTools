using Microsoft.AspNetCore.Mvc.Rendering;

namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides methods for retrieving various option lists, such as binary options, language options, and theme options,
	/// for use in user interface selection controls.
	/// </summary>
	/// <remarks>This service is designed to generate lists of selectable options, typically for use in dropdown
	/// menus or similar UI components.  Each method returns a list of <see cref="SelectListItem"/> objects, which
	/// represent the available options.</remarks>
	public interface IOptionsService
	{
		/// <summary>
		/// Generates a list of binary options represented as <see cref="SelectListItem"/> objects.
		/// </summary>
		/// <param name="selected">A value indicating whether the first option in the list should be marked as selected. If <see langword="true"/>,
		/// the first option will be selected; otherwise, it will not.</param>
		/// <returns>A list of <see cref="SelectListItem"/> objects representing binary options, such as "Yes" and "No".</returns>
		List<SelectListItem> BinaryOptions(bool selected = true);

		/// <summary>
		/// Asynchronously retrieves a list of language options for selection.
		/// </summary>
		/// <remarks>The returned list is typically used to populate a dropdown or similar UI element for language
		/// selection.</remarks>
		/// <param name="selected">The value of the currently selected language. This value will be used to mark the corresponding option as selected
		/// in the returned list.</param>
		/// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="SelectListItem"/>
		/// objects, where each item represents a language option. The list will include one item marked as selected if the
		/// <paramref name="selected"/> value matches a language option.</returns>
		List<SelectListItem> GetLanguagesOptionsAsync(string selected);

		/// <summary>
		/// Retrieves a list of themes formatted as selectable items for use in a dropdown or similar UI component.
		/// </summary>
		/// <param name="selected">The value of the currently selected theme. If specified, the corresponding item in the list will be marked as
		/// selected.</param>
		/// <returns>A list of <see cref="SelectListItem"/> objects representing the available themes.  The list will include one item
		/// marked as selected if the <paramref name="selected"/> value matches an available theme.</returns>
		List<SelectListItem> GetThemes(string? selected);
	}
}