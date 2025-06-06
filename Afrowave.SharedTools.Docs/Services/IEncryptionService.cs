using Afrowave.SharedTools.Docs.Models.Enums;

namespace Afrowave.SharedTools.Docs.Services
{
	public interface IEncryptionService
	{
		string DecryptTextAsync(string encryptedText, string key);
		string EncryptTextAsync(string text, string key);
		string GenerateApplicationSecret();
		string GenerateOtp();
		string GenerateOtp(OtpComplexity complexity);
		string HashPasswordAsync(string password);
		bool VerifyPassword(string password, string hashedPassword);
	}
}