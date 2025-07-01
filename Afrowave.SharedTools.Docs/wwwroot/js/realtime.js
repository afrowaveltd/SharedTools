// ===================== Page Elements =====================
// DOM elements used for displaying real-time translation and statistics
const basic_statistics_element = document.getElementById('basic_statistics');
const default_language = document.getElementById('default_language');
const default_language_tick = document.getElementById('default_language_tick');
const ignore_json = document.getElementById('ignore_json');
const ignore_json_tick = document.getElementById('ignore_json_tick');
const ignore_md = document.getElementById('ignore_md');
const ignore_md_tick = document.getElementById('ignore_md_tick');
const json_table_body = document.getElementById('json_table_body');
const language_count_nr = document.getElementById("language_count_nr");
const language_names_translation_error = document.getElementById('language_names_translation_error');
const language_names_translation_error_count = document.getElementById('language_names_translation_error_count');
const language_names_translation_progress = document.getElementById("language_names_translation_progress");
const language_translation_done = document.getElementById("language_translation_done");
const language_translation_finished_tick = document.getElementById("language_translation_finished_tick");
const languageProgressText = document.getElementById("language_progress_text");
const languages_count = document.getElementById('languages_count');
const languages_received_tick = document.getElementById('languages_received_tick');
const languages_translate_info = document.getElementById('languages_translate_info');
const last_update = document.getElementById('last_update');
const last_update_tick = document.getElementById('last_update_tick');
const segmentedProgressElement = document.getElementById("language_segmented_progress");

// ===================== State Variables =====================
// Track total, successful, and error translation counts
let totalLanguageCount = 1;
let successCount = 0;
let errorCount = 0;
let isEnglishDefault = true; // Track if English is the default language

// ===================== Page Functions =====================

/**
 * Resets all UI elements and state variables to their default values.
 * Called at the start of a new translation cycle.
 */
const initializeCycle = () => {
	last_update_tick.innerHTML = "&nbsp;&nbsp;&nbsp;&nbsp;";
	last_update.innerHTML = ""; // Reset last update time
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
	languageProgressText.innerHTML = '';
	totalLanguageCount = 1;
	successCount = 0;
	errorCount = 0;
	segmentedProgressElement.classList.remove("completed");
	json_table_body.innerHTML = ''; // Clear the JSON table body
	updateLastUpdated();
}

/**
 * Updates the segmented progress bar to reflect the number of successful and error translations.
 * Also updates the progress text with localized labels.
 */
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

/**
 * After receiving set of languages and translator settings, we have all to generate JSON translation progress table
 */

const generateJsonTranslationProgressTable = async (languages) => {
	console.log(languages);
	json_table_body.innerHTML = ''; // Clear existing table body
	for (let i = 0; i < languages.length; i++) {
		let language = languages[i];
		console.log(language);
		let row = document.createElement('tr');
		row.innerHTML = `
			<td>${await localize(language.name)}</td>
			<td class="text-center" id="${language.code}_json_summa">0</td>
			<td class="text-center" id="${language.code}_json_toAdd">0</td>
			<td class="text-center" id="${language.code}_json_toRemove">0</td>
			<td class="text-center" id="${language.code}_json_toUpdate">0</td>
			<td><span class="multi-progress" id="${language.code}_json_progress"></span></td>
			<td class="text-center" id="${language.code}_json_status">${language.code == trans ? "🟡" : "🔴"}</td>
		`;
		json_table_body.innerHTML += row;
	}
	updateLastUpdated();
}

/**
 * Updates the last updated time and sets the tick icon.
 */
const updateLastUpdated = () => {
	let now = new Date().toLocaleTimeString();
	last_update_tick.innerHTML = ok_tick;
	last_update.innerHTML = now;
}

// ===================== SignalR Event Handlers =====================

/**
 * Handles the 'ReceiveLanguages' event from SignalR.
 * Updates UI with the received languages and resets progress bar.
 * @param {Array} languages - Array of received language objects or codes.
 */
manager.hubs.realtime.connection.on('ReceiveLanguages', async (languages) => {
	languages_received_tick.innerHTML = ok_tick;
	let msg2 = "<b>" + languages.length + "</b>";
	languages_count.innerHTML = msg2;
	if (!isEnglishDefault) {
		languages_translate_info.style.display = 'block';
		language_names_translation_progress.max = languages.length;
		language_count_nr.innerHTML = languages.length;
		segmentedProgressElement.classList.remove("completed");
	} else {
		languages_translate_info.style.display = 'none';
	}
	await generateJsonTranslationProgressTable(languages);
	updateLastUpdated();
	// Process the received languages as needed
	// For example, you can update the UI or store them in a variable
});

/**
 * Handles the 'NewCycle' event from SignalR.
 * Resets the UI for a new translation cycle.
 */
manager.hubs.realtime.connection.on('NewCycle', () => {
	initializeCycle();
});

/**
 * Handles the 'ReceiveTranslationSettings' event from SignalR.
 * Updates UI with translation settings such as default language and ignored languages.
 * @param {Object} settings - Settings object containing defaultLanguage, ignoredForJson, ignoredForMd.
 */
manager.hubs.realtime.connection.on("ReceiveTranslationSettings", (settings) => {
	default_language_tick.innerHTML = ok_tick;
	default_language.innerHTML = settings.defaultLanguage;
	ignore_json_tick.innerHTML = ok_tick;
	ignore_json.innerHTML = settings.ignoredForJson.join(", ");
	ignore_md_tick.innerHTML = ok_tick;
	ignore_md.innerHTML = settings.ignoredForMd.join(", ");
	isEnglishDefault = settings.defaultLanguage == 'en';
	updateLastUpdated();
});

/**
 * Handles the 'LanguageNameTranslationChanged' event from SignalR.
 * Updates progress bar and UI with the number of translated languages.
 * @param {number} languageCount - Total number of languages.
 * @param {number} translatedCount - Number of successfully translated languages.
 */
manager.hubs.realtime.connection.on("LanguageNameTranslationChanged", async (languageCount, translatedCount) => {
	totalLanguageCount = languageCount;
	successCount = translatedCount;
	updateSegmentedProgressBar();
	if (successCount + errorCount === totalLanguageCount)
		segmentedProgressElement.classList.add("completed");
	else try {
		segmentedProgressElement.classList.remove("completed");
	} catch { };
	language_translation_done.innerHTML = translatedCount; // remains for legacy text
	updateLastUpdated();
});

/**
 * Handles the 'LanguageNameTranslationError' event from SignalR.
 * Updates error count and progress bar, and displays error info in the UI.
 * @param {number} errorCountValue - Number of translation errors.
 */
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

/**
 * Handles the 'LanguageNamesTranslationFinished' event from SignalR.
 * Marks the progress bar as completed and updates the last updated time.
 */
manager.hubs.realtime.connection.on("LanguageNamesTranslationFinished", async () => {
	if (successCount + errorCount === totalLanguageCount)
		segmentedProgressElement.classList.add("completed");
	updateLastUpdated();
	console.log("language names translation finished");
});