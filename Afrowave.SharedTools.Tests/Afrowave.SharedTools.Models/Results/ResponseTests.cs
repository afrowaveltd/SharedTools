using Afrowave.SharedTools.Models.Results;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Models.Results
{
	public class ResponseTests
	{
		[Fact]
		public void SuccessResponse_ShouldBeSuccessful_WithDataAndMessage()
		{
			var response = Response<string>.SuccessResponse("DATA", "Success message");

			Assert.True(response.Success);
			Assert.False(response.Warning);
			Assert.Equal("Success message", response.Message);
			Assert.Equal("DATA", response.Data);
		}

		[Fact]
		public void Fail_ShouldReturnFailure_WithMessage()
		{
			var response = Response<string>.Fail("Failure occurred");

			Assert.False(response.Success);
			Assert.Equal("Failure occurred", response.Message);
		}

		[Fact]
		public void EmptySuccess_ShouldReturnSuccessWithoutMessageOrData()
		{
			var response = Response<string>.EmptySuccess();

			Assert.True(response.Success);
			Assert.Null(response.Data);
			Assert.Equal(string.Empty, response.Message);
		}

		[Fact]
		public void SuccessWithWarning_ShouldSetWarningFlag_AndMessage()
		{
			var response = Response<int>.SuccessWithWarning(42, "This is a warning");

			Assert.True(response.Success);
			Assert.True(response.Warning);
			Assert.Equal(42, response.Data);
			Assert.Equal("This is a warning", response.Message);
		}

		[Fact]
		public void Fail_WithException_ShouldExtractMessage()
		{
			var ex = new System.Exception("Something bad happened");
			var response = Response<string>.Fail(ex);

			Assert.False(response.Success);
			Assert.Equal("Something bad happened", response.Message);
		}
	}
}