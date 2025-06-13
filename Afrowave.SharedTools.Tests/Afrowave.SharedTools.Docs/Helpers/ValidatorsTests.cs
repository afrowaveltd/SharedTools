using Afrowave.SharedTools.Docs.Helpers;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Docs.Helpers
{
	public class ValidatorsTests
	{
		[Fact]
		public void IsValidEmail_ShouldReturnTrue_ForValidEmail()
		{
			var result = Validators.IsValidEmail("test@example.com");
			Assert.True(result);
		}

		[Fact]
		public void IsValidEmail_ShouldReturnFalse_ForInvalidEmail()
		{
			var result = Validators.IsValidEmail("invalid-email");
			Assert.False(result);
		}

		[Fact]
		public void IsValidEmail_ShouldReturnFalse_ForNullOrEmptyEmail()
		{
			Assert.False(Validators.IsValidEmail(null));
			Assert.False(Validators.IsValidEmail(string.Empty));
			Assert.False(Validators.IsValidEmail("   "));
		}

		[Fact]
		public void IsValidEmailRegex_ShouldReturnTrue_ForValidEmailFormat()
		{
			var result = Validators.IsValidEmailRegex("test@example.com");
			Assert.True(result);
		}

		[Fact]
		public void IsValidEmailRegex_ShouldReturnFalse_ForInvalidEmailFormat()
		{
			var result = Validators.IsValidEmailRegex("invalid-email");
			Assert.False(result);
		}

		[Fact]
		public void IsValidEmailRegex_ShouldReturnFalse_ForNullOrEmptyEmail()
		{
			Assert.False(Validators.IsValidEmailRegex(null));
			Assert.False(Validators.IsValidEmailRegex(string.Empty));
			Assert.False(Validators.IsValidEmailRegex("   "));
		}
	}
}