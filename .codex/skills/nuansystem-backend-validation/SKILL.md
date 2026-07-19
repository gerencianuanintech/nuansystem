---
name: nuansystem-backend-validation
description: Define or review NuanSystem backend validation, FluentValidation command rules, authoritative Application invariants, normalization, uniqueness checks, stable ApiError codes, state validation, and SQL defense-in-depth. Use when adding fields, commands, business rules, closed code sets, uniqueness, transitions, or validation/error behavior.
---

# NuanSystem Backend Validation

## Validation ownership

Separate three layers:

1. **Shape validation** — required values, ranges, maximum lengths, format, collection shape: FluentValidation.
2. **Authoritative business validation** — uniqueness, current persisted state, allowed transition, company capability, cross-record invariant: Application handler/domain policy.
3. **Persistence defense** — foreign keys, checks, uniqueness, nullability, concurrency/status guards: SQL.

Frontend validation is early feedback only.

## Required workflow

1. Run `$nuansystem-framework-discovery` and inspect a same-domain validator/handler.
2. Define normalization and field semantics before writing rules.
3. Align C# maximum lengths and required/optional behavior with SQL and API contracts.
4. Define stable error codes and field names.
5. Reload authoritative state for updates, deletes, transitions, or operational decisions.
6. Add SQL constraints for invariants the database can guarantee.
7. Test valid, invalid, boundary, and race-sensitive cases.

## Rules

- Normalize once in Application before uniqueness checks and persistence.
- Do not silently truncate input.
- Do not invent legal, tax, identity, financial, or integration algorithms without an approved requirement/source.
- Validate closed code sets in Application and SQL; persist stable codes, not UI labels or indices.
- Use `RuleForEach`/child rules for collections and enforce aggregate-wide constraints such as one active primary item.
- Use repository existence checks with an exclusion id for update when uniqueness is required.
- Return `Result<T>.Failure` with stable `ApiError` codes for business conflicts and not-found cases.
- Do not use exceptions for expected validation/business outcomes.
- Preserve the exact field/property name needed by frontend error presentation.

## State-changing validation

```text
load current record
  -> verify existence/company ownership
  -> verify status/version/allowed action
  -> recalculate authoritative values
  -> persist inside the required transaction
```

Do not trust totals, prices, stock, balances, document status, approval state, or sync success sent by UI.

## Representative references

- Complex shape validation: `Features/BusinessPartners/Commands/BusinessPartnerCommandValidator.cs`.
- Uniqueness and stable errors: `Features/Geography/Commands/GeographyCommandHandlers.cs`.
- State/workflow policy: `Features/Purchasing/PurchaseOrders`.
- Shared responses: `NuanSystem.Shared/Responses` and `Application/Common/Models`.

## Antipatterns

- Same rule duplicated with different limits in UI, validator, handler, and SQL.
- Endpoint-only validation.
- Uniqueness checked only before insert without a database constraint when authoritative.
- Missing update/delete row treated as success.
- Generic exception for an expected conflict.
- Reflection-heavy shared validators introduced before proving stable reuse.

## Completion checklist

- [ ] Required/optional, normalization, length, format, and closed sets are explicit.
- [ ] Authoritative checks live in Application/domain.
- [ ] SQL constraints match the contract.
- [ ] Error codes and fields are stable.
- [ ] Create/update/delete and boundary tests exist.
- [ ] Operational rules reload persisted state and address concurrency.
