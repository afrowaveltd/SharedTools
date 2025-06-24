// page elements
const basic_statistics_element = document.getElementById('basic_statistics');
const last_update_tick = document.getElementById('last_update_tick');
const last_update = document.getElementById('last_update');
const languages_received_tick = document.getElementById('languages_received_tick');
const languages_count = document.getElementById('languages_count');
const default_language_tick = document.getElementById('default_language_tick');
const default_language = document.getElementById('default_language');
const ignore_json_tick = document.getElementById('ignore_json_tick');
const ignore_json = document.getElementById('ignore_json');
const ignore_md_tick = document.getElementById('ignore_md_tick');
const ignore_md = document.getElementById('ignore_md');
const languages_translate_info = document.getElementById('languages_translate_info');
const ok_tick = "✅&nbsp;&nbsp; ";

//page functions
const initializeCycle = () => {
	// reset function that will set all elements to their default values
	last_update_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	languages_received_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	default_language_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	ignore_json_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	ignore_md_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	languages_count.innerHTML = "";
	default_language.innerHTML = "";
	ignore_json.innerHTML = "";
	ignore_md.innterHTML = "";
	languages_translate_info.style.display = "none";
	languages_translate_info.innerHTML = "";
	updateLastUpdated();
}

const updateLastUpdated = () => {
	let now = new Date().toLocaleTimeString();
	last_update_tick.innerHTML = ok_tick;
	last_update.innerHTML = now;
}

// SignalR functions
manager.hubs.realtime.connection.on('ReceiveLanguages', async (languages) => {
	languages_received_tick.innerHTML = ok_tick;
	let msg2 = "<b>" + languages.length + "</b>";
	languages_count.innerHTML = msg2;
	updateLastUpdated();
	// Process the received languages as needed
	// For example, you can update the UI or store them in a variable
});

// cycle starts
manager.hubs.realtime.connection.on('NewCycle', () => {
	initializeCycle();
});

// received settings
manager.hubs.realtime.connection.on("ReceiveTranslationSettings", (settings) => {
	default_language_tick.innerHTML = ok_tick;
	default_language.innerHTML = settings.defaultLanguage;
	ignore_json_tick.innerHTML = ok_tick;
	ignore_json.innerHTML = settings.ignoredForJson.join(", ");
	ignore_md_tick.innerHTML = ok_tick;
	ignore_md.innerHTML = settings.ignoredForMd.join(", ");
	updateLastUpdated();
});

// language names translation - status changed
manager.hub.realtime.connection.on("LanguageNameTranslationChanged", async (languageCount, translatedCount) => {
	console.log("status changed");
});

// language names translation finished
manager.hub.realtime.connection.on("LanguageNamesTranslationFinished", async () => {
	console.log("language names translation finished");
});