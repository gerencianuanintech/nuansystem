---
name: nuansystem-winforms-devexpress
description: Build or modify NuanSystem WinForms UI with DevExpress, FormStyler, BrandResources, BaseCrudListForm/BaseEditForm, operational forms, ViewModels, HTTP service clients, ApiSession permissions, grid/list forms, edit forms, lookup/search lookup controls, related-record creation from selectors, menu integration, and frontend normalization for a configurable multi-business commercial system. Use for tasks touching src/Frontend, forms, controls, desktop service clients, view models, DevExpress visual standards, new form creation, or adding a form to the dynamic menu/security navigation.
---

# NuanSystem WinForms DevExpress

## Workflow

1. Read `docs/FRONTEND-DEVEXPRESS-NOMENCLATURA.md`, `docs/estandar-visual-winforms.md`, `docs/ARQUITECTURA-COMERCIAL.md`, and the closest existing form before editing.
2. When the user asks for a new form, ask for the destination folder/module name before creating files unless the user already provided it. Use the answer to place files under `Forms/{Folder}`, `ViewModels/{Folder}`, and `Services/{Folder}` as applicable.
3. When the user asks for a new form, ask how it should appear in the menu unless the user already provided menu details. Capture parent menu, visible menu text, `FormKey`, display order, and desired operations/permissions.
4. Create or update the menu/security registration according to the user's indication. Prefer the existing dynamic menu model using `SecurityForms`, `SecurityMenus`, `SecurityRoleMenus`, form operations, and `FormKey`; update SQL seed/scripts when the menu entry must be available by default.
5. Use DevExpress controls for new UI. Prefer `XtraForm`, `SimpleButton`, `TextEdit`, `LabelControl`, `GridControl`, `GridView`, `LookUpEdit`, `ComboBoxEdit`, `SpinEdit`, `CheckEdit`, and `MemoEdit`.
6. Apply `Common.FormStyler.ApplyBase(this)` in forms built by code. Use `FormStyler` fonts and `BrandResources` colors instead of hard-coded colors.
7. Put API calls in `NuanSystem.WinForms.Services`; keep forms and view models free of direct HTTP mechanics.
8. Use ViewModels for UI state and orchestration. Do not place business rules in forms.
9. Use `BaseCrudListForm` for maintenance lists and `BaseEditForm` for create/edit/consult flows when applicable.
10. Respect permissions through `ApiSession`, `CrudOperationPermissions`, and operation access from backend security.
11. Keep `X-Company-Code` handled by `NuanApiClient`; individual clients should only call route paths.
12. For `LookUpEdit` or `SearchLookUpEdit` controls bound to another table/maintenance, show both code and name columns and provide a way to create a related option only when the current user has create access for that related maintenance.
13. When editing `.Designer.cs` files, use syntax that the Visual Studio WinForms Designer can parse, not only syntax that the C# compiler accepts. Avoid modern collection expressions and other advanced shorthand inside `InitializeComponent`.

## New Form Intake

Before creating a new form, ensure these decisions are known:

- Folder/module name: the exact folder under `Forms`, `ViewModels`, and `Services`.
- Form class name and screen title.
- Menu placement: parent menu/category and visible menu label.
- `FormKey`: kebab-case key used by navigation, permissions, grid settings, and form-operation authorization.
- Operations: refresh, create, update, delete, consult, copy, history, column customization, and exports as applicable.
- Default access: whether an existing admin role should receive the new menu and operations in seed SQL.

If any of these are missing and the user asked for a new form, pause and ask a concise question before writing files.

## Naming

- Prefix control fields with the approved DevExpress prefixes.
- Name service clients as `{Entity}Client` and interfaces as `I{Entity}Client`.
- Place service models under `Services/{Module}/Models`.
- Place view models under `ViewModels/{Module}` and forms under `Forms/{Module}`.
- Use the user-provided folder/module name exactly when it matches existing folder conventions; otherwise normalize only casing/pluralization to match nearby folders after confirming.

## UI Rules

- Keep maintenance screens efficient: toolbar/actions, grid, focused edit dialog.
- For hierarchical maintenances such as chart of accounts, categories, organizational structures, or parent/child catalogs, prefer an edit experience with the detail fields and a visible `TreeList`/hierarchy selector in the same form. Show code and name in the tree, keep the parent selector synchronized, and use side action/type buttons when they make navigation or classification faster.
- Do not force operational flows such as sales, cash closing, transfers, counts, or purchase receiving into CRUD list/edit forms; use dedicated workflow screens.
- Hide, show, enable, or require fields based on company capabilities when a feature varies by business giro.
- Use `GridControl` + `GridView` instead of `DataGridView`.
- For table-backed `LookUpEdit` and `SearchLookUpEdit`, include `Codigo` and `Nombre` columns at minimum, set a readable display text, and keep the selected value bound to the stable Id/code expected by the API.
- Add an inline create action for related lookup records when useful. For `LookUpEdit` and `SearchLookUpEdit`, prefer adding an embedded `EditorButton(ButtonPredefines.Plus)` to `Properties.Buttons`, as used by `InventoryItems/ItemEditForm` for item groups, instead of placing a separate `SimpleButton` beside the combo. Show/add that plus button only if `ApiSession` or form-operation access confirms create permission for the related maintenance.
- Handle embedded lookup create buttons through `Properties.ButtonClick` and compare `e.Button` against the stored `EditorButton` instance before opening the related maintenance.
- After creating a related lookup record, refresh that lookup's data source and select the newly created value when possible.
- Use `XtraMessageBox` or base form helpers for confirmations and feedback.
- Disable actions during async operations with the existing busy-state patterns.
- Avoid direct database calls, hard-coded company selection, and manual audit user input in WinForms.
- Do not add a form without also considering its dynamic menu entry and `FormKey`.

## WinForms Designer Compatibility

The WinForms Designer is more restrictive than the C# compiler. Code that builds successfully can still prevent Visual Studio from opening the visual designer.

When creating or editing `*.Designer.cs` files:

- Do not use C# collection expressions such as `[control1, control2]` inside `InitializeComponent`.
- Do not use `Properties.Buttons.AddRange([new EditorButton(...)])`.
- Do not use `Controls.AddRange([control1, control2])`.
- Use classic array syntax instead:
  - `Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });`
  - `Controls.AddRange(new Control[] { control1, control2 });`
- Prefer simple assignments and object creation statements that match the style generated by Visual Studio.
- Keep runtime logic, permission checks, API calls, events, and lookup loading outside `.Designer.cs`; place them in the main form partial class.
- After touching a designer file, scan it for `AddRange([` or `[new` and replace those patterns before compiling.
- Validate by compiling the WinForms project. If Visual Studio still shows a designer error, inspect the reported line in `InitializeComponent` first.

## References

- Load `references/ui-checklist.md` before implementing a new screen.
- Load `references/lookup-controls.md` when a form uses `LookUpEdit`, `SearchLookUpEdit`, or a selector backed by another table.
- Load `references/menu-integration.md` when a new form needs a menu entry or form-operation permissions.
- Use `$nuansystem-backend-crud` when an API endpoint or DTO must be added.
