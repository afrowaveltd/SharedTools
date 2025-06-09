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

		/// <summary>
		/// Validates the installation form data for an application and returns a list of validation errors, if any.
		/// </summary>
		/// <remarks>This method performs a series of checks to ensure the provided application data meets the
		/// required criteria: <list type="bullet"> <item><description>The <paramref name="application"/> parameter must not
		/// be <see langword="null"/>.</description></item> <item><description>The application name, display name, SMTP host,
		/// and email must be valid and non-empty.</description></item> <item><description>The SMTP port must be within the
		/// range of 1 to 65535.</description></item> <item><description>If authentication is required, both the SMTP login
		/// and password must be provided.</description></item> </list> If any validation errors are found, they are included
		/// in the returned list of errors.</remarks>
		/// <param name="application">The <see cref="ApplicationInstall"/> object containing the application's installation form data to validate.</param>
		/// <returns>A <see cref="Response{T}"/> object containing a list of validation errors.  If the validation is successful, the
		/// <see cref="Response{T}.Success"/> property is <see langword="true"/> and the list of errors is empty.  Otherwise,
		/// <see cref="Response{T}.Success"/> is <see langword="false"/> and the list contains the validation error messages.</returns>
		public static Response<List<string>> IsInstallationFormValid(ApplicationInstall application)
		{
			Response<List<string>> result = new()
			{
				Data = []
			};
			var errors = new List<string>();

			// Validate the application data
			if(application == null)
			{
				errors.Add("Application data cannot be null.");
				result.Success = false;
				return result;
			}
			if(string.IsNullOrWhiteSpace(application.ApplicationName))
			{
				result.Message += ("Application name was empty. Used default \n");
				result.Warning = true;
			}
			if(!IsEmail(application.Email))
			{
				errors.Add("Email is not valid.");
				result.Success = false;
			}
			if(string.IsNullOrWhiteSpace(application.DisplayName))
			{
				errors.Add("Display name was empty.");
				result.Success = false;
			}
			if(string.IsNullOrWhiteSpace(application.SmtpHost))
			{
				errors.Add("Host was empty.");
				result.Success = false;
			}
			if(application.SmtpPort < 1 || application.SmtpPort > 65535)
			{
				errors.Add("Port number is out of range. It should be between 1 and 65535.");
				result.Success = false;
			}
			if(application.UseAuthentication)
			{
				if(string.IsNullOrWhiteSpace(application.SmtpLogin))
				{
					errors.Add("Sender email is required when authentication is used.");
					result.Success = false;
				}
				if(string.IsNullOrWhiteSpace(application.SmtpPassword))
				{
					errors.Add("Sender password is required when authentication is used.");
					result.Success = false;
				}
			}

			result.Data = errors;
			return result;
		}
	}
}