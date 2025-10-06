# Afrowave.SharedTools.Api

This module provides **universal HTTP and API communication utilities** for Afrowave projects.
It offers both **static** and **dependency-injectable (DI)** variants for handling web requests, data serialization, retries, and fault-tolerant communication.
All components are fully compatible with **.NET Standard 2.1** and can be used in desktop, web, and service environments.

---

## 📦 Contents

* `Models/HttpRequestOptions.cs` – Configuration for timeout, proxy, and retry policy
* `Models/RetryPolicyOptions.cs` – Exponential retry with optional jitter
* `Interfaces/IHttpService.cs` – Unified API interface for async HTTP operations
* `DI/HttpService.cs` – Injectable service implementing the interface
* `Static/HttpClientHelper.cs` – Lightweight static helper (no DI required)
* `Serialization/JsonOptions.cs` – Preconfigured JSON serialization settings

---

## ⚙️ Purpose

Afrowave.SharedTools.Api acts as the **networking foundation** for all Afrowave tools.
It standardizes request logic, response wrapping, retry behavior, and proxy handling — enabling consistent API access across backend, frontend, and CLI applications.

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

## ⚙️ Static Helper

For projects without dependency injection (e.g. CLI tools, scripts, or test runners),
the static version `HttpClientHelper` offers identical features using a direct API.

```csharp
var opts = new HttpRequestOptions
{
    Timeout = TimeSpan.FromSeconds(10),
    Retry = new RetryPolicyOptions { MaxRetries = 3 }
};

var res = await HttpClientHelper.GetJsonAsync<MyDto>("https://api.example.com", opts);
if (res.Success)
    Console.WriteLine(res.Data.Name);
```

---

## 🧬 Configuration Models

### HttpRequestOptions

Defines global behavior of HTTP clients.
*Members:*

* `Uri BaseAddress` – Base URL for requests
* `TimeSpan Timeout` – Timeout duration
* `bool UseProxy` – Whether to use proxy
* `string ProxyAddress` – Proxy host and port
* `bool AllowInvalidCertificates` – Ignore SSL validation (for dev only)
* `RetryPolicyOptions Retry` – Retry configuration
* `IDictionary<string,string> DefaultHeaders` – Default headers (e.g., User-Agent)

### RetryPolicyOptions

Controls exponential retry with optional random jitter.
*Members:*

* `int MaxRetries`
* `TimeSpan BaseDelay`
* `double BackoffFactor`
* `bool Jitter`

---

## 🌐 Example Integration

**Program.cs (DI registration)**

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
services.AddSingleton<IHttpService, HttpService>();
```

**Usage in app code:**

```csharp
var res = await _http.PostJsonAsync<RequestDto, ResponseDto>(
    "https://api.example.com/process", payload);
if (res.Success)
    Console.WriteLine("Response OK");
```

---

## 🧱 Design Principles

* Compatible with **.NET Standard 2.1**
* No dependency on ASP.NET Core runtime — only `Microsoft.Extensions.Http`
* Optional proxy and SSL flexibility
* Consistent `Response<T>` return type across all methods
* Automatic retry and exponential backoff
* Parallel processing support for bulk HTTP operations

---

## 🧭 Structure

All classes include XML documentation and follow Afrowave naming conventions.
Both static and DI versions share identical logic to simplify cross-project usage.

---

✍️ This file is part of the multilingual documentation system.
Translations will be managed automatically by **LangHub**.
