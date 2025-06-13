using Afrowave.SharedTools.Docs.I18n;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using NSubstitute;

namespace Afrowave.SharedTools.Tests.Afrowave.SharedTools.Docs.I18n
{
	/// <summary>
	/// Unit tests for the <see cref="JsonStringLocalizerFactory"/> class.
	/// </summary>
	public class JsonStringLocalizerFactoryTests
	{
		private readonly IDistributedCache _mockCache;
		private readonly JsonStringLocalizerFactory _factory;

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonStringLocalizerFactoryTests"/> class.
		/// </summary>
		public JsonStringLocalizerFactoryTests()
		{
			_mockCache = Substitute.For<IDistributedCache>();
			_factory = new JsonStringLocalizerFactory(_mockCache);
		}

		/// <summary>
		/// Verifies that the <see cref="JsonStringLocalizerFactory"/> constructor initializes the instance correctly.
		/// </summary>
		[Fact]
		public void Create_WithResourceSource_ReturnsJsonStringLocalizer()
		{
			// Act
			IStringLocalizer localizer = _factory.Create(typeof(JsonStringLocalizerFactoryTests));

			// Assert
			Assert.NotNull(localizer);
			_ = Assert.IsType<JsonStringLocalizer>(localizer);
		}

		/// <summary>
		/// Verifies that the <see cref="JsonStringLocalizerFactory"/> constructor initializes the instance correctly.
		/// </summary>
		[Fact]
		public void Create_WithBaseNameAndLocation_ReturnsJsonStringLocalizer()
		{
			// Act
			IStringLocalizer localizer = _factory.Create("Resource", "Location");

			// Assert
			Assert.NotNull(localizer);
			_ = Assert.IsType<JsonStringLocalizer>(localizer);
		}
	}
}