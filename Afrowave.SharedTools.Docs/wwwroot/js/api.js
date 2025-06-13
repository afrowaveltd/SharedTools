// Purpose: Contains the functions that are used to interact with the API.

/**
 * Represents the result of an API request.
 */
class Result {
	/**
	 * Creates a new Result instance.
	 * @param {boolean} success - Indicates if the request was successful.
	 * @param {any} [data=null] - The response data.
	 * @param {{code: number, message: string}} [error=null] - Error details if the request failed.
	 */
	constructor(success, data = null, error = null) {
		this.success = success; // Boolean: true if request succeeded
		this.data = data;       // Data: server output (JSON, text, etc.)
		this.error = error;     // Error: { code, message } in case of failure
	}
}

// API Request function
/**
 * Makes an API request with various options.
 * @param {object} options - Request options.
 * @param {string} options.url - The API URL.
 * @param {string} [options.method='GET'] - HTTP method.
 * @param {object|null} [options.data=null] - Request payload.
 * @param {object} [options.headers={}] - Request headers.
 * @param {string} [options.responseType='json'] - Expected response type ('json', 'text', 'html').
 * @param {boolean} [options.useFormData=false] - Whether to use FormData.
 * @param {string|null} [options.token=null] - Authorization token.
 * @param {string|null} [options.formName=null] - Optional form name (for FormData submission).
 * @returns {Promise<Result>} - Result object containing success flag, data, or error.
 */
const apiRequest = async ({
	url,
	method = 'GET',
	data = null,
	headers = {},
	responseType = 'json',
	useFormData = false,
	token = null,
	formName = null // Optional form name
} = {}) => {
	try {
		// Set default headers if not using FormData
		if (!useFormData && !headers['Content-Type']) {
			headers['Content-Type'] = 'application/json';
		}

		// Add Authorization header if token is provided
		if (token) {
			headers['Authorization'] = `Bearer ${token}`;
		}

		let options = {
			method,
			headers,
		};

		// Handle FormData when formName is provided
		if (useFormData && formName) {
			const formElement = document.forms[formName];
			if (!formElement) {
				return new Result(false, null, { code: 404, message: `Form with name "${formName}" not found on the page.` });
			}
			const formData = new FormData(formElement);
			options.body = formData;
		}
		// Handle FormData when an HTMLElement is passed as data
		else if (useFormData && data instanceof HTMLElement) {
			const formData = new FormData(data);
			options.body = formData;
		}
		// Handle FormData when an object is passed
		else if (useFormData && data instanceof Object) {
			const formData = new FormData();
			Object.entries(data).forEach(([key, value]) => formData.append(key, value));
			options.body = formData;
		}
		// Handle JSON and URL-encoded data
		else if (data) {
			if (headers['Content-Type'] === 'application/x-www-form-urlencoded') {
				options.body = new URLSearchParams(data).toString();
			} else {
				options.body = JSON.stringify(data);
			}
		}

		// For GET requests, append data as query parameters
		if (method.toUpperCase() === 'GET' && data) {
			const queryParams = new URLSearchParams(data).toString();
			url += `?${queryParams}`;
		}

		// Make the API call
		const response = await fetch(url, options);

		// Check if the response status is OK (200-299)
		if (!response.ok) {
			return new Result(false, null, { code: response.status, message: response.statusText });
		}

		// Parse the response based on the specified type
		let responseData;
		if (responseType === 'json') {
			responseData = await response.json();
		} else if (responseType === 'text' || responseType === 'html') {
			responseData = await response.text();
		} else {
			return new Result(false, null, { code: 500, message: `Unsupported response type: ${responseType}` });
		}

		return new Result(true, responseData, null);
	} catch (error) {
		console.error('API Request Error:', error);
		return new Result(false, null, { code: 500, message: error.message });
	}
};

/**
 * Submits a form using its name.
 * @param {string} formName - The name of the form.
 * @param {string} url - The submission URL.
 * @param {string} [method='POST'] - HTTP method.
 * @param {string|null} [token=null] - Authorization token.
 * @returns {Promise<Result>}
 */
const submitForm = async (formName, url, method = 'POST', token = null) => {
	const result = await apiRequest({ url, method, useFormData: true, formName, token });
	return result;
}

/**
 * Fetches data from the given API URL.
 * @param {string} url - API URL.
 * @param {string|null} [token=null] - Authorization token.
 * @returns {Promise<Result>}
 */
const fetchData = async (url, token = null) => {
	const result = await apiRequest({ url, token });
	return result;
}

/**
 * Fetches text from the given API URL.
 * @param {string} url - API URL.
 * @param {string|null} [token=null] - Authorization token.
 * @returns {Promise<Result>}
 */
const fetchText = async (url, token = null) => {
	const result = await apiRequest({ url, token, responseType: 'text' });
	return result;
}

/**
 * Fetches HTML from the given API URL.
 * @param {string} url - API URL.
 * @param {string|null} [token=null] - Authorization token.
 * @returns {Promise<Result>}
 */
const fetchHTML = async (url, token = null) => {
	const result = await apiRequest({ url, token, responseType: 'html' });
	return result;
}

/**
 * Fetches data with a request body.
 * @param {string} url - API URL.
 * @param {object} data - Request body.
 * @param {string} [method='POST'] - HTTP method.
 * @param {string|null} [token=null] - Authorization token.
 * @returns {Promise<Result>}
 */
const fetchDataWithBody = async (url, data, method = 'POST', token = null) => {
	const result = await apiRequest({ url, method, data, token });
	return result;
}

/**
 * Fetches data with parameters and headers.
 * @param {string} url - API URL.
 * @param {object} params - Request parameters.
 * @param {object} headers - Request headers.
 * @param {string|null} [token=null] - Authorization token.
 * @returns {Promise<Result>}
 */
const fetchDataWithParamsAndHeaders = async (url, params, headers, token = null) => {
	const result = await apiRequest({ url, data: params, headers, token });
	return result;
}

/**
 * Fetches plain text with query parameters and custom headers.
 * @param {string} url - The API endpoint.
 * @param {object} params - Query parameters.
 * @param {object} headers - Additional headers.
 * @param {string|null} [token=null] - Optional authorization token.
 * @returns {Promise<Result>} - The result containing fetched text or error.
 */
const fetchTextWithParamsAndHeaders = async (url, params, headers, token = null) => {
	const result = await apiRequest({ url, data: params, headers, token, responseType: 'text' });
	return result;
}

/**
 * Fetches HTML content with query parameters and custom headers.
 * @param {string} url - The API endpoint.
 * @param {object} params - Query parameters.
 * @param {object} headers - Additional headers.
 * @param {string|null} [token=null] - Optional authorization token.
 * @returns {Promise<Result>} - The result containing fetched HTML or error.
 */
const fetchHTMLWithParamsAndHeaders = async (url, params, headers, token = null) => {
	const result = await apiRequest({ url, data: params, headers, token, responseType: 'html' });
	return result;
}

/**
 * Deletes data at the specified API endpoint.
 * @param {string} url - The API endpoint.
 * @param {string|null} [token=null] - Optional authorization token.
 * @returns {Promise<Result>} - The result indicating success or failure.
 */
const deleteData = async (url, token = null) => {
	const result = await apiRequest({ url, method: 'DELETE', token });
	return result;
}

/**
 * Updates data at the specified API endpoint (PUT).
 * @param {string} url - The API endpoint.
 * @param {object} data - The data to update.
 * @param {string|null} [token=null] - Optional authorization token.
 * @returns {Promise<Result>} - The result indicating success or failure.
 */
const updateData = async (url, data, token = null) => {
	const result = await apiRequest({ url, method: 'PUT', data, token });
	return result;
}

/**
 * Deletes data with query parameters and custom headers.
 * @param { string } url - The API endpoint.
 * @param { object } params - Query parameters.
 * @param { object } headers - Additional headers.
 * @param { string | null } [token = null] - Optional authorization token.
 * @returns { Promise < Result >} - The result indicating success or failure.
 */
const deleteDataWithParamsAndHeaders = async (url, params, headers, token = null) => {
	const result = await apiRequest({ url, method: 'DELETE', data: params, headers, token });
	return result;
}

/**
 * Updates data with parameters and custom headers.
 * @param {string} url - The API endpoint.
 * @param {object} data - The data to update.
 * @param {object} headers - Additional headers.
 * @param {string|null} [token=null] - Optional authorization token.
 * @returns {Promise<Result>} - The result indicating success or failure.
 */
const updateDataWithParamsAndHeaders = async (url, data, headers, token = null) => {
	const result = await apiRequest({ url, method: 'PUT', data, headers, token });
	return result;
}

/**
 * Sends POST data with parameters and custom headers.
 * @param {string} url - The API endpoint.
 * @param {object} data - The data to send.
 * @param {object} headers - Additional headers.
 * @param {string|null} [token=null] - Optional authorization token.
 * @returns {Promise<Result>} - The result indicating success or failure.
 */
const postDataWithParamsAndHeaders = async (url, data, headers, token = null) => {
	const result = await apiRequest({ url, method: 'POST', data, headers, token });
	return result;
}

/**
 * Sends POST data with parameters (no headers).
 * @param {string} url - The API endpoint.
 * @param {object} data - The data to send.
 * @param {string|null} [token=null] - Optional authorization token.
 * @returns {Promise<Result>} - The result indicating success or failure.
 */
const postDataWithParams = async (url, data, token = null) => {
	const result = await apiRequest({ url, method: 'POST', data, token });
	return result;
}

/**
 * Localizes text using a language cookie or specified language.
 * Falls back to English if no language is provided.
 * @param {string} text - The text to localize.
 * @param {string} [language=""] - Optional language code.
 * @returns {Promise<string>} - Localized text.
 */
const localize = async (text, language = "") => {
	const getCookie = (cname) => {
		let name = cname + "=";
		let decodedCookie = decodeURIComponent(document.cookie);
		let ca = decodedCookie.split(';');
		for (let i = 0; i < ca.length; i++) {
			let c = ca[i];
			while (c.charAt(0) == ' ') {
				c = c.substring(1);
			}
			if (c.indexOf(name) == 0) {
				return c.substring(name.length, c.length).trim();
			}
		}
		return "";
	}
	if (language == "")
		language = getCookie("language")
	if (language = "")
		language = getCookie("Language")
	language == "" ? language = "en" : language = language;
	let res;
	try {
		let url = '/api/GetLocalized/' + text + '/' + language;
		console.log(url)
		const result = await fetch('/api/GetLocalized/' + text + '/' + language);
		res = await result.text();
		console.log(res);
	} catch (error) {
		res = text;
	}
	return res;
}

/**
 * Loads and displays help content for a given help topic.
 * Replaces the content of 'div_help' with the loaded help topic.
 * @param {string} helpTopic - The help topic identifier.
 * @returns {Promise<void>}
 */
const loadHelp = async (helpTopic) => {
	const slow = "fast";
	console.log(helpTopic)
	const element = document.getElementById('div_help');
	const link = '/Helps/' + helpTopic;
	$(element).hide(slow);

	const result = await fetch(link);
	if (result.status == 404) {
		let translation = await localize("Help topic not found");
		console.log(translation);
		element.innerHTML = translation;
		$(element).show(slow);
		return;
	}
	let txt = await result.text();

	element.innerHTML = txt;

	$(element).show(slow);
}