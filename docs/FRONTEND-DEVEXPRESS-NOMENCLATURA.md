# Frontend - DevExpress y nomenclatura

## Regla general

Todos los formularios nuevos o modificados del frontend WinForms deben usar DevExpress.

Base de formularios:

```text
XtraForm
```

Controles permitidos y prefijos:

| Control DevExpress | Prefijo | Ejemplo |
| --- | --- | --- |
| `XtraForm` | `frm` | `frmLogin` |
| `SimpleButton` | `btn` | `btnContinuar` |
| `TextEdit` | `txt` | `txtUsuario` |
| `LabelControl` | `lbl` | `lblTitulo` |
| `PictureEdit` / imagen | `pic` | `picLogo` |
| `ComboBoxEdit` | `cmb` | `cmbEstado` |
| `LookUpEdit` | `lue` | `lueEmpresa` |
| `GridControl` | `grc` | `grcUsuarios` |
| `GridView` | `grv` | `grvUsuarios` |
| `SpinEdit` | `sed` | `sedCantidad` |
| `TreeList` | `trl` | `trlMenu` |
| `ListBoxControl` | `lst` | `lstRoles` |
| `PanelControl` / panel | `pnl` | `pnlContenido` |
| `CheckEdit` | `chk` | `chkActivo` |
| `DateEdit` | `dtp` | `dtpFecha` |
| `MemoEdit` | `mem` | `memObservacion` |

## Regla para controles nuevos

Si un formulario requiere un control que no esta en la tabla anterior, se debe confirmar antes de usarlo.

Equivalencias aprobadas:

| Uso actual | Posible equivalente DevExpress |
| --- | --- |
| `DataGridView` | `GridControl` + `GridView` |
| `NumericUpDown` | `SpinEdit` |
| `TreeView` | `AccordionControl` o `TreeList` |
| `ListBox` | `ListBoxControl` |
| `Panel` / `FlowLayoutPanel` | `PanelControl`, `XtraScrollableControl`, `LayoutControl` |
| `CheckBox` | `CheckEdit` |
| `DateTimePicker` | `DateEdit` |
| `TextBox` multiline | `MemoEdit` |

## Colores base

Color principal de accion:

```text
RGB 0, 184, 148
HEX #00B894
```

Hover:

```text
RGB 0, 161, 132
HEX #00A184
```

Pressed:

```text
RGB 0, 141, 116
HEX #008D74
```

Texto sobre botones principales:

```text
#FFFFFF
```
