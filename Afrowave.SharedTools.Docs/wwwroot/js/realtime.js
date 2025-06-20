// page elements
const basic_statistics_element = document.getElementById('basic_statistics');

//page functions
const initializeCycle = () => {
	// reset function that will set all elements to their default values
	basic_statistics_element.innerHTML = "";
}

// SignalR functions
manager.hubs.realtime.connection.on('ReceiveLanguages', async (languages) => {
	console.log("Received languages:", languages);
	let msg = await localize("Libretranslate languages count");
	msg = "<span>" + msg + ":<b> " + languages.length + " </b></span><span style='text-right'>  ✔️</span><br> ";
	basic_statistics_element.innerHTML = msg;

	// Process the received languages as needed
	// For example, you can update the UI or store them in a variable
});

manager.hubs.realtime.connection.on("ReceiveTranslationSettings", (settings) => {
	console.log(settings);
});