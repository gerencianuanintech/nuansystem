# Monitor de documentos SRI

## Discovery record

- Arquitectura revisada: Constitution/Kernel, Framework/Pattern Catalogs, Knowledge Graph, Review Checklist y contratos SRI de Iteracion 5.
- Estructuras existentes reutilizadas: `SriDocumentQueue`, `SriDocumentAttempts`, `SriAuthorizedDocuments` y `AuditSriDocumentChanges`; no se creo otra tabla de documentos ni auditoria.
- Referencia frontend: patron P5 Dashboard/Monitor de `SyncMonitorForm`, exclusivamente para ciclo visual y composicion. No se reutilizaron `SyncOutbox`, sus endpoints ni sus workers.
- Framework reutilizado: MediatR, `Result<T>`, repositorio Dapper tenant, permisos existentes, `INuanApiClient`, `NuanDataGridControl`, `NuanKpiCardControl`, `NuanActionButton`, `FormStyler` y navegacion dinamica por `FormKey`.
- Gap confirmado: el transporte JSON no podia preservar un archivo binario; se agrego `GetFileAsync` al cliente central sin crear `HttpClient` en la vertical.
- Scripts seleccionados: `118` tenant y `119` Master, siguientes numeros disponibles. No fueron ejecutados en bases reales.

## Alcance

`SriDocumentMonitorForm` consulta documentos registrados, detalle, intentos y auditoria, y descarga un XML previamente persistido por `NuanSystem.SriWorker`. No genera, firma, autoriza, anula, reintenta ni consulta al SRI.

## Seguridad

- `SRI.DOCUMENTS.VIEW`: menu, resumen, lista e intentos.
- `SRI.DOCUMENTS.VIEW_PAYLOAD`: detalle seguro y auditoria.
- `SRI.DOCUMENTS.DOWNLOAD_XML`: accion y endpoint de descarga.
- La empresa proviene del contexto autenticado y el repositorio abre solamente la base tenant activa.
- La pantalla no presenta XML, claves de acceso completas ni rutas del servidor.

## Descarga

Seleccione un documento `Authorized` con XML disponible, pulse **Descargar XML** y elija una ubicacion en `SaveFileDialog`. El cliente conserva los bytes y no abre el archivo. Cada exito agrega una auditoria `DownloadXml`; repetirlo no duplica el documento ni cambia el estado.

## Despliegue y validacion pendientes

1. Aplicar `118_tenant_sri_document_monitor_and_download.sql` en cada tenant autorizado.
2. Aplicar `119_master_sri_document_monitor_security.sql` en Master y renovar el JWT.
3. Validar perfiles con/sin vista, detalle y descarga, incluido aislamiento entre dos tenants.
4. Descargar un XML mediante API y confirmar headers, bytes, auditoria adicional y estado intacto.
5. Abrir el formulario en Visual Studio Designer y revisar layout, DPI, redimensionamiento y tema.

No se necesita habilitar el worker ni realizar otra llamada al SRI.
