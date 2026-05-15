# Operational Use Case Checklist

## Domain

- Define invariants before coding.
- Avoid putting core business rules only in SQL or UI.
- Use explicit state names for documents, shifts, movements, and payments.

## Application

- Use one command per business action.
- Keep handler orchestration explicit.
- Read company capabilities before applying optional behavior.
- Use `ITransactionRunner` when multiple writes must succeed or fail together.

## Persistence

- Stored procedures should persist, query, and enforce database consistency.
- For transaction-scoped operations, pass the shared connection and transaction through repository methods or create operation-specific persistence methods.
- Ensure audit writes participate in the same transaction when required.

## Frontend

- Operational screens should optimize workflow speed and clarity.
- Do not force an operational flow into `BaseGridCrudListForm` if it is not a maintenance list.
- Use lookup DTOs optimized for the operation.
