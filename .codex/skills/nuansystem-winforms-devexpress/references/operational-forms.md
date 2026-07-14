# Operational Forms Reference

Use this reference for WinForms screens that represent operational workflows rather than simple administrative maintenance.

## Do Not Force CRUD

Do not force these workflows into standard CRUD list/edit forms:

- Sales or invoicing.
- Cash opening, cash closing, cash movements, and cash reconciliation.
- Purchase receiving.
- Inventory transfers.
- Inventory counts.
- Adjustments.
- Returns or cancellations.
- Operational monitors.
- Any workflow that affects stock, money, prices, purchases, documents, audit-sensitive records, or concurrency-sensitive state.

`BaseGridCrudListForm` is correct for standard maintenance. Operational workflows need dedicated screens shaped around the transaction.

## Responsibilities

Operational forms may:

- Manage UI state.
- Display draft document lines.
- Coordinate user actions.
- Show totals returned or validated by the API.
- Call API services.
- Show operational status, warnings, and friendly errors.
- Disable actions while async operations are running.

Operational forms must not:

- Contain business rules.
- Calculate authoritative stock, tax, price, cost, payment, or accounting results.
- Directly update databases.
- Connect to SAP or SRI.
- Bypass API permissions.
- Treat frontend validation as authoritative backend validation.

## ViewModel State

Use a ViewModel for screen state when the workflow has more than trivial interactions:

- Header state.
- Detail lines.
- Selected customer/supplier/item/warehouse.
- Busy flags.
- Validation messages.
- Draft status.
- Totals as displayed values.
- Available actions based on permissions and document state.

Keep state transitions explicit and easy to test from the UI layer.

## API Services

- Use module service clients backed by `NuanApiClient`.
- The API/Application layer owns business rules and transaction boundaries.
- The backend returns the authoritative result after validation or posting.
- Show backend business errors clearly and safely.

## Async UX

- Disable posting, saving, deleting, authorizing, receiving, transferring, or counting actions while the operation is running.
- Keep cancel/close behavior predictable during long calls.
- Show success, pending, rejected, and failed states using the established UI pattern.
- Preserve user-entered draft data when an API business error is returned and correction is possible.

## Layout

- Prefer a workflow layout over a generic CRUD layout.
- For document-like screens, use flat `PanelControl` sections and explicit `LabelControl` section titles.
- Avoid `GroupControl`/boxed group panels unless the user explicitly requests them or a nearby operational form already uses that local convention.
- Keep primary actions visible and consistent with project form standards.

## Checklist

Before delivering an operational form:

- The form is not forced into CRUD just because it has a grid.
- Business rules remain in Application/API.
- API calls go through service clients.
- Busy state blocks duplicate async actions.
- Operational errors and statuses are visible and user-friendly.
- Permissions are reflected through `ApiSession` while backend remains authoritative.
- The touched frontend project compiles when practical.
