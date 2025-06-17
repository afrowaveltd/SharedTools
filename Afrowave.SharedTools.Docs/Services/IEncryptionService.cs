namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Defines methods for encryption, decryption, password hashing, and OTP generation.
	/// </summary>
	/// <remarks>This interface provides a set of cryptographic operations, including text encryption and
	/// decryption,  password hashing and verification, and generation of application secrets and one-time passwords
	/// (OTPs). Implementations of this interface should ensure secure handling of sensitive data.</remarks>
	public interface IEncryptionService
	{
		/// <summary>
		/// Decrypts the specified encrypted text using the provided key.
		/// </summary>
		/// <param name="encryptedText">The text to decrypt. This must be a valid encrypted string.</param>
		/// <param name="key">The decryption key to use. The key must match the one used during encryption.</param>
		/// <returns>The decrypted plain text as a string.</returns>
		string DecryptTextAsync(string encryptedText, string key);

		/// <summary>
		/// Encrypts the specified text using the provided encryption key.
		/// </summary>
		/// <remarks>The encryption algorithm and key requirements depend on the implementation. Ensure the key meets
		/// the necessary criteria for the algorithm used. The caller is responsible for securely managing the encryption
		/// key.</remarks>
		/// <param name="text">The plain text to encrypt. Cannot be null or empty.</param>
		/// <param name="key">The encryption key to use. Must be a valid key for the encryption algorithm.</param>
		/// <returns>A string containing the encrypted text.</returns>
		string EncryptTextAsync(string text, string key);

		/// <summary>
		/// Generates a new application secret.
		/// </summary>
		/// <returns>A string representing the newly generated application secret. The secret is typically used for authentication or
		/// secure communication.</returns>
		string GenerateApplicationSecret();

		/// <summary>
		/// Generates a one-time password (OTP) as a string.
		/// </summary>
		/// <returns>A randomly generated OTP as a string. The format and length of the OTP may vary depending on the implementation.</returns>
		string GenerateOtp();

		/// <summary>
		/// Generates a one-time password (OTP) based on the specified complexity level.
		/// </summary>
		/// <remarks>The generated OTP is intended for temporary use and should be securely transmitted and stored.
		/// Ensure that the <paramref name="complexity"/> parameter is set to a valid value to avoid exceptions.</remarks>
		/// <param name="complexity">The desired complexity level of the OTP, which determines its length and character set.</param>
		/// <returns>A string representing the generated OTP. The format and length of the OTP depend on the specified <paramref
		/// name="complexity"/>.</returns>
		string GenerateOtp(OtpComplexity complexity);

		/// <summary>
		/// Asynchronously generates a hashed representation of the specified password.
		/// </summary>
		/// <remarks>The hashing algorithm used ensures that the resulting hash is suitable for secure password
		/// storage. The caller is responsible for securely storing the returned hash.</remarks>
		/// <param name="password">The plain-text password to hash. Must not be null or empty.</param>
		/// <returns>A string containing the hashed version of the password.</returns>
		string HashPasswordAsync(string password);

		/// <summary>
		/// Verifies whether the provided password matches the specified hashed password.
		/// </summary>
		/// <remarks>This method compares the plain-text password with the hashed password using a secure hashing
		/// algorithm. Ensure that the hashed password was generated using a compatible hashing method.</remarks>
		/// <param name="password">The plain-text password to verify.</param>
		/// <param name="hashedPassword">The hashed password to compare against.</param>
		/// <returns><see langword="true"/> if the password matches the hashed password; otherwise, <see langword="false"/>.</returns>
		bool VerifyPassword(string password, string hashedPassword);
	}
}