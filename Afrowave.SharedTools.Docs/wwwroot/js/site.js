/**
 * @file site.js
 * @description This file contains JavaScript functions for handling UI interactions, cookies, and debouncing.
 *
 */

// Icons for various statuses
const ok_tick = "✅&nbsp;&nbsp; ";
const warning_tick = "⚠️&nbsp;&nbsp; ";
const error_tick = "⛔&nbsp;&nbsp; ";
const info_tick = "ℹ️&nbsp;&nbsp; ";
const loading_tick = "⏳&nbsp;&nbsp; ";
const ignore_tick = "❌&nbsp;&nbsp; ";
const is_default_language_tick = "🌐&nbsp;&nbsp; ";

// Global variables for layout data
const layoutData = {
    topIcons: [
        { id: 1, emoji: "🔔", title: "Notifikace" },
        { id: 2, emoji: "✉️", title: "Zprávy" },
        { id: 3, emoji: "⚙️", title: "Nastavení" }
    ],
    navItems: [
        { id: 1, emoji: "🏠", text: "Domů", href: "#" },
        { id: 2, emoji: "📊", text: "Statistiky", href: "#" },
        { id: 3, emoji: "📝", text: "Dokumenty", href: "#" },
        { id: 4, emoji: "📅", text: "Kalendář", href: "#" },
        { id: 5, emoji: "👤", text: "Profil", href: "#" }
    ]
};

// sidebar navigation
const menuToggle = document.getElementById('menu-toggle');
const sideNav = document.getElementById('side-nav');
const mainContent = document.getElementById('main-content');

menuToggle.addEventListener('click', function () {
    sideNav.classList.toggle('open');
    mainContent.classList.toggle('shifted');

    // Změna ikony podle stavu menu
    const emojiSpan = this.querySelector('.emoji');
    if (sideNav.classList.contains('open')) {
        emojiSpan.textContent = '✖️';
    } else {
        emojiSpan.textContent = '📑';
    }
});

/**
 * Sets a cookie with a specified name, value, and expiration period.
 * @param {string} name - The name of the cookie.
 * @param {string} value - The value to store in the cookie.
 * @param {number} [expirationDays=36500] - The number of days before the cookie expires (default is 100 years).
 */
const setCookie = (name, value, expirationDays = 36500) => {
    const d = new Date();
    d.setTime(d.getTime() + (expirationDays * 24 * 60 * 60 * 1000));
    const expires = `expires=${d.toUTCString()}`;
    document.cookie = `${name}=${encodeURIComponent(value)};${expires};path=/`;
};

/**
 * Gets the value of a cookie with the specified name.
 * @param {string} name - The name of the cookie to retrieve.
 * @returns {string} The cookie value, or an empty string if the cookie does not exist.
 */
const getCookie = (name) => {
    const decodedCookie = decodeURIComponent(document.cookie);
    const cookies = decodedCookie.split(';');
    for (const cookie of cookies) {
        const [key, value] = cookie.trim().split('=');
        if (key === name) {
            return value || '';
        }
    }
    return '';
};

/**
 * Changes the website language by setting a language cookie and reloading the page.
 * @param {string} language - The language code to set (e.g., 'en', 'de').
 */
const changeLanguage = (language) => {
    language = language.toLowerCase();
    setCookie('language', language); // Use consistent cookie naming
    location.reload(); // Reload only if the cookie is different
};

// Apply the theme without reloading
/**
 * Applies a specified theme by setting the `data-theme` attribute on the root element and reloading the page.
 * @param {string} theme - The name of the theme (e.g., 'dark', 'light').
 */
const applyTheme = (theme) => {
    theme = theme.toLowerCase();
    document.documentElement.setAttribute('data-theme', theme);
    location.reload();
};

/**
 * Changes the website theme by setting a theme cookie and applying the theme immediately.
 * @param {string} theme - The name of the theme (e.g., 'dark', 'light').
 */
const setTheme = (theme) => {
    theme = theme.toLowerCase();
    setCookie('theme', theme); // Use consistent cookie naming
    applyTheme(theme);
};

/**
 * Creates a debounced version of a function that delays its execution until after a specified time.
 * @param {Function} func - The function to debounce.
 * @param {Function} [onError] - Optional error handling function.
 * @returns {Function} - A debounced function.
 */
const debounce = (func, onError) => {
    console.log("Debouncing");
    const delay = 800; // 0.8 second
    let timeoutId; // Stores the timer ID

    return function (...args) {
        // Clear the previous timer if it exists
        clearTimeout(timeoutId);

        // Set a new timer
        timeoutId = setTimeout(async () => {
            try {
                await func.apply(this, args);
                console.log("triggering function");
            } catch (error) {
                if (onError && typeof onError === 'function') {
                    onError(error);
                } else {
                    console.error('Error in debounced function:', error);
                }
            }
        }, delay);
    };
};

/**
 * Puts the log text to LI elements and then to UL element
 * @param {string} message
 * @returns
 */
const logToHtml = (message) => {
    const messageLines = message.split('\n');
    // Create a list of items
    const listItems = messageLines.map((line) => `<li>${line}</li>`);
    // Join the list items into a single string
    const list = listItems.join('\n');
    // create HTML content in UL element
    const html = `<ul>\n${list}\n</ul>`;
    return html;
};

/**
 * Displays a message in the specified element
 * @param {string} message
 * @param {string} elementId
 */
const showLogInElement = (message, elementId = 'log') => {
    const logElement = document.getElementById(elementId);
    if (logElement) {
        let lines = message.split('\n');
        let html = '<ul>\n';
        for (let line of lines) {
            if (line.length > 0) {
                let res = escapeHTML(line);
                html += `<li>${res}</li>\n`;
            }
        }
        html += '</ul>';
        typewriter(elementId, html, 5);
    }
};

/**
 *
 * @param {string} elementId
 * @param {string} text
 * @param {number} speed
 */
const typewriter = async (elementId, text, speed = 10) => {
    const element = document.getElementById(elementId);
    if (!element) {
        console.error(`Element with ID "${elementId}" not found.`);
        return;
    }

    element.innerHTML = ""; // Clear the content before starting
    let container = document.createElement("div");
    container.innerHTML = text; // Convert text to DOM elements

    const processNode = async (node, parent) => {
        if (node.nodeType === Node.TEXT_NODE) {
            let textContent = node.textContent;
            for (let i = 0; i < textContent.length; i++) {
                parent.append(textContent[i]);
                await new Promise(resolve => setTimeout(resolve, Math.random() * speed * 2));
            }
        } else if (node.nodeType === Node.ELEMENT_NODE) {
            let newElement = document.createElement(node.tagName);
            parent.appendChild(newElement);
            for (let child of node.childNodes) {
                await processNode(child, newElement);
            }
        }
    };

    // Ensure <ul> is created for <li> elements
    let ulElement = document.createElement("ul");
    element.appendChild(ulElement);

    for (let child of container.childNodes) {
        if (child.nodeType === Node.ELEMENT_NODE && child.tagName === "LI") {
            await processNode(child, ulElement);
        } else {
            await processNode(child, element);
        }
    }
};

const escapeHTML = (text) => {
    return text.replace(/</g, "&lt;").replace(/>/g, "&gt;");
};