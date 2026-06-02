---
name: nuansystem-frontend-api-client
description: Build or review NuanSystem WinForms API consumption through a centralized NuanApiClient, ApiSession, AuthService, CompanyService, module services, automatic JWT and X-Company-Code headers, HTTP error handling, JSON serialization, timeouts, cancellation tokens, and DevExpress user-friendly messages. Use when touching frontend services, login, company selection, API clients, or form-to-API communication.
---

# NuanSystem Frontend API Client

## Core Rules

- WinForms consumes only the backend REST API.
- WinForms must never query SQL Server directly.
- WinForms must never connect directly to SAP Business One.
- Forms must not create `HttpClient` directly.
- Forms must not manually add `Authorization` or `X-Company-Code` headers.
- All HTTP communication goes through `NuanApiClient` or an approved service client.
- Keep JWT and selected company in `ApiSession`.
- Store tokens only in memory unless a secure persistence requirement is explicitly approved.
- Clear session data on logout.
- Convert API errors into user-friendly DevExpress messages through a common handler.
- Use `CancellationToken` for long-running loads, search, exports, and sync operations.

## Recommended Structure

```text
NuanSystem.WinForms
├── Services
│   ├── NuanApiClient.cs
│   ├── AuthService.cs
│   ├── CompanyService.cs
│   ├── CustomerService.cs
│   └── SapSyncService.cs
├── Session
│   └── ApiSession.cs
├── Models
│   ├── Requests
│   └── Responses
└── ErrorHandling
    └── ApiException.cs
```

## ApiSession

```csharp
public sealed class ApiSession
{
    public string? AccessToken { get; private set; }
    public string? UserName { get; private set; }
    public string? CompanyCode { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken);
    public bool HasCompany => !string.IsNullOrWhiteSpace(CompanyCode);

    public void SetAuthenticatedUser(string accessToken, string userName)
    {
        AccessToken = accessToken;
        UserName = userName;
    }

    public void SelectCompany(string companyCode)
    {
        CompanyCode = companyCode;
    }

    public void Clear()
    {
        AccessToken = null;
        UserName = null;
        CompanyCode = null;
    }
}
```

## ApiException

```csharp
public sealed class ApiException : Exception
{
    public string Code { get; }
    public string? TraceId { get; }
    public IReadOnlyList<ApiFieldError> Errors { get; }

    public ApiException(string code, string message, string? traceId, IReadOnlyList<ApiFieldError> errors)
        : base(message)
    {
        Code = code;
        TraceId = traceId;
        Errors = errors;
    }
}
```

## NuanApiClient

```csharp
public sealed class NuanApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ApiSession _session;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public NuanApiClient(HttpClient httpClient, ApiSession session)
    {
        _httpClient = httpClient;
        _session = session;
        _httpClient.Timeout = TimeSpan.FromSeconds(60);
    }

    public Task<T?> GetAsync<T>(string route, CancellationToken cancellationToken = default)
        => SendAsync<T>(HttpMethod.Get, route, null, cancellationToken);

    public Task<TResponse?> PostAsync<TRequest, TResponse>(
        string route,
        TRequest request,
        CancellationToken cancellationToken = default)
        => SendAsync<TResponse>(HttpMethod.Post, route, request, cancellationToken);

    private async Task<T?> SendAsync<T>(
        HttpMethod method,
        string route,
        object? body,
        CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(method, route);
        PrepareHeaders(request);

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw CreateApiException(content, response.StatusCode);

        if (string.IsNullOrWhiteSpace(content))
            return default;

        return JsonSerializer.Deserialize<T>(content, _jsonOptions);
    }

    private void PrepareHeaders(HttpRequestMessage request)
    {
        if (_session.IsAuthenticated)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.AccessToken);

        if (_session.HasCompany)
            request.Headers.TryAddWithoutValidation("X-Company-Code", _session.CompanyCode);
    }
}
```

## AuthService

```csharp
public sealed class AuthService
{
    private readonly NuanApiClient _apiClient;
    private readonly ApiSession _session;

    public async Task LoginAsync(string userName, string password, CancellationToken cancellationToken)
    {
        var response = await _apiClient.PostAsync<LoginRequest, LoginResponse>(
            "api/auth/login",
            new LoginRequest(userName, password),
            cancellationToken);

        _session.SetAuthenticatedUser(response!.AccessToken, response.UserName);
    }
}
```

## Usage From a DevExpress Form

```csharp
private async void btnGuardar_Click(object sender, EventArgs e)
{
    try
    {
        SetBusy(true);
        await _customerService.CreateAsync(BuildRequest(), CancellationToken.None);
        XtraMessageBox.Show(this, "Cliente guardado correctamente.", "NuanSystem");
    }
    catch (ApiException ex)
    {
        ApiErrorMessageBox.Show(this, ex);
    }
    finally
    {
        SetBusy(false);
    }
}
```

The form coordinates UI only. Validation that affects business rules must be enforced by the API.
