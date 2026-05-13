# Fase 12 - Modulos operativos WinForms

## Objetivo

La fase 12 convierte el shell WinForms en una aplicacion operativa inicial. Se conectaron los modulos principales del menu contra la API REST:

- Clientes.
- Articulos.
- Documentos.
- Envio de documentos a SAP.

El frontend mantiene la regla arquitectonica principal: no accede a base de datos ni contiene reglas de negocio. Todo pasa por servicios HTTP reutilizables y viewmodels.

## Servicios HTTP agregados

### Clientes

Archivos:

- `src/Frontend/NuanSystem.WinForms.Services/Customers/CustomerClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Customers/ICustomerClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Customers/Models/CustomerItem.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Customers/Models/SaveCustomerRequest.cs`

Endpoints consumidos:

- `GET /api/customers`
- `POST /api/customers`
- `PUT /api/customers/{id}`
- `DELETE /api/customers/{id}`

### Articulos

Archivos:

- `src/Frontend/NuanSystem.WinForms.Services/Items/ItemClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Items/IItemClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Items/Models/ItemItem.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Items/Models/SaveItemRequest.cs`

Endpoints consumidos:

- `GET /api/items`
- `POST /api/items`
- `PUT /api/items/{id}`
- `DELETE /api/items/{id}`

### Documentos

Archivos:

- `src/Frontend/NuanSystem.WinForms.Services/Documents/DocumentClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Documents/IDocumentClient.cs`
- `src/Frontend/NuanSystem.WinForms.Services/Documents/Models/DocumentSummaryItem.cs`

Endpoint consumido:

- `GET /api/documents`

El envio a SAP usa el cliente creado en la fase 11:

- `POST /api/sap/send-document/{documentId}`

## ViewModels agregados

- `CustomersViewModel`: carga clientes y ejecuta crear, editar y eliminar.
- `ItemsViewModel`: carga articulos y ejecuta crear, editar y eliminar.
- `DocumentsViewModel`: carga documentos y permite enviar el seleccionado a SAP.

Los viewmodels coordinan el estado de pantalla, pero delegan persistencia y sincronizacion a los servicios HTTP.

## Formularios agregados

### Clientes

- `CustomersForm`
- `CustomerEditForm`

Funciones:

- Listar clientes.
- Crear cliente.
- Editar cliente.
- Eliminar cliente de forma logica mediante la API.

### Articulos

- `ItemsForm`
- `ItemEditForm`

Funciones:

- Listar articulos.
- Crear articulo.
- Editar articulo.
- Eliminar articulo de forma logica mediante la API.

### Documentos

- `DocumentsForm`

Funciones:

- Listar documentos comerciales.
- Enviar documento seleccionado a SAP.

La creacion visual de documentos queda pendiente porque requiere una pantalla maestro-detalle con seleccion de cliente, articulos y calculo de lineas.

## Menu principal

`MainForm` ahora abre formularios reales para:

- Clientes.
- Articulos.
- Documentos.
- Integracion SAP.

El modulo de configuracion permanece reservado para una fase posterior.

## Consideraciones

Por ahora se mantienen controles WinForms nativos (`DataGridView`, `TextBox`, `Button`) para no introducir dependencia visual adicional antes de validar los flujos. La arquitectura permite reemplazarlos luego por controles DevExpress:

- `GridControl` para grillas.
- `XtraForm` para formularios.
- `RibbonControl` o `AccordionControl` para navegacion.
- `TextEdit`, `MemoEdit`, `LookUpEdit` para captura.

## Verificacion

Se compilo la solucion completa:

```text
dotnet build NuanSystem.sln --no-restore
0 Advertencia(s)
0 Errores
```

Para probar la fase visualmente:

1. Ejecutar la API.
2. Ejecutar WinForms.
3. Iniciar sesion.
4. Seleccionar empresa.
5. Abrir Clientes, Articulos o Documentos desde el menu principal.

La API debe tener acceso correcto a SQL Server y a la base tenant para que los listados y operaciones funcionen.
