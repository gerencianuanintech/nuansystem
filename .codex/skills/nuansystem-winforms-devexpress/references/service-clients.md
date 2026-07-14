# Service Clients Reference

Use this reference before creating or modifying WinForms service clients or form-to-API communication.

## Boundaries

- Forms must never create `HttpClient` directly.
- Forms must not manually send HTTP requests.
- Forms coordinate UI and call ViewModels or service clients.
- Module service clients call `NuanApiClient` or the approved centralized API client.
- WinForms must never connect directly to SQL Server, MySQL, SAP Business One, SRI, or tenant databases.

## NuanApiClient

`NuanApiClient` owns shared API mechanics:

- Base URL.
- JSON serialization.
- JWT `Authorization` header.
- `X-Company-Code` header.
- Timeout and cancellation behavior.
- Standard API error handling.
- Session-aware requests.

Do not add `Authorization` or `X-Company-Code` manually in forms or module clients.

## Module Clients

Module clients should only know:

- Route paths.
- Request models.
- Response models.
- Query string shape when needed.
- Basic mapping to frontend DTO/model types.

Keep module clients focused. Do not place business rules, permission decisions, tenant decisions, or UI messages inside them.

## Models

- Place request/response models under the relevant module `Models` folder.
- Keep frontend models aligned with API contracts but do not expose backend persistence entities directly.
- Prefer clear DTO names such as `CreateSupplierRequest`, `UpdateSupplierRequest`, `SupplierListItem`, or the local module convention.

## Errors

- Convert API errors into user-friendly messages through the common frontend error handler or established base form helpers.
- Do not show raw SQL, SAP, stack trace, JWT, connection string, or tenant details to users.
- Preserve technical details only in logs handled by approved logging paths.

## Cancellation And Busy State

- Pass `CancellationToken` when the existing local pattern supports it.
- Disable UI actions during async calls.
- Restore the UI state after success, business failure, cancellation, or exception.

## Checklist

Before delivering service-client changes:

- No form creates `HttpClient`.
- No form or module client manually adds `Authorization`.
- No form or module client manually adds `X-Company-Code`.
- The centralized client handles routes, headers, serialization, and errors.
- Models live in the module service model area.
- User-facing errors are friendly and production-safe.
- The touched frontend project compiles when practical.
