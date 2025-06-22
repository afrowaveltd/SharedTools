// page elements
const basic_statistics_element = document.getElementById('basic_statistics');
const status_table = document.getElementById('status_table');

//page functions
const initializeCycle = () => {
	// reset function that will set all elements to their default values
}

// SignalR functions
manager.hubs.realtime.connection.on('ReceiveLanguages', async (languages) => {
	let msg = "✅&nbsp;&nbsp; " + await localize("Libretranslate languages count");

	let msg2 = "<b>" + languages.length + "</b>";
	status_table.innerHTML = msg + ": &nbsp;&nbsp;" + msg2;

	// Process the received languages as needed
	// For example, you can update the UI or store them in a variable
});

manager.hubs.realtime.connection.on("ReceiveTranslationSettings", (settings) => {
	console.log(settings);
});