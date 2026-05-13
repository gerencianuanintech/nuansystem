# Fase 10 - Integracion SAP Business One

## Objetivo

La fase 10 incorpora la primera capa tecnica para sincronizar documentos comerciales de NuanSystem hacia SAP Business One, manteniendo la arquitectura desacoplada por capas.

La API no conoce directamente el cliente SAP. El flujo pasa por Application, usa contratos, consulta el documento en la base tenant y delega el envio al proyecto especializado `NuanSystem.SapIntegration`.

## Componentes implementados

### Application

Se agregaron contratos y casos de uso para SAP:

- `ISapDocumentSender`: contrato para enviar documentos hacia SAP.
- `ISapSyncLogRepository`: contrato para registrar y consultar sincronizaciones.
- `SendDocumentToSapCommand`: caso de uso para enviar un documento por id.
- `GetSapSyncLogsQuery`: consulta de bitacora de sincronizacion SAP.
- DTOs de resultado y log: `SapSendResultDto`, `SapSyncLogDto`, `CreateSapSyncLogData`.

El handler de envio valida el id del documento, delega en `ISapDocumentSender` y retorna un resultado estandar usando `Result<T>`.

### SapIntegration

Se creo la base de integracion SAP:

- `ISapClient`: contrato comun para clientes SAP.
- `ISapClientFactory`: selecciona el cliente segun el modo de integracion de la empresa.
- `SapServiceLayerClient`: cliente inicial para SAP Service Layer.
- `SapDiApiClient`: cliente reservado para DI API.
- `SapDocumentSender`: orquestador que carga el documento, construye el payload, envia a SAP y registra el resultado.
- Payloads: `SapDocumentPayload`, `SapDocumentLinePayload`, `SapClientResult`.

El cliente Service Layer mapea tipos de documento iniciales:

- `SalesOrder` -> `Orders`
- `Delivery` -> `DeliveryNotes`
- `Invoice` -> `Invoices`

### Persistence

Se agrego `SapSyncLogRepository` para trabajar contra la tabla tenant `dbo.SapSyncLog`.

Funciones principales:

- Crear registros de sincronizacion SAP.
- Consultar logs recientes de la empresa activa.
- Guardar estado, mensaje, codigo de error, `DocEntry`, `DocNum` y fecha de intento.

### API

Se agregaron endpoints protegidos por autenticacion:

- `POST /api/sap/send-document/{documentId:long}`
- `GET /api/sap/sync-logs`

Ambos usan MediatR y respetan el contexto multiempresa mediante el header `X-Company-Code`.

## Configuracion agregada

En `appsettings.json` y `appsettings.Local.json` se agrego:

```json
"Sap": {
  "ServiceLayer": {
    "BaseUrl": ""
  }
}
```

`BaseUrl` debe apuntar al Service Layer de SAP Business One cuando se active un ambiente real.

## Flujo de envio

1. El usuario autenticado llama `POST /api/sap/send-document/{documentId}`.
2. El middleware resuelve la empresa activa con `X-Company-Code`.
3. `SendDocumentToSapCommandHandler` ejecuta el caso de uso.
4. `SapDocumentSender` carga el documento desde la base tenant.
5. Se selecciona el cliente SAP segun el modo de integracion de la empresa.
6. El cliente SAP envia el documento.
7. Se registra el intento en `dbo.SapSyncLog`.
8. La API responde con el estado de sincronizacion.

## Limites actuales

Esta fase deja lista la estructura tecnica, pero todavia no completa la conexion productiva con SAP:

- Service Layer aun no implementa login, sesion ni cookies de SAP.
- DI API queda como contrato preparado, porque requiere SDK/COM instalado en el servidor Windows.
- La configuracion SAP por empresa ya existe conceptualmente en master, pero el cliente actual usa `Sap:ServiceLayer:BaseUrl` global.
- El payload inicial cubre documento, cliente, fechas, moneda, comentarios y lineas; falta enriquecer impuestos, almacenes, series, vendedores y reglas especificas de SAP.

## Verificacion

Se verifico la solucion completa:

```text
dotnet build NuanSystem.sln --no-restore
0 Advertencia(s)
0 Errores
```

Tambien se ejecuto la API con el perfil HTTP:

```text
GET http://localhost:5081/health
200 Healthy
```

El aviso `Failed to determine the https port for redirect` aparece al usar el perfil HTTP y no impide que la API funcione.
