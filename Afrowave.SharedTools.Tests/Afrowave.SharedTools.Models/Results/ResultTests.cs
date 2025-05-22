using Afrowave.SharedTools.Models.Results;

namespace Afrowave.SharedTools.Tests.Models.Results
{
	public class ResultTests
	{
		[Fact]
		public void Ok_ShouldBeSuccessful_WithEmptyMessage()
		{
			var result = Result.Ok();

			Assert.True(result.Success);
			Assert.Equal(string.Empty, result.Message);
		}

		[Fact]
		public void Ok_WithMessage_ShouldReturnGivenMessage()
		{
			var result = Result.Ok("All good");

			Assert.True(result.Success);
			Assert.Equal("All good", result.Message);
		}

		[Fact]
		public void Fail_ShouldNotBeSuccessful()
		{
			var result = Result.Fail("Something went wrong");

			Assert.False(result.Success);
			Assert.Equal("Something went wrong", result.Message);
		}
	}
}