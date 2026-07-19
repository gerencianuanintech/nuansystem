---
name: nuansystem-api-error-logging
description: Implement or review NuanSystem Result, ApiResponse, ApiError, FluentValidation failures, GlobalExceptionMiddleware, audit error logging, SQL/SAP exception classification, TraceIdentifier correlation, and production-safe API messages. Use when changing backend failures, HTTP error mapping, exception middleware, logging, validation responses, audit error records, or frontend error contracts.
---

# NuanSystem API Errors and Logging

## Authoritative contracts

Inspect before editing:

- `Application/Common/Models/Result.cs`
- `Shared/Responses/ApiResponse.cs`
- `Shared/Responses/ApiError.cs`
- `Api/Extensions/ResultExtensions.cs`
- `Application/Common/Behaviors/ValidationBehavior.cs`
- `Application/Common/Exceptions/ApplicationValidationException.cs`
- `Api/Middleware/GlobalExceptionMiddleware.cs`

Do not introduce `ApiErrorResponse`, `ProblemDetails`, a second `Error` record, or a parallel result type unless a separately approved migration updates every consumer.

## Expected versus unexpected failures

```text
Expected validation shape failure
  -> FluentValidation -> ValidationBehavior -> ApplicationValidationException -> 400 ApiResponse
Expected business/not-found/conflict outcome
  -> Result<T>.Failure(message, ApiError[]) -> ToHttpResult()
Unexpected SQL/SAP/infrastructure/programming exception
  -> GlobalExceptionMiddleware -> safe classified ApiResponse + technical server/audit log
```

`ToHttpResult()` currently maps failed `Result<T>` values to HTTP 400. Do not claim 404/409 behavior that the shared mapper does not implement. Any status-code refinement is a cross-cutting API contract change requiring frontend/test inspection.

## Error rules

- Use stable, feature-owned `ApiError.Code` values and the exact input field name when field feedback is useful.
- Keep user messages clear in Spanish and technical details out of production responses.
- Do not return raw SQL messages, SAP payloads, stack traces, connection strings, tokens, passwords, or encrypted secrets.
- Do not catch expected failures in every endpoint; keep endpoints transport-thin.
- Do not convert an unexpected persistence failure into fabricated success or a misleading business error.
- Preserve `context.TraceIdentifier` in classified technical error detail and audit records.

## Logging and audit

`GlobalExceptionMiddleware` logs unexpected failures through `ILogger`, attempts to persist an audit error through `IAuditLogRepository`, classifies known SQL/SAP cases, and returns a safe `ApiResponse<object>`.

- Log structured properties rather than concatenated secret-bearing strings.
- Record operation identity, route, company code, user, trace id, and safe context when available.
- Keep audit logging best-effort: a failure to write the error audit must not hide the original failure.
- Never log request bodies or parameters wholesale when they can contain credentials or personal data.
- Development-only technical detail must remain guarded by environment checks.

## Change-impact tree

```text
Change Result<T>/ApiResponse/ApiError?
  -> inspect every handler, ToHttpResult, frontend deserializer, and contract test
Change exception classification?
  -> inspect SQL/SAP wrappers, middleware tests, status/message expectations, and audit logging
Change validation mapping?
  -> inspect validators, ValidationBehavior, frontend field matching, and tests
```

## Representative evidence

- Business errors: `Application/Features/Carriers/Commands/CarrierCommandHandlers.cs`.
- Validation conversion: `Application/Common/Behaviors/ValidationBehavior.cs`.
- SQL/SAP classification: `Api/Middleware/GlobalExceptionMiddleware.cs`.
- HTTP mapping: `Api/Extensions/ResultExtensions.cs`.

## Completion gate

- [ ] Existing response/result contracts are reused or migration scope is explicit.
- [ ] Expected and unexpected failures follow different paths.
- [ ] Codes, fields, user messages, status behavior, and consumers align.
- [ ] Secrets and raw technical data cannot reach clients/logs.
- [ ] Trace/audit behavior survives the failure path.
- [ ] Targeted validation/middleware/client tests and build are reported.
