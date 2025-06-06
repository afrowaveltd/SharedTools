namespace Afrowave.SharedTools.Docs.Models.Enums
{
	/// <summary>
	/// Specifies the complexity levels for generating one-time passwords (OTPs).
	/// </summary>
	/// <remarks>The complexity level determines the length and character composition of the generated OTPs: <list
	/// type="bullet"> <item> <description><see cref="Simple"/>: A six-character string consisting of numeric digits
	/// only.</description> </item> <item> <description><see cref="Normal"/>: A six-character string consisting of numeric
	/// digits and alphabetic characters.</description> </item> <item> <description><see cref="Long"/>: A twelve-character
	/// string consisting of numeric digits and alphabetic characters.</description> </item> <item> <description><see
	/// cref="Paranoid"/>: A twelve-character string consisting of numeric digits and case-sensitive alphabetic
	/// characters.</description> </item> </list></remarks>
	public enum OtpComplexity
	{
		/// <summary>
		/// Simply mode - OTP contains 6 numeric characters
		/// </summary>
		Simple = 0, // six numbers string

		/// <summary>
		/// Represents the normal priority level. String contains small letter characters and digits - 6 characters
		/// </summary>
		Normal = 1, // six numbers and characters string

		/// <summary>
		/// High security OTP - small letter characters and digit - 12 characters
		/// </summary>
		Secure = 2, // twelve numbers and character things

		/// <summary>
		/// Represents a security level where twelve numbers and character strings are case-sensitive.
		/// </summary>
		/// <remarks>This level enforces strict validation, requiring both numbers and character strings to be
		/// case-sensitive.</remarks>
		Paranoid = 3 // twelve numbers and character strings case sensitive
	}
}