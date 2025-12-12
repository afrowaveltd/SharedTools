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
         Assert.True(response.HasData);
         Assert.Equal("Success message", response.Message);
         Assert.Equal("DATA", response.Data);
      }

      [Fact]
      public void Fail_ShouldReturnFailure_WithMessage()
      {
         var response = Response<string>.Fail("Failure occurred");

         Assert.False(response.Success);
         Assert.False(response.Warning);
         Assert.False(response.HasData);
         Assert.Equal("Failure occurred", response.Message);
      }

      [Fact]
      public void Fail_Parameterless_ShouldReturnFailure_WithoutMessageOrData()
      {
         var response = Response<string>.Fail();

         Assert.False(response.Success);
         Assert.False(response.Warning);
         Assert.False(response.HasData);
         Assert.Equal(string.Empty, response.Message);
      }

      [Fact]
      public void EmptySuccess_ShouldReturnSuccessWithoutMessageOrData()
      {
         var response = Response<string>.EmptySuccess();

         Assert.True(response.Success);
         Assert.False(response.Warning);
         Assert.False(response.HasData);
         Assert.Null(response.Data); // default(string) is null, and that's fine when HasData=false
         Assert.Equal(string.Empty, response.Message);
      }

      [Fact]
      public void Ok_Parameterless_ShouldReturnSuccess_NoData()
      {
         var response = Response<int>.Ok();

         Assert.True(response.Success);
         Assert.False(response.Warning);
         Assert.False(response.HasData);
         Assert.Equal(string.Empty, response.Message);

         // Data is default(int)=0, but HasData=false means "not provided"
         Assert.Equal(default(int), response.Data);
      }

      [Fact]
      public void Ok_WithMessage_ShouldReturnSuccess_NoData_WithMessage()
      {
         var response = Response<string>.Ok("Done");

         Assert.True(response.Success);
         Assert.False(response.Warning);
         Assert.False(response.HasData);
         Assert.Equal("Done", response.Message);
         Assert.Null(response.Data);
      }

      [Fact]
      public void Ok_WithData_ShouldReturnSuccess_WithData()
      {
         var response = Response<int>.Ok(42);

         Assert.True(response.Success);
         Assert.False(response.Warning);
         Assert.True(response.HasData);
         Assert.Equal(string.Empty, response.Message);
         Assert.Equal(42, response.Data);
      }

      [Fact]
      public void Ok_WithDataAndMessage_ShouldReturnSuccess_WithDataAndMessage()
      {
         var response = Response<int>.Ok(42, "All good");

         Assert.True(response.Success);
         Assert.False(response.Warning);
         Assert.True(response.HasData);
         Assert.Equal("All good", response.Message);
         Assert.Equal(42, response.Data);
      }

      [Fact]
      public void OkWithWarning_NoData_ShouldSetWarningFlag_AndMessage()
      {
         var response = Response<int>.OkWithWarning("This is a warning");

         Assert.True(response.Success);
         Assert.True(response.Warning);
         Assert.False(response.HasData);
         Assert.Equal("This is a warning", response.Message);
      }

      [Fact]
      public void SuccessWithWarning_ShouldSetWarningFlag_AndMessage()
      {
         var response = Response<int>.SuccessWithWarning(42, "This is a warning");

         Assert.True(response.Success);
         Assert.True(response.Warning);
         Assert.True(response.HasData);
         Assert.Equal(42, response.Data);
         Assert.Equal("This is a warning", response.Message);
      }

      [Fact]
      public void OkWithWarning_WithData_ShouldSetWarningFlag_AndMessage_AndData()
      {
         var response = Response<int>.OkWithWarning(42, "This is a warning");

         Assert.True(response.Success);
         Assert.True(response.Warning);
         Assert.True(response.HasData);
         Assert.Equal(42, response.Data);
         Assert.Equal("This is a warning", response.Message);
      }

      [Fact]
      public void Fail_WithException_ShouldExtractMessage()
      {
         var ex = new System.Exception("Something bad happened");
         var response = Response<string>.Fail(ex);

         Assert.False(response.Success);
         Assert.False(response.Warning);
         Assert.False(response.HasData);
         Assert.Equal("Something bad happened", response.Message);
      }

      [Fact]
      public void NonGeneric_Response_Ok_ShouldWork()
      {
         var response = Response.Ok("Saved");

         Assert.True(response.Success);
         Assert.Equal("Saved", response.Message);
      }

      [Fact]
      public void NonGeneric_Response_Fail_ShouldWork()
      {
         var response = Response.Fail("Invalid");

         Assert.False(response.Success);
         Assert.Equal("Invalid", response.Message);
      }

      [Fact]
      public void Unit_ShouldHaveStableValue()
      {
         Assert.Equal("Unit", Unit.Value.ToString());
      }
   }
}