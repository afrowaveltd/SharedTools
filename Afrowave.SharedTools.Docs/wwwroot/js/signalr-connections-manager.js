// SignalRConnectionsManager.js (standalone version)

(function (global) {
	class SignalRConnectionsManager {
		constructor({ adminEnabled = false, onStatusChange = () => { } } = {}) {
			this.hubs = {
				open: {
					url: '/open_hub',
					connection: null,
					connected: true,
					reconnectDelay: 1000,
					maxReconnectDelay: 60000,
				},
				admin: {
					url: '/admin_hub',
					connection: null,
					connected: false,
					reconnectDelay: 1000,
					maxReconnectDelay: 60000,
					enabled: adminEnabled,
				}
			};
			this.onStatusChange = onStatusChange;
			this.init();
		}

		// Call this if user logs in (enables admin hub)
		enableAdminHub() {
			if (!this.hubs.admin.enabled) {
				this.hubs.admin.enabled = true;
				this.connectHub('admin');
			}
		}

		// Call this if user logs out (disables admin hub)
		disableAdminHub() {
			if (this.hubs.admin.enabled) {
				this.hubs.admin.enabled = false;
				this.disconnectHub('admin');
			}
		}

		// Exposes statuses for UI
		getStatuses() {
			return {
				open: this.hubs.open.connected,
				admin: this.hubs.admin.enabled ? this.hubs.admin.connected : null,
			};
		}

		// Internal init
		init() {
			this.connectHub('open');
			if (this.hubs.admin.enabled) {
				this.connectHub('admin');
			}
		}

		// Core connect logic with auto-reconnect
		connectHub(hubKey) {
			const hub = this.hubs[hubKey];
			if (!hub.enabled && hubKey === 'admin') return;

			if (hub.connection) {
				hub.connection.stop();
			}

			hub.connection = new signalR.HubConnectionBuilder()
				.withUrl(hub.url)
				.withAutomaticReconnect()
				.build();

			hub.connection.onclose(() => {
				hub.connected = false;
				this.onStatusChange(this.getStatuses());
				this.scheduleReconnect(hubKey);
			});

			hub.connection.onreconnecting(() => {
				hub.connected = false;
				this.onStatusChange(this.getStatuses());
			});

			hub.connection.onreconnected(() => {
				hub.connected = true;
				hub.reconnectDelay = 1000;
				this.onStatusChange(this.getStatuses());
			});

			hub.connection
				.start()
				.then(() => {
					hub.connected = true;
					hub.reconnectDelay = 1000;
					this.onStatusChange(this.getStatuses());
				})
				.catch((ex) => {
					hub.connected = false;
					this.onStatusChange(this.getStatuses());
					this.scheduleReconnect(hubKey);
				});
		}

		// Disconnect logic
		disconnectHub(hubKey) {
			const hub = this.hubs[hubKey];
			if (hub.connection) {
				hub.connection.stop();
				hub.connection = null;
			}
			hub.connected = false;
			this.onStatusChange(this.getStatuses());
		}

		// Reconnect with progressive backoff
		scheduleReconnect(hubKey) {
			const hub = this.hubs[hubKey];
			if (hubKey === 'admin' && !hub.enabled) return; // Don't reconnect if admin disabled

			setTimeout(() => {
				if (hubKey === 'admin' && !hub.enabled) return;
				this.connectHub(hubKey);
				// Increase delay up to max
				hub.reconnectDelay = Math.min(
					Math.floor(hub.reconnectDelay * 1.7),
					hub.maxReconnectDelay
				);
			}, hub.reconnectDelay);
		}
	}

	// Expose to global scope
	global.SignalRConnectionsManager = SignalRConnectionsManager;
})(window);