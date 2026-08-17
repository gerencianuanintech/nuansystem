---
name: nuansystem-frontend-api-client
description: Build or review NuanSystem WinForms HTTP transport, sessions, and typed API clients through INuanApiClient. Use for NuanApiClient, ApiSession, authentication/company clients, ApiClientException, DI, or typed service contracts; skip for form-only changes that do not alter API consumption.
---

# NuanSystem Frontend API Client

## Authority and discovery

Run `$nuansystem-framework-discovery`, reuse its core record, and treat these repository contracts as authoritative:

| Responsibility | Current implementation |
|---|---|
| HTTP abstraction | `src/Frontend/NuanSystem.WinForms.Services/Http/INuanApiClient.cs` |
| HTTP transport | `src/Frontend/NuanSystem.WinForms.Services/Http/NuanApiClient.cs` |
| API failure | `src/Frontend/NuanSystem.WinForms.Services/Http/ApiClientException.cs` |
| Session state | `src/Frontend/NuanSystem.WinForms.Services/Session/ApiSession.cs` |
| Authentication client | `src/Frontend/NuanSystem.WinForms.Services/Authentication/AuthenticationClient.cs` |
| Authentication contract | `src/Frontend/NuanSystem.WinForms.Services/Authentication/IAuthenticationClient.cs` |
| Company client | `src/Frontend/NuanSystem.WinForms.Services/Companies/CompanyClient.cs` |
| Company contract | `src/Frontend/NuanSystem.WinForms.Services/Companies/ICompanyClient.cs` |

Inspect the actual interfaces and one nearby typed client before changing a method signature or adding transport behavior. Documentation examples never override source contracts.

## Request flow

```text
Form/ViewModel
  -> typed feature client
  -> INuanApiClient
  -> NuanApiClient + ApiSession
  -> REST API
  -> typed response or ApiClientException
  -> UI presentation at the form boundary
```

- Forms depend on typed feature services; they do not create `HttpClient` or compose transport headers.
- Typed clients own feature routes and request/response types, not shared transport mechanics.
- `NuanApiClient` owns JSON serialization, HTTP execution, authentication/company headers, standard error parsing, file transfer, and availability checks exposed by its interface.
- Backend business validation and authorization remain authoritative.

## Session and company context

- Use the registered `ApiSession`; do not create a second global session store.
- Keep access tokens in the approved in-memory lifecycle unless secure persistence is explicitly designed and approved.
- Clear authentication and company state through the existing logout/session flow.
- Do not manually attach bearer tokens or `X-Company-Code` in forms or typed clients.
- Treat the company header as request context only; backend middleware must validate user-company access.
- When changing login, token renewal, company selection, or logout, inspect the authentication UI, dependency registration, and backend authentication/company contracts.

## Typed client rules

1. Reuse `INuanApiClient` public methods before extending the transport.
2. Add a focused interface and implementation in the owning feature folder when the repository pattern requires one.
3. Keep routes centralized in the typed client and preserve the API's exact JSON contract.
4. Pass `CancellationToken` through supported async operations, especially loads, search, export, upload, and synchronization.
5. Use the existing multipart and download methods for files; never expose the underlying `HttpClient` merely to bypass the abstraction.
6. Register clients through the established frontend composition path and verify their consumers.
7. Do not duplicate DTOs just to rename the same contract.

## Errors and user messages

- Preserve `ApiClientException` and the response metadata it exposes; do not introduce a parallel `ApiException` type.
- Do not surface raw SQL, SAP, SRI, stack traces, or sensitive response bodies.
- Typed clients may add feature context without discarding the original safe message or trace correlation.
- Forms catch at the UI boundary using the established shared handling pattern, restore busy state, and show an understandable message.
- Cancellation is not a generic failure and must not produce a misleading error dialog.

Use `$nuansystem-api-error-logging` when the backend error envelope or Result-to-HTTP mapping changes. Use `$nuansystem-security-auth` when credentials, JWT behavior, login, logout, or token lifecycle changes.

## Cross-layer impact

When a frontend API contract changes, verify:

```text
typed client interface/implementation
  -> frontend models and consumers
  -> API route and request/response contract
  -> authorization and company context
  -> validation/error behavior
  -> dependency registration
  -> tests
```

A form-only layout or presentation change does not activate this skill unless the API call, model, session, error, or cancellation behavior also changes.

## Antipatterns

- `new HttpClient()` in forms or feature clients.
- Parallel `NuanApiClient`, session, serialization, or exception infrastructure.
- Inventing `AuthService`, `CompanyService`, or `ApiException` when the repository uses the contracts listed above.
- Manual JWT or company headers outside the centralized transport.
- Blocking `.Result` or `.Wait()` on UI requests.
- Swallowing API failures, fabricating success, or leaving stale data visible after a failed refresh.
- Direct WinForms access to SQL Server, SAP Business One, SRI, or worker storage.

## Completion gate

- [ ] Actual transport, session, exception, and typed-client contracts were inspected.
- [ ] No parallel HTTP/session/error infrastructure was introduced.
- [ ] Authentication, company context, cancellation, and safe errors are preserved.
- [ ] API and frontend contracts remain aligned.
- [ ] Dependency registration and affected consumers were verified.
- [ ] Relevant build/tests and negative paths are reported truthfully.
