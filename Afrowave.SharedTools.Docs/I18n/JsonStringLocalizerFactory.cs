using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;

namespace Afrowave.SharedTools.Docs.I18n
{
	/// <summary>
	/// Factory for creating instances of <see cref="JsonStringLocalizer"/>.
	/// </summary>
	public class JsonStringLocalizerFactory(IDistributedCache cache) : IStringLocalizerFactory
	{
		private readonly IDistributedCache _cache = cache;

		/// <summary>
		/// Creates an <see cref="IStringLocalizer"/> for the given resource type.
		/// </summary>
		/// <param name="resourceSource">The type of the resource to localize.</param>
		/// <returns>An instance of <see cref="JsonStringLocalizer"/>.</returns>
		public IStringLocalizer Create(Type resourceSource)
		{
			return new JsonStringLocalizer(_cache);
		}

		/// <summary>
		/// Creates an <see cref="IStringLocalizer"/> using the specified base name and location.
		/// </summary>
		/// <param name="baseName">The base name of the resource to localize.</param>
		/// <param name="location">The location of the resource.</param>
		/// <returns>An instance of <see cref="JsonStringLocalizer"/>.</returns>
		public IStringLocalizer Create(string baseName, string location)
		{
			return new JsonStringLocalizer(_cache);
		}
	}
}