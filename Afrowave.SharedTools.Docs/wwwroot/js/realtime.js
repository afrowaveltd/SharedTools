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
const language_names_translation_progress = document.getElementById("language_names_translation_progress");
const language_translation_finished_tick = document.getElementById("language_translation_finished_tick");
const language_translation_done = document.getElementById("language_translation_done");
const language_count_nr = document.getElementById("language_count_nr");
const language_names_translation_error = document.getElementById('language_names_translation_error');
const language_names_translation_error_count = document.getElementById('language_names_translation_error_count');
const segmentedProgressElement = document.getElementById("language_segmented_progress");
const languageProgressText = document.getElementById("language_progress_text");

let totalLanguageCount = 1;
let successCount = 0;
let errorCount = 0;

//page functions
const initializeCycle = () => {
	// reset function that will set all elements to their default values
	last_update_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	languages_received_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	default_language_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	ignore_json_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	ignore_md_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	languages_count.innerHTML = "";
	languages_count_nr.innerHTML = "0";
	language_translation_done.innerHTML = "0";
	default_language.innerHTML = "";
	ignore_json.innerHTML = "";
	ignore_md.innerHTML = "";
	languages_translate_info.style.display = "none";
	language_names_translation_progress.value = 0;
	language_names_translation_progress.max = 1;
	language_translation_finished_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	language_names_translation_error.style.display = 'none';
	language_names_translation_error.style.width = '0px';
	language_names_translation_error_count.innerHTML = '';
	segmentedProgressElement.innerHTML = '';
	successSegment.width = 0;
	errorSegment.width = 0;
	languageProgressText.innerHTML = '';
	totalLanguageCount = 1;
	successCount = 0;
	errorCount = 0;
	segmentedProgressElement.classList.remove("completed");
	updateLastUpdated();
}

async function updateSegmentedProgressBar() {
	const successPercent = (successCount / totalLanguageCount) * 100;
	const errorPercent = (errorCount / totalLanguageCount) * 100;

	segmentedProgressElement.innerHTML = '';

	if (successCount > 0) {
		const successSegment = document.createElement('div');
		successSegment.className = 'segment success';
		successSegment.style.width = `${successPercent}%`;
		segmentedProgressElement.appendChild(successSegment);
	}

	if (errorCount > 0) {
		const errorSegment = document.createElement('div');
		errorSegment.className = 'segment error';
		errorSegment.style.width = `${errorPercent}%`;
		segmentedProgressElement.appendChild(errorSegment);
	}
	let successText = await localize("Successful") + ":";
	let errorText = await localize("Errors") + ":";
	languageProgressText.innerHTML = `${successText} ${successCount} / ${totalLanguageCount}, ${errorText} ${errorCount}`;
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
	languages_translate_info.style.display = 'block';
	language_names_translation_progress.max = languages.length;
	language_count_nr.innerHTML = languages.length;
	segmentedProgressElement.classList.remove("completed");
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
manager.hubs.realtime.connection.on("LanguageNameTranslationChanged", async (languageCount, translatedCount) => {
	totalLanguageCount = languageCount;
	successCount = translatedCount;
	updateSegmentedProgressBar();
	if (successCount + errorCount === totalLanguageCount)
		segmentedProgressElement.classList.add("completed");
	else try {
		segmentedProgressElement.classList.remove("completed");
	} catch { };
	language_translation_done.innerHTML = translatedCount; // zůstává pro starý text
	updateLastUpdated();
});

// language names translation - error
manager.hubs.realtime.connection.on("LanguageNameTranslationError", (errorCountValue) => {
	errorCount = errorCountValue;
	updateSegmentedProgressBar();
	if (successCount + errorCount === totalLanguageCount)
		segmentedProgressElement.classList.add("completed");
	else try {
		segmentedProgressElement.classList.remove("completed");
	} catch { };
	language_names_translation_error.style.display = 'inline-block';
	language_names_translation_error_count.innerHTML = errorCountValue;
	updateLastUpdated();
});

// language names translation finished
manager.hubs.realtime.connection.on("LanguageNamesTranslationFinished", async () => {
	if (successCount + errorCount === totalLanguageCount)
		segmentedProgressElement.classList.add("completed");
	updateLastUpdated();
	console.log("language names translation finished");
});