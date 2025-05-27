# Afrowave.MockLocalizationBackend

This is a mock implementation of the `ILocalizationBackend` interface designed for testing and educational purposes within the Afrowave.Localization system.

It is part of the broader Afrowave.SharedTools.Localization ecosystem, and serves as a reference backend for developers who want to:

- Test localization resolution logic
- Simulate edge cases (failures, warnings, missing keys, etc.)
- Build their own custom backend packages
- Understand how localization chaining and fallback works

---

## 📦 Features

| Feature                        | Description |
|-------------------------------|-------------|
| ✅ In-memory data             | Simulates language dictionaries using dictionaries per language (`Dictionary<string, Dictionary<string, string>>`) |
| ✅ Configurable outcomes      | Can simulate success, warnings, failures, and missing values |
| ✅ Error simulation           | Can throw exceptions to test resilience in chaining |
| ✅ Call tracking              | Counts number of `GetValueAsync` invocations |
| ✅ Fully implements ILocalizationBackend | Compatible with all Afrowave localization systems |
| ✅ Ideal for unit tests       | Lightweight and deterministic |
| ✅ Readable fallback behavior | Returns key as fallback if configured (via `ChainedLocalizationBackend`) |

---

## 🚀 Quick Start

```csharp
var mock = new MockLocalizationBackend();

mock.SetValue("en", "save", "Save");
mock.SetValue("cs", "save", "Uložit");

var response = await mock.GetValueAsync("cs", "save");
// response.Data == "Uložit"
```

---

## 🔧 Behavior Flags

You can configure the mock backend to simulate different conditions.

### `SimulateWarning`
If set to `true`, returned values will include `Warning = true`.

```csharp
mock.SimulateWarning = true;
```

---

### `SimulateEmptySuccess`
Returns a successful response with an empty string (not a failure).

```csharp
mock.SimulateEmptySuccess = true;
// Simulates: translation key exists, but no value defined yet
```

---

### `ThrowOnGet`
Simulates a runtime failure in the backend.

```csharp
mock.ThrowOnGet = true;
// Throws InvalidOperationException on every GetValueAsync
```

---

### `CallCount`
Tracks the number of times `GetValueAsync` was called.

```csharp
var count = mock.CallCount;
```

---

## 🧪 Integration in Tests

This backend is ideal for test-driven development:

```csharp
var mock = new MockLocalizationBackend();
mock.SetValue("en", "exit", "Exit");

var chain = new ChainedLocalizationBackend(mock);
var result = await chain.GetValueAsync("cs", "exit");

Assert.Equal("exit", result.Data); // fallback to key
Assert.True(result.Warning);
```

---

## 🧠 Architectural Notes

- The mock backend fully implements `ILocalizationBackend`, and mimics all capabilities (`CanRead`, `CanWrite`, `CanBulkRead`, etc.)
- It does not perform any file or network I/O
- It is designed to behave predictably in order to simulate real-world scenarios without requiring full backend setup
- It can be extended to simulate backend-specific metadata or to preload mock dictionaries

---

## 🌍 Community Use

This backend is provided as a **reference implementation**. If you want to create your own custom backend (e.g. SQLite, Firebase, REST API), you are encouraged to:

- Explore this mock as an example of a fully implemented and testable backend
- Create your own NuGet package using the same structure
- Add documentation in the `Docs/Readme` folder for consistency

---

## 📁 Folder Structure (Recommended)

```
/Afrowave.SharedTools.Localization.Backend.Mock/
│
├── MockLocalizationBackend.cs
├── Docs/
│   └── Readme/
│       └── en.md          ← You're here
├── Tests/                 ← Optional: Separate test project
│   └── ...
```

---

## 🤝 Contributing

To contribute your own backend implementation:

1. Create a new .NET Standard 2.1 class library
2. Reference `Afrowave.SharedTools.Localization`
3. Implement `ILocalizationBackend`
4. Add a `Docs/Readme/en.md` file with a clear description
5. (Optional) Submit your backend to Afrowave as a community-supported plugin

---

## ❤️ About

This backend is maintained by the Afrowave team and contributors.  
It is intended for learning, testing, and enabling others to build robust localization systems with clarity and joy.
