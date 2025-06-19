manager.hubs.realtime.connection.on('SendGreeting', function (message) {
	console.log("SignalR: " + message);
});