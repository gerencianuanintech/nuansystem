---
name: nuansystem-winforms-navigation-security
description: Add, modify, or review NuanSystem WinForms navigation and security wiring, including FormKey, dynamic menus, SecurityForms, SecurityMenus, SecurityRoleMenus, form operations, CrudOperationPermissions, ApiSession, Program registration, MainForm/ShellViewModel factories, column personalization keys, company context, and backend authorization.
---

# NuanSystem WinForms Navigation and Security

## Authority and discovery

Run `$nuansystem-framework-discovery`, reuse its core record, and inspect:

- `.codex/skills/nuansystem-winforms-devexpress/references/menu-integration.md`
- `src/Frontend/NuanSystem.WinForms.Services/Session/ApiSession.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Common/CrudOperationPermissions.cs`
- `src/Frontend/NuanSystem.WinForms/Program.cs`
- `src/Frontend/NuanSystem.WinForms.Forms/Shell/MainForm.cs`
- `src/Frontend/NuanSystem.WinForms.ViewModels/Shell/ShellViewModel.cs`
- `src/Backend/NuanSystem.Api/Extensions/EndpointAuthorizationExtensions.cs`
- the closest idempotent Master security/menu SQL script

Frontend visibility is UX. Backend authorization and tenant isolation remain authoritative.

## Identity contract

A navigable screen has one stable `FormKey` shared by:

- `SecurityForms`;
- `SecurityMenus` and navigation response;
- endpoint form-operation authorization;
- frontend form/factory routing;
- operation-access loading;
- grid column personalization;
- logs/audit references where applicable.

Use kebab-case following nearby values. Never introduce aliases or casing variations for the same screen.

## New screen workflow

1. Discover the closest menu/security slice.
2. Determine domain folder, visible label, parent menu, display order, `FormKey`, operations, and default access.
3. Add/update idempotent Master data for API `Permissions`, approved `RolePermissions`, form, menu, operations, role-menu access, and approved default role access.
4. Protect API endpoints through the established form-operation authorization path.
5. Register typed clients/ViewModels/forms in `Program.cs` as required.
6. Add the navigation/factory mapping used by `MainForm`/`ShellViewModel`.
7. Use the same `FormKey` for the form and grid personalization.
8. Validate allowed and denied users, menu visibility, direct navigation, and company context.

Infer naming/order from the nearest module when safe. Stop for product decisions such as a new top-level module, destructive permissions, or default access that cannot be inferred.

## Operation permissions

- Reuse established operation names/codes when semantics match.
- Map standard read/create/update/delete through `CrudOperationPermissions` when the feature follows that model.
- Include refresh, consult, copy, history, customize columns, export, post, cancel, approve, or other operations only when the screen actually exposes them.
- Registrar por separado los dos niveles obligatorios: `SecurityFormOperations` define qué operaciones son aplicables y visibles para el formulario; `SecurityRoleFormOperations` define cuáles de esas operaciones puede ejecutar cada rol. Una concesión al rol nunca sustituye el registro de aplicabilidad.
- Para un maestro CRUD estándar, registrar las doce operaciones canónicas: actualizar, consultar, crear, modificar, eliminar, copiar, historial, personalizar columnas y exportar a Excel, PDF, JSON y XML. Si la pantalla no soporta alguna, documentar y probar explícitamente la excepción.
- Disable/hide UI actions based on loaded access, but keep backend enforcement.
- Related lookup creation checks the related maintenance's create permission, not only the parent form permission.
- Consult/read-only mode disables all mutation paths.

## `ApiSession`

Use it for current authenticated user, active company, access token exposure to central transport, and permission lookup.

- Do not mutate session from feature forms.
- Do not manually add JWT or `X-Company-Code` in forms/feature clients.
- Do not treat `HasPermission` as backend authorization.
- Clear company-dependent screen state when company context changes through the established shell flow.

## Menu and DI registration

- Keep Services, ViewModels, Forms, and factories registered consistently.
- Do not use service locator calls inside forms to avoid constructor registration.
- Ensure dynamic menu payload can resolve the exact `FormKey`.
- Preserve one-instance/multiple-instance behavior from the shell's existing form-opening pattern.
- Keep visible menu text and codes separate from stable `FormKey` identity.

## Master SQL

- Use a new versioned, idempotent script; do not rewrite historical deployment scripts for new features.
- Seed every permission code required by endpoint policies in `Permissions`; registering a policy constant in code does not create the database permission or add it to JWT claims.
- Add approved default-role mappings in `RolePermissions` separately from `SecurityRoleFormOperations`; menu/form access cannot satisfy an endpoint permission policy.
- Insert/update `SecurityForms`, `SecurityMenus`, operations, and role mappings using established keys and guards.
- Resolve forms, menus, and role-menu mappings by stable keys even when they are soft-deleted; reactivate the existing physical row before considering an insert so unique constraints remain rerunnable.
- Comprobar que el script de instalación limpia inserte o reactive primero las operaciones en `SecurityFormOperations` y después conceda únicamente los roles aprobados en `SecurityRoleFormOperations`.
- Keep menu codes in the established `MENU.{PARENT}.{ITEM}` family.
- Do not silently grant default roles beyond the approved policy.
- Preserve existing IDs/relationships on re-execution.
- If an incomplete script already ran in an environment, correct the source script for clean installations and add a later idempotent repair script for deployed installations.
- Después de desplegar, verificar en la base real el conteo de operaciones aplicables y concedidas. Para un maestro CRUD canónico deben existir 12 aplicables; el formulario de Accesos debe devolverlas aunque todavía no estén concedidas a un rol no administrativo.
- After granting a new API permission, require a new login/token before runtime validation; an existing JWT does not acquire newly inserted claims.

## Multi-company boundary

- Frontend requests flow through `NuanApiClient` with active company context.
- Backend middleware validates company selection/access.
- Persistence obtains the correct tenant connection through established infrastructure.
- Never accept audit company/user identity from an untrusted form request when claims/context provide it.
- Menu visibility does not grant cross-company data access.

## Decision tree

```text
Screen is navigable?
  No -> no menu row; still evaluate FormKey/operations for embedded security.
  Yes -> existing parent/module?
           Yes -> follow nearest menu/security seed and factory.
           No -> product/architecture decision before a new top-level module.
Standard CRUD permissions fit?
  Yes -> reuse CrudOperationPermissions pattern.
  No -> define only real operations and enforce them end-to-end.
```

## Antipatterns

- Menu entry without API authorization.
- UI-hidden action treated as security.
- Different `FormKey` in SQL, API, factory, and grid settings.
- Hard-coded menu trees that bypass dynamic navigation.
- Default ADMIN grants assumed without policy evidence.
- Form/menu/operation seeds without the API `Permissions` and approved `RolePermissions` required by `RequirePermission`.
- Manual auth/company headers.
- Form service-location instead of DI registration.
- Non-idempotent security seed or rewritten historical script.

## Validation checklist

- [ ] One stable `FormKey` is used end-to-end.
- [ ] Form/menu/operations/role mappings are complete and idempotent.
- [ ] `SecurityFormOperations` y `SecurityRoleFormOperations` fueron validados por separado; no se aceptan permisos de rol sin operaciones aplicables.
- [ ] Endpoint permission codes exist in `Permissions`, approved roles have `RolePermissions`, and runtime testing uses a freshly issued token.
- [ ] DI and shell navigation resolve the screen.
- [ ] UI permissions and backend authorization align.
- [ ] Related creation and read-only paths are secured.
- [ ] Allowed, denied, direct-route, and multi-company scenarios are tested or reported.
- [ ] Menu/navigation, operation loading, build, and SQL checks are truthfully evidenced.


