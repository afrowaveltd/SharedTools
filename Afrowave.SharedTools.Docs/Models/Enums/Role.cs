namespace Afrowave.SharedTools.Docs.Models.Enums;

/// <summary>
/// User role types
/// </summary>
public enum Role
{
	/// <summary>
	/// Represents a user in the system, including their personal and account-related information.
	/// </summary>
	User,

	/// <summary>
	/// Represents a text editor that provides functionality for editing and managing text content.
	/// </summary>
	/// <remarks>The <c>Editor</c> role is designed to handle text editing operations such as inserting, deleting,
	/// and modifying text. It can be used in applications that require text manipulation features, such as  code editors,
	/// word processors, or note-taking tools.</remarks>
	Editor,

	/// <summary>
	/// Provides functionality to translate text between different languages.
	/// </summary>
	/// <remarks>This role is designed to handle text translation for supported languages. </remarks>
	Translator,

	/// <summary>
	/// Represents an administrative user with elevated permissions and access to manage system-level operations.
	/// </summary>
	/// <remarks>This class is typically used to define users who have the ability to perform administrative tasks,
	/// such as managing other users, configuring system settings, or accessing restricted resources.  Ensure that
	/// instances of this class are only assigned to trusted individuals.</remarks>
	Admin,

	/// <summary>
	/// Represents the owner of the system.
	/// </summary>
	/// <remarks>This property identifies the individual or entity responsible for the system.</remarks>
	Owner
}