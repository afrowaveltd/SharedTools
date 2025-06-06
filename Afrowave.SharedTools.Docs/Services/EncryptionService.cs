using Afrowave.SharedTools.Docs.Models.Enums;
using System.Security.Cryptography;
using System.Text;

namespace Afrowave.SharedTools.Docs.Services
{
	/// <summary>
	/// Provides cryptographic services, including password hashing, password verification,  encryption, decryption,
	/// application secret generation, and one-time password (OTP) generation.
	/// </summary>
	/// <remarks>This service is designed to simplify common cryptographic operations for secure application
	/// development.  It includes methods for securely hashing and verifying passwords, generating cryptographically secure
	/// random values, encrypting and decrypting text using AES, and generating one-time passwords (OTPs) with  varying
	/// levels of complexity. The service is thread-safe and suitable for use in multi-threaded applications.</remarks>
	/// <param name="logger"></param>
	public class EncryptionService(ILogger<EncryptionService> logger) : IEncryptionService
	{
		private readonly ILogger<EncryptionService> _logger = logger;
		private const string Numbers = "0123456789";
		private const string LowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
		private const string UpperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

		/// <summary>
		/// Generates a hashed representation of the specified password using a cryptographic algorithm.
		/// </summary>
		/// <remarks>The method uses the PBKDF2 algorithm with SHA-256 as the hash function, a salt of 16 bytes,  and
		/// 10,000 iterations to derive the hash. The resulting string includes both the salt and  the hash, allowing for
		/// password verification.</remarks>
		/// <param name="password">The plain-text password to hash. Must not be null or empty.</param>
		/// <returns>A base64-encoded string containing the hashed password and the salt used during hashing.</returns>
		public string HashPasswordAsync(string password)
		{
			byte[] salt = new byte[16];
			RandomNumberGenerator.Fill(salt);

			byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(
				password,
				salt,
				10000,
				HashAlgorithmName.SHA256,
				32
			);

			byte[] hashWithSalt = new byte[48];
			Array.Copy(salt, 0, hashWithSalt, 0, 16);
			Array.Copy(hashBytes, 0, hashWithSalt, 16, 32);

			return Convert.ToBase64String(hashWithSalt);
		}

		/// <summary>
		/// Verifies whether the specified password matches the provided hashed password.
		/// </summary>
		/// <remarks>The method uses the PBKDF2 key derivation function with SHA-256 to compute the hash of the
		/// provided password using the salt extracted from the hashed password. The computed hash is then compared to the
		/// hash stored in the provided hashed password.</remarks>
		/// <param name="password">The plain-text password to verify.</param>
		/// <param name="hashedPassword">The hashed password, encoded as a Base64 string, which includes both the hash and the salt.</param>
		/// <returns><see langword="true"/> if the password matches the hashed password; otherwise, <see langword="false"/>.</returns>
		public bool VerifyPassword(string password, string hashedPassword)
		{
			byte[] hashWithSalt = Convert.FromBase64String(hashedPassword);
			byte[] salt = new byte[16];
			byte[] hashBytes = new byte[32];

			Array.Copy(hashWithSalt, 0, salt, 0, 16);
			Array.Copy(hashWithSalt, 16, hashBytes, 0, 32);

			byte[] newHashBytes = Rfc2898DeriveBytes.Pbkdf2(
				password,
				salt,
				10000,
				HashAlgorithmName.SHA256,
				32
			);

			return newHashBytes.SequenceEqual(hashBytes);
		}

		/// <summary>
		/// Generates a cryptographically secure application secret.
		/// </summary>
		/// <remarks>The generated secret is a 256-bit random value encoded as a Base64 string.  This method is
		/// suitable for scenarios requiring secure, random keys, such as API keys or encryption secrets.</remarks>
		/// <returns>A Base64-encoded string representing a 256-bit cryptographically secure random value.</returns>
		public string GenerateApplicationSecret()
		{
			using var rng = RandomNumberGenerator.Create();
			byte[] key = new byte[32]; // 256 bits
			rng.GetBytes(key);
			return Convert.ToBase64String(key);
		}

		/// <summary>
		/// Encrypts the specified plain text using the provided encryption key and returns the result as a Base64-encoded
		/// string.
		/// </summary>
		/// <remarks>The method uses AES encryption in CBC mode with PKCS7 padding. A random initialization vector
		/// (IV) is generated for each encryption operation and is prepended to the encrypted data. The caller must ensure
		/// that the same key is used for decryption.</remarks>
		/// <param name="text">The plain text to encrypt. Cannot be null or empty.</param>
		/// <param name="key">The encryption key as a Base64-encoded string. The key must be a valid AES key (e.g., 128, 192, or 256 bits).</param>
		/// <returns>A Base64-encoded string containing the encrypted data, which includes the initialization vector (IV) followed by
		/// the ciphertext.</returns>
		public string EncryptTextAsync(string text, string key)
		{
			byte[] keyBytes = Convert.FromBase64String(key);
			byte[] textBytes = Encoding.UTF8.GetBytes(text);

			using Aes aes = Aes.Create();
			aes.Key = keyBytes;
			aes.Mode = CipherMode.CBC; // Cipher Block Chaining mode
			aes.Padding = PaddingMode.PKCS7;

			// Generate a random IV (Initialization Vector)
			aes.GenerateIV();
			byte[] iv = aes.IV;

			using var encryptor = aes.CreateEncryptor();
			byte[] encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

			// Combine IV and encrypted data into a single array
			byte[] result = new byte[iv.Length + encryptedBytes.Length];
			Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
			Buffer.BlockCopy(encryptedBytes, 0, result, iv.Length, encryptedBytes.Length);

			return Convert.ToBase64String(result);
		}

		/// <summary>
		/// Decrypts the specified encrypted text using the provided key.
		/// </summary>
		/// <remarks>This method uses the AES algorithm in Cipher Block Chaining (CBC) mode with PKCS7 padding. The
		/// initialization vector (IV) is expected to be included at the beginning of the <paramref name="encryptedText"/>.
		/// Ensure that the key and IV used for decryption match those used during encryption.</remarks>
		/// <param name="encryptedText">A base64-encoded string representing the encrypted text. This must include the initialization vector (IV)
		/// prepended to the ciphertext.</param>
		/// <param name="key">A base64-encoded string representing the encryption key. The key must match the one used to encrypt the text.</param>
		/// <returns>The decrypted plaintext as a UTF-8 encoded string.</returns>
		public string DecryptTextAsync(string encryptedText, string key)
		{
			byte[] keyBytes = Convert.FromBase64String(key);
			byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

			using Aes aes = Aes.Create();
			aes.Key = keyBytes;
			aes.Mode = CipherMode.CBC; // Cipher Block Chaining mode
			aes.Padding = PaddingMode.PKCS7;

			// Extract the IV from the beginning of the encrypted data
			byte[] iv = new byte[aes.BlockSize / 8];
			byte[] cipherText = new byte[encryptedBytes.Length - iv.Length];
			Buffer.BlockCopy(encryptedBytes, 0, iv, 0, iv.Length);
			Buffer.BlockCopy(encryptedBytes, iv.Length, cipherText, 0, cipherText.Length);

			aes.IV = iv;

			using var decryptor = aes.CreateDecryptor();
			byte[] decryptedBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
			return Encoding.UTF8.GetString(decryptedBytes);
		}

		/// <summary>
		/// Generates a one-time password (OTP) based on the specified complexity level.
		/// </summary>
		/// <param name="complexity">The complexity level of the OTP.</param>
		/// <returns>A string containing the generated OTP.</returns>
		public string GenerateOtp(OtpComplexity complexity)
		{
			string allowedChars;
			int length;

			switch(complexity)
			{
				case OtpComplexity.Simple:
					allowedChars = Numbers;
					length = 6;
					break;

				case OtpComplexity.Normal:
					allowedChars = Numbers + LowerCaseLetters;
					length = 6;
					break;

				case OtpComplexity.Secure:
					allowedChars = Numbers + LowerCaseLetters;
					length = 12;
					break;

				case OtpComplexity.Paranoid:
					allowedChars = Numbers + LowerCaseLetters + UpperCaseLetters;
					length = 12;
					break;

				default:
					throw new ArgumentOutOfRangeException(nameof(complexity), complexity, null);
			}

			return GenerateRandomString(allowedChars, length);
		}

		/// <summary>
		/// Generates a one-time password (OTP) using the default complexity level.
		/// </summary>
		/// <remarks>This method is a shorthand for generating an OTP with a predefined complexity level, typically
		/// <see cref="OtpComplexity.Normal" />. To customize the complexity, use the overload that accepts an <see
		/// cref="OtpComplexity" /> parameter.</remarks>
		/// <returns>A string representing the generated OTP. The format and length of the OTP depend on the default complexity level.</returns>
		public string GenerateOtp()
		{
			return GenerateOtp(OtpComplexity.Normal);
		}

		private static string GenerateRandomString(string allowedChars, int length)
		{
			using var rng = RandomNumberGenerator.Create();
			var result = new char[length];
			var byteBuffer = new byte[length];

			rng.GetBytes(byteBuffer);

			for(int i = 0; i < length; i++)
			{
				var randomIndex = byteBuffer[i] % allowedChars.Length;
				result[i] = allowedChars[randomIndex];
			}

			return new string(result);
		}
	}
}