---
name: devexpress-enterprise-typography
description: Define and apply the official NuanSystem typography standard for C# WinForms DevExpress enterprise screens, including fonts, sizes, GridView typography, menu typography, Designer-visible font application, and the AppTypography helper class. Use when creating or modifying DevExpress forms, CRUD screens, maintenance forms, administrative screens, SAP Business One screens, configuration forms, menus, grids, or any NuanSystem WinForms UI.
---

# DevExpress Enterprise Typography

## Objetivo

Este skill define el estÃ¡ndar oficial de letras, fuentes y tamaÃ±os para todos los formularios empresariales de NuanSystem desarrollados con C# Windows Forms y DevExpress.

La fuente oficial de NuanSystem para WinForms DevExpress es:

```text
Segoe UI
```

No cambiar la fuente oficial salvo que el usuario lo indique explÃ­citamente.

## Reglas Obligatorias

- Aplicar este estÃ¡ndar automÃ¡ticamente al generar o modificar formularios DevExpress, pantallas CRUD, mantenimientos, grids, formularios de configuraciÃ³n, formularios SAP Business One, pantallas administrativas, menÃºs y formularios de consulta.
- Priorizar consistencia visual sobre experimentaciÃ³n grÃ¡fica.
- Mantener una interfaz compacta, profesional y legible.
- No mezclar muchos tamaÃ±os de fuente dentro de una misma pantalla.
- Configurar fuentes en el `.Designer.cs` o mediante una clase central compatible con Visual Studio Designer, de forma que el diseÃ±o se vea correctamente en el diseÃ±ador.
- Aplicar fuentes directamente a los controles DevExpress relevantes: `Appearance.Font`, `Properties.Appearance.Font`, `Appearance.HeaderPanel.Font`, `Appearance.Row.Font`, etc.

## Tabla Oficial

| Elemento | Fuente | TamaÃ±o | Estilo |
| --- | --- | ---: | --- |
| Fuente base del formulario | Segoe UI | 9 pt | Regular |
| TÃ­tulo principal del formulario | Segoe UI Semibold | 14 pt | Semibold/Bold |
| SubtÃ­tulos o textos descriptivos | Segoe UI | 10 pt | Regular |
| TÃ­tulos de secciones o `GroupControl` | Segoe UI Semibold | 11 pt | Semibold/Bold |
| Labels de campos | Segoe UI | 9 pt | Regular |
| Controles de entrada | Segoe UI | 9 pt | Regular |
| Botones `SimpleButton` | Segoe UI Semibold | 9 pt | Semibold/Bold |
| `GridView` Header | Segoe UI Semibold | 9 pt | Bold |
| `GridView` Rows | Segoe UI | 9 pt | Regular |
| `GridView` Footer | Segoe UI Semibold | 9 pt | Bold |
| `GridView` Filter row | Segoe UI | 9 pt | Regular |
| MenÃºs `RibbonControl`, `AccordionControl`, `NavBarControl`, `BarManager` | Segoe UI | 9 pt | Regular |
| Texto auxiliar o notas pequeÃ±as | Segoe UI | 8.5 pt | Regular |

## Controles de Entrada

Aplicar `InputFont` a:

- `TextEdit`
- `LookUpEdit`
- `GridLookUpEdit`
- `SearchLookUpEdit`
- `ComboBoxEdit`
- `DateEdit`
- `SpinEdit`
- `MemoEdit`
- `ButtonEdit`
- `CalcEdit`

Regla:

```csharp
control.Properties.Appearance.Font = AppTypography.InputFont;
control.Properties.Appearance.Options.UseFont = true;
```

## Fuentes Prohibidas

No usar fuentes decorativas, informales o no empresariales en la interfaz:

- Comic Sans
- Impact
- Brush Script
- Papyrus
- Times New Roman para interfaz
- Courier New para formularios normales

`Consolas` solo puede usarse para:

- Logs
- SQL
- JSON
- CÃ³digo
- DiagnÃ³sticos tÃ©cnicos

## Clase C# Oficial

Crear o reutilizar una clase central `AppTypography` para mantener una sola fuente de verdad. La clase debe estar en un proyecto compartido de UI, por ejemplo:

```text
src/Frontend/NuanSystem.WinForms.Forms/Common/AppTypography.cs
```

Ejemplo compatible con WinForms + DevExpress:

```csharp
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid;

namespace NuanSystem.WinForms.Forms.Common;

internal static class AppTypography
{
    public static readonly Font BaseFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font LabelFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font InputFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font ButtonFont = new("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font TitleFont = new("Segoe UI Semibold", 14F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font SectionFont = new("Segoe UI Semibold", 11F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font GridHeaderFont = new("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point);
    public static readonly Font GridRowFont = new("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
    public static readonly Font SmallFont = new("Segoe UI", 8.5F, FontStyle.Regular, GraphicsUnit.Point);

    public static void ApplyToForm(XtraForm form)
    {
        form.Font = BaseFont;
        form.Appearance.Font = BaseFont;
        form.Appearance.Options.UseFont = true;
    }

    public static void ApplyTitle(LabelControl label)
    {
        label.Appearance.Font = TitleFont;
        label.Appearance.Options.UseFont = true;
    }

    public static void ApplyLabel(LabelControl label)
    {
        label.Appearance.Font = LabelFont;
        label.Appearance.Options.UseFont = true;
    }

    public static void ApplyButton(SimpleButton button)
    {
        button.Appearance.Font = ButtonFont;
        button.Appearance.Options.UseFont = true;
    }

    public static void ApplyGrid(GridView gridView)
    {
        gridView.Appearance.HeaderPanel.Font = GridHeaderFont;
        gridView.Appearance.HeaderPanel.Options.UseFont = true;

        gridView.Appearance.Row.Font = GridRowFont;
        gridView.Appearance.Row.Options.UseFont = true;

        gridView.Appearance.FooterPanel.Font = GridHeaderFont;
        gridView.Appearance.FooterPanel.Options.UseFont = true;

        gridView.Appearance.FilterPanel.Font = GridRowFont;
        gridView.Appearance.FilterPanel.Options.UseFont = true;
    }
}
```

## Compatibilidad con Visual Studio Designer

Para que el diseÃ±ador muestre la tipografÃ­a correctamente:

- Poner las asignaciones visuales en `.Designer.cs` cuando el formulario es diseÃ±ado manualmente.
- Evitar que la fuente solo se aplique en eventos como `Load`, porque Visual Studio Designer no siempre ejecuta esa lÃ³gica.
- En formularios manuales, aplicar campo por campo:

```csharp
lblCustomerCode.Appearance.Font = AppTypography.LabelFont;
lblCustomerCode.Appearance.Options.UseFont = true;

txtCustomerCode.Properties.Appearance.Font = AppTypography.InputFont;
txtCustomerCode.Properties.Appearance.Options.UseFont = true;

btnSave.Appearance.Font = AppTypography.ButtonFont;
btnSave.Appearance.Options.UseFont = true;

grvCustomers.Appearance.HeaderPanel.Font = AppTypography.GridHeaderFont;
grvCustomers.Appearance.HeaderPanel.Options.UseFont = true;
grvCustomers.Appearance.Row.Font = AppTypography.GridRowFont;
grvCustomers.Appearance.Row.Options.UseFont = true;
```

## Ejemplo de Uso: CustomerListForm

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using NuanSystem.WinForms.Forms.Common;

namespace NuanSystem.WinForms.Forms.Customers;

public partial class CustomerListForm : XtraForm
{
    private LabelControl lblTitle;
    private SimpleButton btnCreate;
    private SimpleButton btnRefresh;
    private GridControl grdCustomers;
    private GridView grvCustomers;

    private void InitializeComponent()
    {
        lblTitle = new LabelControl();
        btnCreate = new SimpleButton();
        btnRefresh = new SimpleButton();
        grdCustomers = new GridControl();
        grvCustomers = new GridView();

        AppTypography.ApplyToForm(this);

        lblTitle.Text = "Maestro de Clientes";
        lblTitle.Location = new Point(16, 16);
        AppTypography.ApplyTitle(lblTitle);

        btnCreate.Text = "Nuevo";
        btnCreate.Location = new Point(16, 56);
        AppTypography.ApplyButton(btnCreate);

        btnRefresh.Text = "Actualizar";
        btnRefresh.Location = new Point(110, 56);
        AppTypography.ApplyButton(btnRefresh);

        grdCustomers.Location = new Point(16, 96);
        grdCustomers.Size = new Size(980, 420);
        grdCustomers.MainView = grvCustomers;
        grdCustomers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] { grvCustomers });
        grdCustomers.Font = AppTypography.BaseFont;

        grvCustomers.OptionsView.ShowGroupPanel = false;
        grvCustomers.OptionsView.ShowFooter = true;
        AppTypography.ApplyGrid(grvCustomers);

        Controls.AddRange(new Control[] { lblTitle, btnCreate, btnRefresh, grdCustomers });
        Text = "NuanSystem ERP - Clientes";
        ClientSize = new Size(1024, 560);
    }
}
```

## Checklist de AplicaciÃ³n

Antes de entregar un formulario DevExpress:

- Verificar que el formulario usa `Segoe UI`.
- Verificar que labels, inputs, botones y grids tienen fuente explÃ­cita.
- Verificar que los tÃ­tulos de secciÃ³n no usan fuentes mÃ¡s grandes que el tÃ­tulo principal.
- Verificar que no hay fuentes prohibidas.
- Verificar que el Designer muestra el resultado sin depender de lÃ³gica en runtime.
- Compilar el proyecto WinForms.

