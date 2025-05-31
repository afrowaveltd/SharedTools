using Afrowave.SharedTools.Localization.Common.Handshake;

namespace Afrowave.SharedTools.Localization.Common.Handshaking
{
	/// <summary>
	/// Interface for fluent handshake feature selection, usable in callbacks like UseAll(...).
	/// </summary>
	public interface IHandshakeOptionsConfigurator
	{
		HandshakeBuilder UseRead();

		HandshakeBuilder UseWrite();

		HandshakeBuilder UseUpdate();

		HandshakeBuilder UseDelete();

		HandshakeBuilder UseBulkRead();

		HandshakeBuilder UseBulkWrite();

		HandshakeBuilder UseListLanguages();

		HandshakeBuilder UseCache();

		HandshakeBuilder UseStream();

		HandshakeBuilder UseSignalR(System.Action<SignalROptions> configure = null);

		HandshakeBuilder UseEvents();

		HandshakeBuilder UseLogging();

		HandshakeBuilder ExcludeRead();

		HandshakeBuilder ExcludeWrite();

		HandshakeBuilder ExcludeUpdate();

		HandshakeBuilder ExcludeDelete();

		HandshakeBuilder ExcludeBulkRead();

		HandshakeBuilder ExcludeBulkWrite();

		HandshakeBuilder ExcludeListLanguages();

		HandshakeBuilder ExcludeCache();

		HandshakeBuilder ExcludeStream();

		HandshakeBuilder ExcludeSignalR();

		HandshakeBuilder ExcludeEvents();

		HandshakeBuilder ExcludeLogging();
	}
}