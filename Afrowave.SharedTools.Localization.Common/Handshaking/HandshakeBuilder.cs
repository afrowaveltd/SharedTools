using Afrowave.SharedTools.Localization.Common.Logging;
using Afrowave.SharedTools.Localization.Common.Options;
using System;
using System.Collections.Generic;

namespace Afrowave.SharedTools.Localization.Common.Handshaking
{
	public sealed class HandshakeBuilder : IHandshakeOptionsConfigurator
	{
		private readonly Afrowave.SharedTools.Localization.Common.Models.Handshake _handshake = new Afrowave.SharedTools.Localization.Common.Models.Handshake();
		private readonly HashSet<string> _excluded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		private readonly List<PluginScopeExclusion> _exclusionsOnce = new List<PluginScopeExclusion>();
		private ILogSink _logger;
		private LoggerVerbosity _verbosity = LoggerVerbosity.Normal;

		public HandshakeBuilder UseRead()
		{ _handshake.UseRead = true; return this; }

		public HandshakeBuilder UseWrite()
		{ _handshake.UseWrite = true; return this; }

		public HandshakeBuilder UseUpdate()
		{ _handshake.UseUpdate = true; return this; }

		public HandshakeBuilder UseDelete()
		{ _handshake.UseDelete = true; return this; }

		public HandshakeBuilder UseBulkRead()
		{ _handshake.UseBulkRead = true; return this; }

		public HandshakeBuilder UseBulkWrite()
		{ _handshake.UseBulkWrite = true; return this; }

		public HandshakeBuilder UseListLanguages()
		{ _handshake.UseListLanguages = true; return this; }

		public HandshakeBuilder UseCache()
		{ _handshake.UseCache = true; return this; }

		public HandshakeBuilder UseStream()
		{ _handshake.UseStream = true; return this; }

		public HandshakeBuilder UseSignalR(Action<SignalROptions> configure = null)
		{
			_handshake.UseSignalR = true;
			configure?.Invoke(_handshake.GetOrCreateOption<SignalROptions>());
			return this;
		}

		public HandshakeBuilder UseEvents()
		{ _handshake.UseEvents = true; return this; }

		public HandshakeBuilder UseLogging()
		{ _handshake.UseLogging = true; return this; }

		public HandshakeBuilder ExcludeRead()
		{ _excluded.Add(nameof(_handshake.UseRead)); return this; }

		public HandshakeBuilder ExcludeWrite()
		{ _excluded.Add(nameof(_handshake.UseWrite)); return this; }

		public HandshakeBuilder ExcludeUpdate()
		{ _excluded.Add(nameof(_handshake.UseUpdate)); return this; }

		public HandshakeBuilder ExcludeDelete()
		{ _excluded.Add(nameof(_handshake.UseDelete)); return this; }

		public HandshakeBuilder ExcludeBulkRead()
		{ _excluded.Add(nameof(_handshake.UseBulkRead)); return this; }

		public HandshakeBuilder ExcludeBulkWrite()
		{ _excluded.Add(nameof(_handshake.UseBulkWrite)); return this; }

		public HandshakeBuilder ExcludeListLanguages()
		{ _excluded.Add(nameof(_handshake.UseListLanguages)); return this; }

		public HandshakeBuilder ExcludeCache()
		{ _excluded.Add(nameof(_handshake.UseCache)); return this; }

		public HandshakeBuilder ExcludeStream()
		{ _excluded.Add(nameof(_handshake.UseStream)); return this; }

		public HandshakeBuilder ExcludeSignalR()
		{ _excluded.Add(nameof(_handshake.UseSignalR)); return this; }

		public HandshakeBuilder ExcludeEvents()
		{ _excluded.Add(nameof(_handshake.UseEvents)); return this; }

		public HandshakeBuilder ExcludeLogging()
		{ _excluded.Add(nameof(_handshake.UseLogging)); return this; }

		public HandshakeBuilder UseAll()
		{
			return UseRead().UseWrite().UseUpdate().UseDelete()
				 .UseBulkRead().UseBulkWrite()
				 .UseListLanguages().UseCache().UseStream()
				 .UseSignalR().UseEvents().UseLogging();
		}

		public HandshakeBuilder UseAll(Action<IHandshakeOptionsConfigurator> config)
		{
			UseAll();
			config?.Invoke(this);
			return this;
		}

		public HandshakeBuilder UseDefaults()
		{
			return UseRead().UseCache().UseLogging();
		}

		public HandshakeBuilder ExcludeOnce(string pluginId, string key = null, string reason = null)
		{
			_exclusionsOnce.Add(new PluginScopeExclusion
			{
				PluginId = pluginId,
				ForKey = key,
				Reason = reason
			});
			return this;
		}

		public HandshakeBuilder AddOption<T>(T option) where T : PluginOptionSet
		{
			if(option != null)
			{
				_handshake.Options[option.Key] = option;
			}
			return this;
		}

		public HandshakeBuilder WithLogger(ILogSink logger, LoggerVerbosity verbosity = LoggerVerbosity.Normal)
		{
			_logger = logger;
			_verbosity = verbosity;
			return this;
		}

		public Handshake Build()
		{
			// Apply exclusions
			foreach(var prop in _excluded)
			{
				var pi = typeof(Handshake).GetProperty(prop);
				if(pi != null && pi.CanWrite && pi.PropertyType == typeof(bool))
					pi.SetValue(_handshake, false);
			}

			return _handshake;
		}
	}
}