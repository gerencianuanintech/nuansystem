# Estandar visual WinForms

Este documento define la base visual para formularios WinForms del sistema.

## Tipografia

- Formulario general: `Segoe UI 9F Regular`
- Etiquetas: `Segoe UI 9F Regular`
- Inputs: `Segoe UI 9F Regular`
- Botones: `Segoe UI 9F Semibold` o `Segoe UI 9F Regular`
- Titulos de seccion: `Segoe UI 11F Semibold`
- Titulos principales: `Segoe UI 14F - 16F Semibold/Bold`
- Grid filas: `Segoe UI 9F Regular`
- Grid encabezados: `Segoe UI 9F Semibold/Bold`
- Ribbon: dejar que DevExpress use su skin, manteniendo iconos y textos consistentes

## Colores

Usar siempre `BrandResources` en lugar de colores sueltos:

- Fondo: `BrandResources.Background`
- Superficies: `BrandResources.Surface`
- Color principal: `BrandResources.Primary`
- Hover principal: `BrandResources.PrimaryHover`
- Texto principal: `BrandResources.Text`
- Texto secundario: `BrandResources.MutedText`
- Bordes: `BrandResources.Border`

## Reglas practicas

- Aplicar `FormStyler.ApplyBase(this)` en formularios construidos por codigo.
- En formularios con designer, configurar `Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)`.
- En `LabelControl`, establecer explicitamente:

```csharp
label.Appearance.Font = FormStyler.LabelFont;
label.Appearance.ForeColor = BrandResources.Text;
label.Appearance.Options.UseFont = true;
label.Appearance.Options.UseForeColor = true;
```

- En grids, usar `Segoe UI 9F` para filas y `Segoe UI 9F Bold` para encabezados.
- Los botones principales deben usar `BrandResources.Primary` con texto blanco.
- Los botones secundarios deben mantenerse sobrios, con texto `BrandResources.Text`.
- Evitar `Tahoma`, `Arial`, `Times New Roman` y colores definidos directamente salvo casos puntuales.
