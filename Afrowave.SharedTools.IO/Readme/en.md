# Afrowave.SharedTools.IO

The **Afrowave.SharedTools.IO** library provides a unified, high-level API for working with files — including reading, writing, and serializing data in multiple formats such as **JSON**, **XML**, **CSV**, and **YAML**.  
It is fully compatible with **.NET Standard 2.1** and designed to provide safe, consistent, and asynchronous file operations for desktop, web, and background applications.

This package complements **Afrowave.SharedTools.Models** by using the same unified result model:

* **Dependency Injection (DI)** service: `IFileService` / `FileService`  
* **Static helper**: `FileHelper`

---

## 📦 Contents

### Core Components

* `Models/FileType.cs` – Enumeration defining supported file formats (`Json`, `Xml`, `Csv`, `Yaml`)
* `Services/IFileService.cs` – Interface defining all read/write and serialization methods  
* `Services/FileService.cs` – Injectable DI service implementing the unified I/O logic  
* `Static/FileHelper.cs` – Static mirror of `IFileService` for quick or script-style access

---

## ⚙️ Purpose

Afrowave.SharedTools.IO simplifies file input/output by abstracting common patterns for reading and writing structured data.  
It provides a single consistent API for various file formats, async/sync access, and customizable serialization options — all using modern encoding defaults (UTF-8 without BOM).

Use it when you need to:

* Store or load data objects in JSON, XML, CSV, or YAML  
* Implement cross-format import/export utilities  
* Build CLI, web, or service applications that handle multiple file types  
* Write robust, testable file operations with `Result` and `Response<T>` wrappers  

---

## 💡 IFileService (Dependency Injection)

```csharp
public interface IFileService
{
    // Write
    Task<Result> WriteTextAsync(string filePath, string content, Encoding? encoding = null);
    Task<Result> WriteBytesAsync(string filePath, byte[] data);
    Task<Result> StoreObjectToJsonFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options);
    Task<Result> StoreObjectToXmlFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options);
    Task<Result> StoreObjectToCsvFileAsync<TData, TOptions>(TData obj, string filePath, TOptions options);
    Task<Result> StoreObjectToFileAsync<TData, TOptions>(TData obj, string filePath, FileType fileType, TOptions options);

    // Read
    Task<Response<string>> ReadTextAsync(string filePath, Encoding? encoding = null);
    Task<Response<byte[]>> ReadBytesAsync(string filePath);
    Task<Response<TData>> ReadObjectFromFileAsync<TData, TOptions>(string filePath, FileType fileType, TOptions options);
    Task<Response<TData>> ReadObjectFromJsonFileAsync<TData, TOptions>(string filePath, TOptions options);
    Task<Response<TData>> ReadObjectFromXmlFileAsync<TData, TOptions>(string filePath, TOptions options);
    Task<Response<TData>> ReadObjectFromCsvFileAsync<TData, TOptions>(string filePath, TOptions options);
    Task<Response<TData>> ReadObjectFromYamlFileAsync<TData, TOptions>(string filePath, TOptions options);
}
```

All operations are also available synchronously for performance-sensitive or legacy code.

---

## 🧠 Example Usage (Dependency Injection)

```csharp
builder.Services.AddSingleton<IFileService, FileService>();

public class ConfigManager
{
    private readonly IFileService _files;

    public ConfigManager(IFileService files) => _files = files;

    public async Task SaveConfigAsync(MySettings settings)
    {
        await _files.StoreObjectToJsonFileAsync(settings, "config.json", new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }

    public async Task<MySettings?> LoadConfigAsync()
    {
        var response = await _files.ReadObjectFromJsonFileAsync<MySettings, JsonSerializerOptions>("config.json", new());
        return response.Success ? response.Data : null;
    }
}
```

---

## ⚡ Static Helper Example

The same operations are available without DI using `FileHelper`, which wraps a singleton instance of `FileService`.

```csharp
var data = new Person { Name = "Alice", Age = 30 };

// Store as JSON
var save = FileHelper.StoreObjectToJsonFile(data, "person.json", new JsonSerializerOptions { WriteIndented = true });

// Read as object
var load = FileHelper.ReadObjectFromJsonFile<Person, JsonSerializerOptions>("person.json", new());
if (load.Success)
    Console.WriteLine($"Loaded {load.Data.Name}, {load.Data.Age}");
```

This static pattern is ideal for **scripts**, **console tools**, or **one-off utilities**.

---

## 🧩 Supported Formats

| Format | Read / Write | Default Options |
| ------- | ------------- | ---------------- |
| **JSON** | ✅ / ✅ | `camelCase`, `Indented`, `IgnoreNullValues` |
| **XML**  | ✅ / ✅ | Indented output, UTF-8 (no BOM) |
| **CSV**  | ✅ / ✅ | Comma delimiter, header record, Invariant culture |
| **YAML** | ✅ / ✅ | CamelCase naming, omit nulls |

Each format accepts **native serializer options**, e.g.:

* `JsonSerializerOptions` for JSON  
* `XmlWriterSettings` or `XmlReaderSettings` for XML  
* `CsvConfiguration` for CSV  
* `ISerializer` / `IDeserializer` for YAML  

---

## 🚀 Key Features

| Feature                       | Description                                                                 |
| ------------------------------ | --------------------------------------------------------------------------- |
| **Multi-format serialization** | JSON, XML, CSV, and YAML out of the box                                    |
| **Async & sync support**       | All read/write methods available in both forms                             |
| **Unified result model**       | Uses `Result` and `Response<T>` from SharedTools.Models                     |
| **Automatic directory creation** | Ensures output directories exist before writing                            |
| **Encoding control**           | Default UTF-8 (no BOM), optional override                                  |
| **Type-safe generic API**      | Uses generics for strong typing and flexibility                             |
| **Dependency-free design**     | Minimal dependencies, lightweight for embedded or CLI tools                |

---

## 🧱 Design Principles

* Clear separation between **DI** and **Static** usage  
* Full **async/await** support for scalability  
* Safe defaults and unified error handling  
* Supports object and raw file operations  
* Follows Afrowave naming and modular conventions  

---

## 📦 Version 0.0.1 Release Notes

| Version | Changes |
| -------- | -------- |
| **0.0.1** | Initial release of **Afrowave.SharedTools.IO**.

Includes:

  • `IFileService` / `FileService` (DI)<br>
  • `FileHelper` (static)<br>
  • Unified async/sync API for JSON, XML, CSV, and YAML<br>
  • Integrated result model (`Result`, `Response<T>`) |

---

✍️ This file is part of the multilingual documentation system.  
Translations will be automatically handled by **LangHub**.
