using Afrowave.SharedTools.Localization.Common.Options;

namespace Afrowave.SharedTools.Localization.Common.Handshaking
{
	/// <summary>
	/// Interface for fluent handshake feature selection, usable in callbacks like UseAll(...).
	/// </summary>
	public interface IHandshakeOptionsConfigurator
	{
		/// <summary>
		/// Configures the handshake builder to use the read operation during the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to include the read operation as part of its
		/// configuration. Subsequent calls to build the handshake will incorporate this operation.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance configured to use the read operation.</returns>
		HandshakeBuilder UseRead();

		/// <summary>
		/// Configures the handshake builder to use the write operation during the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to include the write operation as part of its</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance configured to use the write operation.</returns>
		HandshakeBuilder UseWrite();

		/// <summary>
		/// Configures the handshake builder to use the update mechanism.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to include update functionality,  allowing subsequent
		/// operations to utilize updated handshake parameters.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance configured with the update mechanism.</returns>
		HandshakeBuilder UseUpdate();

		/// <summary>
		/// Configures the handshake builder to use the delete operation during the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to include the delete operation as part of its
		/// configuration. Subsequent calls to build the handshake will incorporate this operation.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance configured to use the delete operation.</returns>
		HandshakeBuilder UseDelete();

		/// <summary>
		/// Configures the handshake builder to use bulk read mode.
		/// </summary>
		/// <remarks>Bulk read mode enables the handshake builder to process data in larger chunks,  which may improve
		/// performance for scenarios involving high-volume data transfers.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		HandshakeBuilder UseBulkRead();

		/// <summary>
		/// Configures the handshake builder to use bulk write operations.
		/// </summary>
		/// <remarks>Bulk write operations allow for more efficient data transfer by sending larger chunks of data at
		/// once. This method modifies the handshake builder's behavior to optimize for scenarios where bulk data processing
		/// is required.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, configured to use bulk write operations.</returns>
		HandshakeBuilder UseBulkWrite();

		/// <summary>
		/// Configures the handshake builder to include a list of supported languages during the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to ensure that the supported languages are
		/// communicated  during the handshake. Use this method when language negotiation is required as part of the
		/// handshake.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance configured to include the list of supported languages.</returns>
		HandshakeBuilder UseListLanguages();

		/// <summary>
		/// Configures the handshake builder to use cached data for faster processing.
		/// </summary>
		/// <remarks>When caching is enabled, the handshake builder may retrieve previously stored data  to optimize
		/// performance. This can reduce the time required for repeated operations  but may result in outdated data if the
		/// cache is not refreshed.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		HandshakeBuilder UseCache();

		/// <summary>
		/// Configures the handshake builder to use a stream-based approach for data transmission.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to operate using a stream, enabling scenarios  where
		/// data is transmitted incrementally or in real-time. Use this method when stream-based  communication is
		/// required.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing for method chaining.</returns>
		HandshakeBuilder UseStream();

		/// <summary>
		/// Configures SignalR for the application, allowing real-time communication between the server and connected clients.
		/// </summary>
		/// <remarks>This method integrates SignalR into the application's middleware pipeline, enabling support for
		/// real-time messaging. If no configuration delegate is provided, default SignalR settings are applied.</remarks>
		/// <param name="configure">An optional delegate to configure SignalR options. If provided, the delegate is invoked to customize settings such
		/// as hubs, protocols, and other SignalR-specific configurations.</param>
		/// <returns>A <see cref="HandshakeBuilder"/> instance that can be used to further configure the application's middleware
		/// pipeline.</returns>
		HandshakeBuilder UseSignalR(System.Action<SignalROptions> configure = null);

		/// <summary>
		/// Configures the handshake builder to use events for processing.
		/// </summary>
		/// <remarks>This method enables event-driven handling within the handshake builder, allowing subscribers  to
		/// respond to specific events during the handshake process.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance configured to use events.</returns>
		HandshakeBuilder UseEvents();

		/// <summary>
		/// Enables logging for the handshake process.
		/// </summary>
		/// <remarks>This method configures the handshake builder to include logging functionality,  allowing detailed
		/// information about the handshake process to be recorded. Logging can be useful for debugging or monitoring
		/// purposes.</remarks>
		/// <returns>The current instance of <see cref="HandshakeBuilder"/>, allowing method chaining.</returns>
		HandshakeBuilder UseLogging();

		/// <summary>
		/// Excludes the read operation from the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake configuration to exclude the read operation,  ensuring that
		/// subsequent handshake steps do not involve reading data. Use this method when the handshake process should only
		/// include write or other operations.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with the updated configuration.</returns>
		HandshakeBuilder ExcludeRead();

		/// <summary>
		/// Excludes the write operation from the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake configuration to exclude the write operation,  ensuring that
		/// subsequent handshake steps do not involve writing data. Use this method when the handshake process should
		/// only include read or other operations.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with the updated configuration.</returns>
		HandshakeBuilder ExcludeWrite();

		/// <summary>
		/// Excludes the update handshake from the current handshake builder.
		/// </summary>
		/// <remarks>Use this method to create a handshake builder that omits the update handshake. This can be useful
		/// in scenarios where updates are not required or should be excluded from the handshake process.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance that does not include the update handshake.</returns>
		HandshakeBuilder ExcludeUpdate();

		/// <summary>
		/// Excludes the delete operation from the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake configuration to exclude the delete operation,  ensuring that
		/// the handshake process does not involve this operation. This can be useful in scenarios where deletion should be avoided.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with the updated configuration.</returns>
		HandshakeBuilder ExcludeDelete();

		/// <summary>
		/// Excludes the bulk read operation from the handshake process.
		/// </summary>
		/// <remarks>Use this method to disable bulk read functionality during the handshake process. This can be
		/// useful in scenarios where bulk read operations are unnecessary or undesirable.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance that allows further configuration of the handshake.</returns>
		HandshakeBuilder ExcludeBulkRead();

		/// <summary>
		/// Excludes the bulk write operation from the handshake process.
		/// </summary>
		/// <remarks>Use this method to disable bulk write functionality during the handshake process. This can be
		/// useful in scenarios where bulk write operations are unnecessary or undesirable.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with the updated configuration.</returns>
		HandshakeBuilder ExcludeBulkWrite();

		/// <summary>
		/// Excludes the list of languages from the handshake configuration.
		/// </summary>
		/// <remarks>This method modifies the handshake builder to exclude specific languages from the configuration.
		/// It can be used to customize the handshake process by removing unwanted language options.</remarks>
		/// <returns>A modified <see cref="HandshakeBuilder"/> instance with the excluded languages applied.</returns>
		HandshakeBuilder ExcludeListLanguages();

		/// <summary>
		/// Excludes cached data from the handshake process.
		/// </summary>
		/// <remarks>Use this method to ensure that the handshake process does not rely on previously cached data.
		/// This can be useful in scenarios where fresh data is required for security or consistency.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance configured to exclude cached data.</returns>
		HandshakeBuilder ExcludeCache();

		/// <summary>
		/// Excludes the stream from the handshake process.
		/// </summary>
		/// <remarks>Use this method to modify the handshake configuration by removing the stream from consideration.
		/// This can be useful in scenarios where certain streams are not required or should be ignored during the handshake
		/// process.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance that represents the updated handshake configuration after excluding the
		/// stream.</returns>
		HandshakeBuilder ExcludeStream();

		/// <summary>
		/// Excludes SignalR-related components from the handshake process.
		/// </summary>
		/// <remarks>Use this method to exclude SignalR-specific functionality when building a handshake. This is
		/// useful in scenarios where SignalR is not required or should be omitted from the configuration.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance that can be further configured.</returns>
		HandshakeBuilder ExcludeSignalR();

		/// <summary>
		/// Excludes events from the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake configuration to exclude event-related data. Use this method
		/// when events are not required as part of the handshake.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with events excluded from the handshake configuration.</returns>
		HandshakeBuilder ExcludeEvents();

		/// <summary>
		/// Excludes logging from the handshake process.
		/// </summary>
		/// <remarks>This method modifies the handshake configuration to disable logging functionality. Use this method
		/// to ensure that logging information is not recorded during the handshake process.</remarks>
		/// <returns>A <see cref="HandshakeBuilder"/> instance with logging excluded from the handshake configuration.</returns>
		HandshakeBuilder ExcludeLogging();

		/// <summary>
		/// Marks this plugin as the default for its type.
		/// </summary>
		HandshakeBuilder UseAsDefault();

		/// <summary>
		/// Marks this plugin as a fallback if other plugins fail.
		/// </summary>
		HandshakeBuilder UseAsFallback();
	}
}