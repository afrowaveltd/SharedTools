const admin_hub_status_element = document.getElementById('admin-status');
const open_hub_status_element = document.getElementById('open-status');
const realtime_hub_status_element = document.getElementById('realtime-status');
const status_line = document.getElementById('bottom-line');

let manager = new SignalRConnectionsManager({
	adminEnabled: userLoggedIn, //true or false
	onStatusChange: function (statuses) {
		// Update UI accordin to the status
		open_hub_status_element.textContent = statuses.open ? '🟢' : '🔴';
		realtime_hub_status_element.textContent = statuses.open ? '🟢' : '🔴';
		if (statuses.admin !== null)
			admin_hub_status_element.textContent = statuses.admin ? '🟢' : '🔴';
		else
			admin_hub_status_element.textContent = '🔒';
	}
});

manager.hubs.open.connection.on("CycleStarted", async function () {
	console.log("Cycle started");
	const now = new Date();
	const time24 = now.toLocaleTimeString('cs', {
		hour12: false,
		hour: '2-digit',
		minute: '2-digit',
		second: '2-digit'
	});
	let text = await localize("Cycle start");

	status_line.innerHTML = text + ": " + time24;
});