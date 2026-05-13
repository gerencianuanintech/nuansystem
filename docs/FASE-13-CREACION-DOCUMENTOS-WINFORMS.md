# Fase 13 - Creacion de documentos desde WinForms

## Objetivo

La fase 13 agrega la creacion de documentos comerciales desde el frontend WinForms mediante una pantalla maestro-detalle.

Hasta la fase anterior el modulo de documentos permitia listar documentos y enviar el seleccionado a SAP. Ahora tambien permite crear documentos usando los endpoints existentes del backend.

## Funcionalidad agregada

### Servicios HTTP

Se extendio `DocumentClient` con soporte para crear documentos:

- `POST /api/documents`

Modelos agregados:

- `CreateDocumentRequest`
- `CreateDocumentLineRequest`
- `DocumentDetailItem`
- `DocumentLineItem`

Ubicacion:

- `src/Frontend/NuanSystem.WinForms.Services/Documents`

### ViewModel

`DocumentsViewModel` ahora puede:

- Cargar documentos existentes.
- Cargar clientes activos.
- Cargar articulos activos.
- Crear documentos.
- Enviar documentos a SAP.

Para esto recibe:

- `IDocumentClient`
- `ICustomerClient`
- `IItemClient`
- `ISapClient`

### Formulario maestro-detalle

Se agrego:

- `DocumentCreateForm`

Ubicacion:

- `src/Frontend/NuanSystem.WinForms.Forms/Documents/DocumentCreateForm.cs`

La pantalla permite:

- Seleccionar tipo de documento:
  - `SalesOrder`
  - `Delivery`
  - `Invoice`
- Seleccionar cliente.
- Seleccionar fecha.
- Definir moneda.
- Agregar lineas con articulo, cantidad, precio e impuesto.
- Quitar lineas.
- Ver subtotal, impuesto y total.
- Guardar el documento usando la API.

El calculo visual de totales es informativo. El backend recalcula y valida nuevamente los totales antes de guardar, manteniendo la regla de negocio fuera del formulario.

### Listado de documentos

`DocumentsForm` ahora tiene el boton:

- `Nuevo`

Al crear correctamente un documento, el listado se actualiza automaticamente.

## Flujo tecnico

1. El usuario abre el modulo `Documentos`.
2. Presiona `Nuevo`.
3. El formulario carga clientes y articulos activos desde la API.
4. El usuario completa cabecera y lineas.
5. El frontend envia `CreateDocumentRequest`.
6. La API valida cliente, articulos y lineas.
7. El backend recalcula subtotal, impuesto y total.
8. Se crea el documento y sus lineas en la base tenant.
9. El frontend muestra el numero generado.
10. El listado se refresca.

## Consideraciones

- La creacion usa documentos en estado `Draft`.
- El backend genera el numero con el formato actual: `{DOCUMENTTYPE}-{ID}`.
- El impuesto se captura en porcentaje visual y se envia como tasa decimal.
- El formulario requiere al menos un cliente activo y un articulo activo.
- La pantalla sigue usando controles WinForms nativos; queda preparada para migracion visual a DevExpress.

## Verificacion

Se compilo la solucion completa:

```text
dotnet build NuanSystem.sln --no-restore
0 Advertencia(s)
0 Errores
```

Para probar la fase:

1. Ejecutar `NuanSystem.Api`.
2. Ejecutar `NuanSystem.WinForms`.
3. Iniciar sesion con `admin`.
4. Seleccionar empresa `DEMO`.
5. Crear al menos un cliente y un articulo.
6. Entrar a `Documentos`.
7. Presionar `Nuevo`.
8. Agregar lineas y guardar.
