using Afrowave.SharedTools.Localization.Helpers;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Localization.Helpers;

public class PathHelperTests
{
	[Fact]
	public void ResolveJsonsFile_Resolves_Relative_To_Library()
	{
		var expected = Path.Combine(PathHelper.GetJsonsFolder(), "testfile.json");
		var actual = PathHelper.ResolveJsonsFile("testfile.json");
		Assert.Equal(expected, actual);
	}

	[Fact]
	public void ResolveJsonsFile_Absolute_Returns_Absolute()
	{
		var abs = Path.GetFullPath("C:/Temp/xyz.json");
		var actual = PathHelper.ResolveJsonsFile("anything.json", abs);
		Assert.Equal(abs, actual);
	}
}