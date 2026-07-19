# Menu Integration Compatibility Reference

Use `$nuansystem-winforms-navigation-security` as the authoritative workflow. This reference remains for older links.

## Intake

Infer routine naming, parent placement, display order, operations, and FormKey from the closest module when repository evidence is clear.

Stop for a user/product decision only when the work introduces a new top-level module, ambiguous destructive operations, or default role grants without policy evidence.

## End-to-end identity

Use one stable kebab-case `FormKey` across:

- `SecurityForms`;
- `SecurityMenus` and navigation payload;
- endpoint form-operation authorization;
- frontend factory/shell routing;
- operation access;
- grid column personalization.

Do not create aliases or casing variants.

## Frontend

- Register clients, ViewModels, forms, and factories in the established DI path.
- Ensure `MainForm`/`ShellViewModel` can resolve the exact FormKey.
- Use the same key for grid settings.
- Reflect operation access through `ApiSession` and established permission models.
- Keep backend authorization authoritative.

## Security/Menu SQL

- Add a new versioned, idempotent Master script.
- Insert/update `SecurityForms`, `SecurityMenus`, form operations, and approved role mappings.
- Follow established `MENU.{PARENT}.{ITEM}` codes.
- Preserve existing records and relationships on re-execution.
- Do not silently grant default access.

## Validation

- Confirm dynamic navigation loads the menu.
- Confirm direct form resolution uses the same FormKey.
- Validate allowed/denied operations and backend enforcement.
- Validate active-company context and tenant isolation.
- Build the frontend and verify idempotent SQL behavior.
