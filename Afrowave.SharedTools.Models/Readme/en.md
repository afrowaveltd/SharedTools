# Afrowave.SharedTools.Models

This module contains standardized models and data wrappers used across Afrowave projects. These structures are intended to simplify the design of method results, API communication, and metadata encapsulation.

---

## 📦 Contents

* `Results/Result.cs` – A minimal boolean result wrapper
* `Results/Response<T>.cs` – A generic response container with data, message, and flags

---

## ✅ Result

A simple result type for indicating success or failure without returning data.

```csharp
var result = Result.Ok("Operation succeeded");
if (!result.Success) Console.WriteLine(result.Message);
```

**Main members:**

* `bool Success`
* `string Message`
* `Result.Ok(string message)` – success with message
* `Result.Fail(string message)` – failure with message

---

## ✅ Response<T>

A standardized generic response class for wrapping data with result metadata.

```csharp
var response = Response<User>.SuccessResponse(user, "User loaded successfully");
if (response.Success) Display(response.Data);
```

**Main members:**

* `bool Success`
* `bool Warning`
* `string Message`
* `T Data`

**Factory methods:**

* `SuccessResponse(T data, string message)`
* `Fail(string message)`
* `EmptySuccess()`
* `SuccessWithWarning(T data, string warningMessage)`
* `Fail(Exception ex)`

---

## 🌐 LibreTranslate Models

Models for integration with the LibreTranslate API.

### Settings

**LibreServer**  
Configuration for LibreTranslate server endpoints and authentication.  
*Members:*  
- `string ApiKey` – API key for authentication  
- `string Host` – Server host or IP  
- `string DetectLanguageEndpoint` – Endpoint for language detection  
- `string LanguagesEndpoint` – Endpoint for supported languages  
- `string TranslateEndpoint` – Endpoint for text translation  
- `string TranslateFileEndpoint` – Endpoint for file translation  
- `bool NeedsKey` – Indicates if API key is required

### Requests

**Translate**  
Request for translating text.  
*Members:*  
- `string Q` – Text to translate  
- `string Source` – Source language code  
- `string Target` – Target language code  
- `string Format` – Output format  
- `int Alternatives` – Number of alternatives  
- `string? Api_key` – API key

**TranslateFile**  
Request for translating a file.  
*Members:*  
- `IFormFile File` – File to translate  
- `string Source` – Source language code  
- `string Target` – Target language code  
- `string Api_key` – API key

**DetectLanguage**  
Request for detecting the language of a text.  
*Members:*  
- `string Q` – Text to analyze  
- `string Api_key` – API key

### Responses

**Detections**  
Result of language detection.  
*Members:*  
- `string Language` – Detected language code  
- `int Confidence` – Confidence score

**Translate**  
Result of a text translation.  
*Members:*  
- `string TranslatedText` – Translated text  
- `Detections DetectedLanguage` – Detected source language  
- `List<string> Alternatives` – Alternative translations

**TranslateFile**  
Result of a file translation.  
*Members:*  
- `string TranslatedFileUrl` – URL to the translated file

**ErrorResponse**  
Error details from LibreTranslate API.  
*Members:*  
- `string Error` – Error message

**LibreLanguage**  
Information about a supported language.  
*Members:*  
- `string Code` – Language code  
- `string Name` – Language name  
- `List<string> Targets` – Supported target languages

---

## 🧭 Structure

All model classes follow clear, consistent naming and XML documentation. They are compatible with .NET Standard 2.1 and intended for use across console, web, and library layers.

---

✍️ This file is part of the multilingual documentation system. Translations will be managed by LangHub.
