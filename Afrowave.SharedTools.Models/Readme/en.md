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

## 🧭 Structure

All model classes follow clear, consistent naming and XML documentation. They are compatible with .NET Standard 2.1 and intended for use across console, web, and library layers.

---

✍️ This file is part of the multilingual documentation system. Translations will be managed by LangHub.
