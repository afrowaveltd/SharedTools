# Afrowave.SharedTools.Models

This module contains **foundational, framework-agnostic models** used across the Afrowave ecosystem.  
Its primary goal is to provide **simple, consistent, and developer-friendly primitives** for:

- method results
- data responses
- request/command envelopes
- metadata transport

All models are compatible with **.NET Standard 2.1** and are designed to scale from small utilities to distributed, plugin-based systems.

---

## 📦 Contents

### Results

- `Results/Result.cs` – Boolean result wrapper (no payload)
- `Results/Response.cs` – Generic and non-generic response wrappers
- `Results/Unit.cs` – Explicit "no data" value for generic responses

### Communication

- `Communication/Request.cs` – Request and Request<T> envelopes

### Other domains (unchanged)

- LibreTranslate models (settings, requests, responses)
- Localization models (Country, Language)

---

## ✅ Result

`Result` represents the outcome of an operation **without returning data**.

```csharp
var result = Result.Ok("Operation succeeded");

if (!result.Success)
{
    Console.WriteLine(result.Message);
}
```

### Members

- `bool Success`
- `bool Warning`
- `string Message`

### Factory methods

- `Result.Ok()`
- `Result.Ok(string message)`
- `Result.OkWithWarning(string message)`
- `Result.Fail()`
- `Result.Fail(string message)`
- `Result.Fail(Exception ex)`

> Result and Response intentionally share the same factory syntax.

---

## ✅ Response / Response<T>

`Response` and `Response<T>` represent standardized operation responses **with optional payload**.

### Non-generic Response

Use when no data needs to be returned:

```csharp
return Response.Ok("Saved successfully");
return Response.Fail("Invalid input");
```

### Generic Response<T>

Use when an operation may return data:

```csharp
var response = Response<User>.Ok(user, "User loaded");

if (response.Success && response.HasData)
{
    Display(response.Data);
}
```

### Key properties

- `bool Success`
- `bool Warning`
- `string Message`
- `T Data`
- `bool HasData` – indicates whether `Data` was explicitly provided

### Factory methods (unified)

- `Ok()`
- `Ok(string message)`
- `Ok(T data)`
- `Ok(T data, string message)`
- `OkWithWarning(string message)`
- `OkWithWarning(T data, string message)`
- `Fail()`
- `Fail(string message)`
- `Fail(Exception ex)`

### Backward-compatible methods

Existing APIs remain supported:

- `SuccessResponse(T data, string message)`
- `SuccessWithWarning(T data, string warningMessage)`
- `EmptySuccess()`

---

## 🧩 Unit

`Unit` represents an explicit **"no payload"** value for generic responses.

```csharp
return Response<Unit>.Ok();
return Response<Unit>.Fail("Not allowed");
```

This avoids forcing placeholder values when `T` is not meaningful.

---

## 📩 Request / Request<T>

Request models provide a lightweight, framework-independent **command/query envelope**.
They are suitable for:

- API calls
- plugin dispatchers
- internal messaging
- background workers

### Request (non-generic)

```csharp
var request = Request.Create("localization.resolve");
```

### Request<T> (with payload)

```csharp
var request = Request<string>.Create(
    action: "docs.translate",
    body: "Hello world",
    sender: "Client",
    target: "Translator"
);
```

### Common properties

- `Guid Id` – correlation identifier
- `DateTimeOffset TimestampUtc`
- `string Action` – operation identifier
- `string? Sender`
- `string? Target`
- `Dictionary<string,string>? Meta`

### Generic-only properties

- `T Body`
- `bool HasBody`

### Factory methods

- `Request.Create(...)`
- `Request<T>.Create(action, body, ...)`
- `Request<T>.CreateNoBody(...)`

---

## 🌍 LibreTranslate Models

Models for integration with the LibreTranslate API (settings, requests, responses).
These remain unchanged and coexist with the shared Result / Response infrastructure.

---

## 🌐 Localization Models

Metadata models for countries and languages used in localization, UI selection, and formatting.

---

## 🧭 Design principles

- ✔ .NET Standard 2.1 compatible
- ✔ No framework dependencies
- ✔ Nullable-friendly without forcing newer runtimes
- ✔ Unified syntax across Result / Response / Request
- ✔ Safe defaults, explicit intent (`HasData`, `HasBody`)

---

✍️ This documentation is part of the Afrowave multilingual docs system.  
Translations are generated and maintained via LangHub.