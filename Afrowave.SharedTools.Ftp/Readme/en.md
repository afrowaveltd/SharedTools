# Afrowave.SharedTools.Ftp

The **Afrowave.SharedTools.Ftp** library provides a universal, high-level API for performing common FTP operations such as uploading, downloading, listing, and deleting files or directories. It is fully compatible with **.NET Standard 2.1**, and designed as a lightweight, dependency-free utility that can be used in desktop, web, and service environments.

This package complements **Afrowave.SharedTools.Api** by providing the same consistent interface style and dual access pattern:

* **Dependency Injection (DI)** service: `IFtpService` / `FtpService`
* **Static helper**: `FtpClientHelper`

---

## 📦 Contents

### Core Options & Configuration

* `Options/FtpOptions.cs` – Defines FTP connection parameters (host, credentials, SSL, retry, etc.)
* `Options/RetryPolicyOptions.cs` – Exponential backoff retry configuration with optional jitter

### Service Layer (Dependency Injection)

* `Interfaces/IFtpService.cs` – Defines unified async API for all FTP operations
* `Services/FtpService.cs` – Injectable service implementing all core functionality, using `FtpWebRequest` under the hood

### Static Helper Layer

* `Static/FtpClientHelper.cs` – Static version mirroring the same API as `FtpService`, ideal for quick scripting or testing without DI

---

## ⚙️ Purpose

Afrowave.SharedTools.Ftp provides an abstraction over the low-level .NET FTP APIs (`FtpWebRequest`, `FtpWebResponse`), making file operations simple, consistent, and error-safe. It introduces retry logic, unified response objects, and async support even in .NET Standard 2.1.

This module is ideal for:

* Automated synchronization of translation dictionaries or localization files
* CI/CD and DevOps tools for remote file management
* Lightweight integration with shared file servers

---

## 🧩 FtpOptions Example

```csharp
var options = new FtpOptions
{
    Host = "ftp.example.com",
    Port = 21,
    Credentials = new NetworkCredential("user", "password"),
    EnableSsl = false,
    UsePassive = true,
    Retry = new RetryPolicyOptions
    {
        MaxRetries = 3,
        BaseDelay = TimeSpan.FromMilliseconds(500),
        BackoffFactor = 2.0,
        Jitter = true
    }
};
```

You can inject these options via **DI**, **appsettings.json**, or by directly passing them to the `FtpService` constructor.

---

## 💡 IFtpService (Dependency Injection)

```csharp
public interface IFtpService
{
    Task<Response<IReadOnlyList<string>>> ListAsync(string remotePath);
    Task<Response<byte[]>> DownloadBytesAsync(string remotePath);
    Task<Result> DownloadToFileAsync(string remotePath, string localFilePath);
    Task<Result> UploadBytesAsync(string remotePath, byte[] data, bool overwrite = true);
    Task<Result> UploadFileAsync(string localFilePath, string remotePath, bool overwrite = true);
    Task<Result> DeleteFileAsync(string remotePath);
    Task<Result> CreateDirectoryAsync(string remotePath);
    Task<Result> DeleteDirectoryAsync(string remotePath);
    Task<Response<long>> GetFileSizeAsync(string remotePath);
    Task<Response<DateTime>> GetModifiedTimeAsync(string remotePath);
    Task<Response<bool>> ExistsAsync(string remotePath);
    Task<Result> RenameAsync(string remotePath, string newName);
}
```

All methods return `Result` or `Response<T>` from the shared **Afrowave.SharedTools.Models** package for consistency.

---

## 🧠 Example Usage (DI)

```csharp
builder.Services.AddConfiguredService<IFtpService, FtpService, FtpOptions>(cfg =>
{
    cfg.Host = "ftp.example.com";
    cfg.Credentials = new NetworkCredential("demo", "demo123");
    cfg.UsePassive = true;
    cfg.EnableSsl = false;
});

// Later in your service or controller:

public class FtpDemoService
{
    private readonly IFtpService _ftp;

    public FtpDemoService(IFtpService ftp) => _ftp = ftp;

    public async Task UploadAsync()
    {
        var result = await _ftp.UploadFileAsync("local.txt", "/upload/local.txt");
        if (!result.Success) Console.WriteLine($"Failed: {result.Message}");
    }
}
```

---

## ⚡ Static Helper Example

If you prefer not to use DI, the same operations are available via the static helper:

```csharp
var result = await FtpClientHelper.UploadFileAsync(options, "local.txt", "/remote/local.txt");
if (result.Success)
    Console.WriteLine("File uploaded.");
```

All static methods take an instance of `FtpOptions` as their first argument.

---

## 🚀 Key Features

| Feature                     | Description                                                  |
| --------------------------- | ------------------------------------------------------------ |
| **.NET Standard 2.1**       | Works across desktop, web, and service apps                  |
| **Async/await support**     | Async wrapping around `FtpWebRequest` for smooth concurrency |
| **Retry logic**             | Configurable exponential retry with optional jitter          |
| **Unified result model**    | Uses `Result` and `Response<T>` from SharedTools.Models      |
| **Stateless static helper** | For testing, scripting, and quick tools                      |
| **Supports FTPS**           | Optional SSL/TLS (explicit mode)                             |
| **Binary + passive mode**   | Defaults to modern secure FTP setup                          |

---

## 🧱 Design Principles

* Type-safe configuration (`FtpOptions` / `RetryPolicyOptions`)
* Separation of static vs DI usage
* Full async support in .NET Standard 2.1
* No external dependencies
* Clear and consistent Afrowave naming conventions

---

## 📦 Version 0.0.1 Release Notes

| Version   | Changes                                                                                                                                                                                                                                                    |
| --------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **0.0.1** | Initial release of **Afrowave.SharedTools.Ftp**.<br>Includes: <br>• `IFtpService` / `FtpService` (DI version)<br>• `FtpClientHelper` (static version)<br>• `FtpOptions` and `RetryPolicyOptions`<br>• Async file and directory operations with retry logic |

---

✍️ This file is part of the multilingual documentation system.
Translations will be automatically handled by **LangHub**.
