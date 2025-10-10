using Afrowave.SharedTools.I18N.Models;

namespace Afrowave.SharedTools.I18N.DataStorages.JsonFlat.Services
{
	/// <summary>
	/// Defines a contract for retrieving the capabilities supported by a data storage provider.
	/// </summary>
	/// <remarks>Implementations of this interface expose information about the features and limitations of a
	/// specific data storage system. This can be used to determine supported operations, performance characteristics, or
	/// compatibility with certain features.</remarks>
	public interface ICapabilities
	{
		/// <summary>
		/// Retrieves the capabilities supported by the data storage provider.
		/// </summary>
		/// <remarks>Use this method to determine which operations, data types, or features are available for the
		/// current storage implementation. The returned capabilities may affect how you interact with the provider, such as
		/// enabling or disabling certain functionality based on support.</remarks>
		/// <returns>A <see cref="DataStorageCapabilities"/> object describing the features and limitations of the data storage
		/// provider.</returns>
		DataStorageCapabilities GetCapabilities();
	}
}