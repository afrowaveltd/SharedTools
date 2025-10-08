# Afrowave.SharedTools.Api

This module provides **universal HTTP, API, and web interaction utilities** for Afrowave projects.
It includes both **static** and **dependency-injectable (DI)** variants for handling HTTP requests, cookies, and related web behaviors.
All components are fully compatible with **.NET Standard 2.1** and can be used in desktop, web, and service environments.

---

## 📦 Contents

### Networking Core

* `Models/HttpRequestOptions.cs` – Configuration for timeout, proxy, and retry policy
* `Models/RetryPolicyOptions.cs` – Exponential retry with optional jitter
* `Interfaces/IHttpService.cs` – Unified API interface for async HTTP operations
* `DI/HttpService.cs` – Injectable service implementing the interface
* `Static/HttpClientHelper.cs` – Lightweight static helper (no DI required)
* `Serialization/JsonOptions.cs` – Preconfigured JSON serialization settings

### Web & Cookie Utilities *(added in 0.0.3)*

* `Options/CookieSettings.cs` – Defines domain, path, SameSite, and security policies with full enum-based configuration
* `Services/ICookieService.cs` – Injectable cookie management interface for reading, writing, updating, and deleting cookies
* `Services/CookieService.cs` – DI implementation supporting named profiles and long-lived cookies
* `Static/CookieHelper.cs` – Static helper version for cookie operations without DI (ideal for controllers, scripts, or testing)

---

## ⚙️ Purpose

Afrowave.SharedTools.Api acts as the **foundation for all HTTP, API, and web-based operations** across Afrowave tools.
It standardizes network access, proxy behavior, retry policies, and now includes high-level cookie management to ensure consistent, configurable, and secure session handling.

---

## ✅ IHttpService (DI)

A fully injectable HTTP service built around `IHttpClientFactory`.
Supports JSON, XML, text, and binary transfers with automatic retry logic.

```csharp
public class MyClient
{
    private readonly IHttpService _http;

    public MyClient(IHttpService http)
    {
        _http = http;
    }

    public async Task<Response<MyData>> GetUserAsync()
    {
        return await _http.GetJsonAsync<MyData>("https://api.example.com/user");
    }
}
```

**Main methods:**

* `GetJsonAsync<T>()`
* `PostJsonAsync<TReq, TRes>()`
* `GetStringAsync()`
* `GetBytesAsync()`
* `GetXmlAsync<T>()`
* `PostFormUrlEncodedAsync()`
* `GetJsonManyAsync<T>()` (parallel requests)

**Return type:**
All methods return `Response<T>` from **Afrowave.SharedTools.Models**, providing consistent success, data, and error metadata.

---

## 🍪 Cookie Management (added in v0.0.3)

Starting with version **0.0.3**, Afrowave.SharedTools.Api introduces a robust cookie management subsystem that simplifies HTTP cookie handling for ASP.NET Core and similar environments.

### `CookieSettings`

Defines the behavior and security policies for all cookies.

```csharp
public sealed class CookieSettings
{
    public string Domain { get; set; }
    public string Path { get; set; } = "/";
    public int ExpiryInDays { get; set; } = 30;
    public bool HttpOnly { get; set; } = true;
    public bool Secure { get; set; } = true;
    public bool IsEssential { get; set; } = true;
    public SameSitePolicy SameSite { get; set; } = SameSitePolicy.Lax;
    public CookieSecurePolicy SecurePolicy { get; set; } = CookieSecurePolicy.Always;
}
```

**SameSitePolicy Enum:**

* `None` – Send cookie in all contexts (cross-site allowed)
* `Lax` – Default modern behavior (safe for most cases)
* `Strict` – Cookie only sent in first-party contexts

The settings can be bound via configuration or named options to define multiple cookie policies (e.g., for session, analytics, or consent).

---

### `ICookieService` and `CookieService` (DI)

Dependency-injectable cookie manager using `IOptionsMonitor<CookieSettings>` for per-profile configurations.
Supports automatic option binding and long-lived (≈20 years) cookie lifetimes.

**Example:**

```csharp
builder.Services.AddHttpContextAccessor();

builder.Services.AddConfiguredService<ICookieService, CookieService, CookieSettings>(options =>
{
    options.Domain = ".afrowave.ltd";
    options.SameSite = CookieSettings.SameSitePolicy.Lax;
    options.ExpiryInDays = 30;
    options.HttpOnly = true;
});
```

**Usage in controllers:**

```csharp
public class LocaleController : ControllerBase
{
    private readonly ICookieService _cookies;

    public LocaleController(ICookieService cookies) => _cookies = cookies;

    [HttpPost("/lang")]
    public IActionResult SetLang(string code)
    {
        var res = _cookies.Update("lang", code);
        return res.Success ? Ok() : BadRequest(res.Message);
    }

    [HttpGet("/lang")]
    public IActionResult GetLang()
    {
        var val = _cookies.ReadOrCreate("lang", "en");
        return Ok(val);
    }
}
```

**Key methods:**

* `Write(name, value)` – writes cookie only if it does not exist
* `Update(name, value)` – creates or overwrites cookie
* `Read(name)` – returns cookie value or null
* `ReadOrCreate(name, default)` – returns cookie or creates one with the default value
* `Delete(name)` – removes cookie with matching policy
* JSON helpers: `UpdateObject<T>()`, `ReadObjectOrCreate<T>()`

---

### `CookieHelper` (Static)

A fully static version of the same functionality, usable without DI. Ideal for quick use in middleware, test utilities, or lightweight apps.

```csharp
var val = CookieHelper.ReadOrCreate(HttpContext, "theme", "dark");
CookieHelper.Update(HttpContext, "session", Guid.NewGuid().ToString());
```

Methods mirror the DI version, taking `HttpContext` as the first argument.

---

## 🌐 Example Integration (combined)

```csharp
services.AddHttpClient("AfrowaveHttpService");
services.AddSingleton(new HttpRequestOptions
{
    Timeout = TimeSpan.FromSeconds(30),
    DefaultHeaders = new Dictionary<string,string>
    {
        ["User-Agent"] = "Afrowave-HttpService/1.0"
    }
});

// HTTP service
services.AddSingleton<IHttpService, HttpService>();

// Cookie service
builder.Services.AddHttpContextAccessor();
services.AddConfiguredService<ICookieService, CookieService, CookieSettings>(cfg =>
{
    cfg.Domain = ".afrowave.ltd";
    cfg.SameSite = CookieSettings.SameSitePolicy.Lax;
    cfg.ExpiryInDays = 7;
});
```

---

## 🧱 Design Principles

* Compatible with **.NET Standard 2.1**
* Unified, predictable interfaces for both DI and static usage
* Fully documented XML and markdown-based documentation
* Secure defaults: `HttpOnly = true`, `SameSite = Lax`, `Secure = true`
* Long-lived cookie helpers for persistent preferences
* Seamless integration with `IOptionsMonitor<T>` for named profiles

---

## 📦 Version 0.0.3 Release Notes

| Version   | Changes                                                                                                                                                                                                                  |
| --------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **0.0.3** | Added complete cookie management subsystem:<br>• `CookieSettings` with enum-based policies<br>• `ICookieService` + `CookieService` (DI)<br>• `CookieHelper` (static)<br>• Improved documentation structure for web tools |
| **0.0.2** | Initial release – HTTP/REST utilities and retry logic                                                                                                                                                                    |

---

✍️ This file is part of the multilingual documentation system.
Translations will be managed automatically by **LangHub**.
