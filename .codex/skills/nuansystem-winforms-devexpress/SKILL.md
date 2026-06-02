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
   - For standard grid-based CRUD listings, prefer the established `BaseGridCrudListForm` pattern used by `InventoryItems/ItemsForm` and `GeneralInventory/ItemGroups/ItemGroupsForm` before creating a custom toolbar/grid layout. The list form should keep a minimal `.Designer.cs`, inherit `BaseGridCrudListForm`, call `SetGridData(...)`, configure visible columns in `ConfigureGridColumns()`, use `SelectedGridItem<T>()`, wire `ConfigureColumnPersonalization(...)` with the form key, and reuse the base search, pagination, selection, audit footer, export, column customization, and CRUD action behavior.
10. Respect permissions through `ApiSession`, `CrudOperationPermissions`, and operation access from backend security.
11. Keep `X-Company-Code` handled by `NuanApiClient`; individual clients should only call route paths.
12. For `LookUpEdit` or `SearchLookUpEdit` controls bound to another table/maintenance, show both code and name columns and provide a way to create a related option only when the current user has create access for that related maintenance.
13. When editing `.Designer.cs` files, use syntax that the Visual Studio WinForms Designer can parse, not only syntax that the C# compiler accepts. Avoid modern collection expressions and other advanced shorthand inside `InitializeComponent`.
14. Fundamental designer rule: visual form design must be authored in the classic Visual Studio Designer style, field by field inside `InitializeComponent`. Do not build visual layout at runtime and do not use helper methods such as `BuildHeader()`, `BuildTabs()`, `AddLabeled(...)`, `AddSwitch(...)`, or `Group(...)` to create or position controls. The non-designer `.cs` file should contain only behavior such as events, data loading, validation, permissions, and request mapping.

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
- For list screens, first inspect nearby examples such as `ItemsForm` and `ItemGroupsForm`. If the screen is a normal maintenance listing, base it on `BaseGridCrudListForm` rather than hand-building a separate `GridControl`, pagination bar, search box, audit panel, export behavior, or column customization UI.
- Use the project typography standard consistently: base forms, labels, inputs, grid rows, and ordinary controls use `Segoe UI 9F Regular`; buttons use `Segoe UI Semibold 9F Bold` when supported; section titles use `Segoe UI Semibold 11F Bold`; main titles use `Segoe UI Semibold 14F Bold`; grid headers use `Segoe UI Semibold 9F Bold`.
- In dense ERP edit forms, vertically stacked field rows must use a 26 px top-to-top cadence between controls. Standard single-line editors must use `Size = new Size(width, 22)`: `TextEdit`, `SearchLookUpEdit`, `LookUpEdit`, `DateEdit`, and `ComboBoxEdit`. This gives a 4 px visible vertical gap between 22 px high editors. Apply the same cadence to the paired label row unless the local form has a documented exception.
- Every DevExpress `GridView`, including popup views used by `SearchLookUpEdit` and `GridLookUpEdit`, must explicitly set `Appearance.Row.Font = new Font("Segoe UI", 9F)` and `Appearance.HeaderPanel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold)` in `.Designer.cs` for designer-backed forms.
- Align numeric values to the right in edit fields and list/grid columns, including summaries, totals, quantities, amounts, percentages, prices, costs, stock fields, dimensions, weights, volumes, days, and counters. Apply this to `TextEdit`, `SpinEdit`, `CalcEdit`, numeric `GridColumn` cells, and similar DevExpress editors that display numbers.
- Apply `FormStyler.ApplyBase(this)` for code-built forms, and for designer-backed forms set the form font according to `$nuansystem-enterprise-typography`: `new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)` for dense ERP forms or `9.75F` when the form needs greater legibility. Prefer `FormStyler.LabelFont` or `AppTypography.LabelFont` for labels and avoid `Tahoma`, `Arial`, `Times New Roman`, or ad-hoc fonts unless a legacy local form already requires an exception.
- For hierarchical maintenances such as chart of accounts, categories, organizational structures, or parent/child catalogs, prefer an edit experience with the detail fields and a visible `TreeList`/hierarchy selector in the same form. Show code and name in the tree, keep the parent selector synchronized, and use side action/type buttons when they make navigation or classification faster.
- Do not force operational flows such as sales, cash closing, transfers, counts, or purchase receiving into CRUD list/edit forms; use dedicated workflow screens.
- In operational document edit forms, do not use `GroupControl`/group panels for visual sections unless the user explicitly asks for that control. Use `PanelControl` with explicit `LabelControl` section titles and manually positioned child controls so the design looks like a flat ERP document screen instead of boxed group panels.
- Hide, show, enable, or require fields based on company capabilities when a feature varies by business giro.
- Use `GridControl` + `GridView` instead of `DataGridView`.
- In edit/maintenance forms, keep the primary footer buttons consistent with the Empresas form: `Guardar` and `Cancelar` should use `Size = new Size(100, 36)` unless an existing local pattern in that same form family requires otherwise. When a form calls `OperationButtonIcons.ApplySaveCancel(...)`, reapply `btnGuardar.Size = new Size(100, 36)` and `btnCancelar.Size = new Size(100, 36)` immediately after that call if the Visual Studio designer or runtime scaling shows a different value.
- For `Guardar` and `Cancelar`, put all visual/layout button properties in the `.Designer.cs` so the Visual Studio designer shows the same result as runtime: `Size`, `Location`, `Appearance.BackColor`, `Appearance.ForeColor`, `AppearanceHovered`, `LookAndFeel.Style = Flat`, `LookAndFeel.UseDefaultLookAndFeel = false`, font, `DialogResult`, `Text`, and tab order. Do not apply these visual/layout properties at runtime because that can move or resize controls and override user designer changes.
- Button images/icons are the only save/cancel button properties that may be applied at runtime when needed. Use `diskette_32.svg` for `Guardar` and `cancelar_32.svg` for `Cancelar`, loaded through `OperationButtonIcons.LoadOperationIcon(...)` when available. Preserve the designer layout by saving and restoring each button `Bounds`, and do not call helpers that also change size, location, font, color, text, hover, tab order, or look-and-feel.
- For edit form design, create and configure each visual control explicitly in `.Designer.cs`, in the old/manual Designer pattern: declare the field, instantiate it in `InitializeComponent`, set `Location`, `Size`, `Name`, `Text`, `Appearance`, `Properties.Buttons`, and add it to the parent. Avoid compact factory/helper methods for visual layout even inside `.Designer.cs`, because those methods hide the design from Visual Studio and make the form harder to maintain visually.
- For table-backed `LookUpEdit` and `SearchLookUpEdit`, include `Codigo` and `Nombre` columns at minimum, set a readable display text, and keep the selected value bound to the stable Id/code expected by the API.
- Add an inline create action for related lookup records when useful. For `LookUpEdit` and `SearchLookUpEdit`, prefer adding an embedded `EditorButton(ButtonPredefines.Plus)` to `Properties.Buttons`, as used by `InventoryItems/ItemEditForm` for item groups, instead of placing a separate `SimpleButton` beside the combo. Show/add that plus button only if `ApiSession` or form-operation access confirms create permission for the related maintenance.
- Handle embedded lookup create buttons through `Properties.ButtonClick` and compare `e.Button` against the stored `EditorButton` instance before opening the related maintenance.
- After creating a related lookup record, refresh that lookup's data source and select the newly created value when possible.
- Administrable auxiliary masters must have independent frontend modules. Create a concrete list form, concrete edit form, service client, models, ViewModel when used by the local pattern, `FormKey`, menu entry, and CRUD permissions for each auxiliary master instead of maintaining it only through the consuming form.
- Shared base forms, descriptors, and generic helpers may be used internally to reduce repeated code, but they must not be the only user-facing forms for multiple auxiliary masters. Each administrable auxiliary master must have explicit classes such as `SupplierGroupsForm`/`SupplierGroupEditForm`, `SupplierClassesForm`/`SupplierClassEditForm`, and equivalent menu/navigation wiring.
- For supplier auxiliary masters, place every concrete maintenance under `Forms/GeneralSupplier/{AuxiliaryMaster}`. The folder must contain at minimum the list form and edit form for that specific master; do not place all supplier auxiliary master screens only under a shared `Catalogs` folder. `Forms/GeneralSupplier/Catalogs` may contain route/permission descriptors only. Shared non-form helpers may live under `Forms/GeneralSupplier/Common`; visual form inheritance must remain on the official `Forms/Common` base forms.
- Consuming forms must treat auxiliary masters as lookups: bind `LookUpEdit`/`SearchLookUpEdit` to data returned by API services, keep value members stable, and use the permission-controlled `+` action to open the owning maintenance. Do not hard-code administrable catalogs as permanent combo values in the form.
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

## API Consumption Rules

- Forms must never create `HttpClient` directly.
- All HTTP communication must go through `NuanApiClient` or the approved service client.
- Individual feature clients must only know route paths and request/response models.
- JWT token and `X-Company-Code` must be injected automatically by `NuanApiClient`.
- Forms must not manually add `Authorization` or `X-Company-Code` headers.
- API errors must be converted into user-friendly messages using the common frontend error handler.
- WinForms must never connect directly to SQL Server.
- WinForms must never connect directly to SAP Business One.
- Forms must not contain business rules.
- Forms should coordinate UI, call services, and render responses.

## Encoding Safety

- Preserve Spanish accented text in WinForms files. Do not leave mojibake such as `CÃ³digo`, `DescripciÃ³n`, `LÃ­nea`, `CategorÃ­a`, `SubcategorÃ­a`, `Ãšltimo`, `Ãtem`, `Â`, or `�`.
- Prefer `apply_patch` for manual edits. Avoid bulk rewrites with PowerShell `Set-Content` unless the encoding is explicitly controlled.
- If a mechanical rewrite is necessary on Windows, read and write with .NET UTF-8 explicitly, preferably `new UTF8Encoding(false)`, so `.cs` and `.Designer.cs` files remain UTF-8 without BOM and accents stay valid.
- After any broad text rewrite in WinForms files, run `rg -n "Ã|Â|�" <changed files>` and fix matches before compiling.
- If accented text is already corrupted, repair the text first, then compile. Do not continue editing layout while the file contains mojibake.

## References

- Load `references/ui-checklist.md` before implementing a new screen.
- Load `references/lookup-controls.md` when a form uses `LookUpEdit`, `SearchLookUpEdit`, or a selector backed by another table.
- Load `references/menu-integration.md` when a new form needs a menu entry or form-operation permissions.
- Use `$nuansystem-backend-crud` when an API endpoint or DTO must be added.
