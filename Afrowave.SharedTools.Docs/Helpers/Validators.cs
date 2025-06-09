using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Afrowave.SharedTools.Docs.Helpers
{
	/// <summary>
	/// Provides utility methods for validating email addresses.
	/// </summary>
	/// <remarks>This class includes methods for validating email addresses using different approaches,  such as
	/// format validation and regular expressions. These methods are static and can be  used without instantiating the
	/// class.</remarks>
	public static class Validators
	{
		/// <summary>
		/// Determines whether the specified string is a valid email address.
		/// </summary>
		/// <remarks>This method checks if the input string is a syntactically valid email address. It returns <see
		/// langword="false"/>  for null, empty, or whitespace-only strings, as well as for strings that do not conform to
		/// email address formatting rules.</remarks>
		/// <param name="email">The email address to validate. Cannot be null, empty, or consist only of whitespace.</param>
		/// <returns><see langword="true"/> if the specified string is a valid email address; otherwise, <see langword="false"/>.</returns>
		public static bool IsValidEmail(string email)
		{
			if(string.IsNullOrWhiteSpace(email))
				return false;

			try
			{
				var addr = new MailAddress(email);
				return addr.Address == email;
			}
			catch { return false; }
		}

		/// <summary>
		/// Determines whether the specified email address is in a valid format based on a regular expression.
		/// </summary>
		/// <remarks>This method uses a regular expression to validate the format of the email address.  It does not
		/// verify the existence of the email address or its domain.</remarks>
		/// <param name="email">The email address to validate. This value can be null or empty.</param>
		/// <returns><see langword="true"/> if the specified email address matches the expected format;  otherwise, <see
		/// langword="false"/>.</returns>
		public static bool IsValidEmailRegex(string email)
		{
			if(string.IsNullOrWhiteSpace(email))
				return false;

			string vzor = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
			return Regex.IsMatch(email, vzor);
		}

		/// <summary>
		/// Determines whether the specified string is a valid email address.
		/// </summary>
		/// <remarks>This method validates the email address using multiple criteria, including format and pattern
		/// matching.</remarks>
		/// <param name="email">The string to validate as an email address. Cannot be <see langword="null"/> or empty.</param>
		/// <returns><see langword="true"/> if the specified string is a valid email address; otherwise, <see langword="false"/>.</returns>
		public static bool IsEmail(string email) => IsValidEmail(email) && IsValidEmailRegex(email);
	}
}