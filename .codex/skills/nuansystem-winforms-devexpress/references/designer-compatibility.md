# Designer Compatibility Reference

Use this reference before creating or editing WinForms `.Designer.cs` files. The Visual Studio WinForms Designer is more restrictive than the C# compiler; code can build and still fail to open visually.

## Core Rules

- Keep visual layout in `.Designer.cs` for designer-backed forms.
- Keep logic, events, API calls, validation, permission checks, lookup loading, and request mapping in the main `.cs` partial class.
- Author visual controls in the classic Designer style: declare fields, instantiate controls in `InitializeComponent`, set properties explicitly, and add controls to their parent.
- For maintenance forms, do not use `Panel`, `PanelControl`, `TableLayoutPanel`, `FlowLayoutPanel`, or similar panel/layout containers unless the user explicitly requests them. Prefer direct placement on the form or `XtraTabPage` with explicit `Location`, `Size`, `Anchor`, and minimal `Dock`.
- Do not build visual layout with runtime helper methods such as `BuildHeader()`, `BuildTabs()`, `AddLabeled(...)`, `AddSwitch(...)`, or `Group(...)`.
- Do not hide visual structure behind factories that the Designer cannot represent.

## Collection Syntax

Do not use C# collection expressions inside `InitializeComponent`.

Avoid:

```csharp
Controls.AddRange([lblCode, txtCode]);
Properties.Buttons.AddRange([new EditorButton(ButtonPredefines.Combo)]);
```

Use classic array syntax:

```csharp
Controls.AddRange(new Control[] { lblCode, txtCode });
Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });
```

The same rule applies to DevExpress view collections, editor button collections, repository items, tab pages, and other control arrays.

## Visual Properties

For designer-backed forms, put visual/layout properties in `.Designer.cs`:

- `Name`
- `Text`
- `Location`
- `Size`
- `TabIndex`
- `Anchor` and `Dock`
- `Appearance` fonts and colors
- `Properties.Appearance`
- `Properties.Buttons`
- `LookAndFeel` values
- `DialogResult` for standard footer buttons

Button images/icons may be applied at runtime when the project helper requires it, but runtime helpers must preserve existing bounds and must not move, resize, recolor, or retitle the controls unexpectedly.

## Review Scans

After editing designer files, run targeted scans:

```powershell
rg -n "AddRange\(\[" src/Frontend
rg -n "\[new" src/Frontend
rg -n "<mojibake-pattern>" <changed files>
```

Fix every `AddRange([ ... ])` match in `.Designer.cs`. Review `[new` matches manually because they may be valid outside Designer layout.

## Validation

- Build the touched frontend project or solution.
- If Visual Studio reports a designer error, inspect the reported line in `InitializeComponent` first.
- Check that the visual tree is still represented by fields and explicit parent `Controls.Add(...)` / `Controls.AddRange(...)` calls.
