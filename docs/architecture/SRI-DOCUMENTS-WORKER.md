# Arquitectura objetivo SRI, documentos electronicos y Worker Service

> **Estado de implementacion:** arquitectura objetivo. La ejecucion de Iteracion 5 y sus quality gates se controlan en [SRI-ITERATION-5-BLUEPRINT.md](SRI-ITERATION-5-BLUEPRINT.md). El primer piloto aprobado es la [consulta y descarga de comprobantes autorizados](SRI-CONSULT-DOWNLOAD-PILOT-CONTRACT.md). La existencia de estos documentos no prueba una cola ni un worker operativos.

Este documento define el modulo SRI centralizado de NuanSystem. Complementa la arquitectura comercial y multiempresa existente. El modulo SRI no depende de SAP Business One.

## Principios

- SRI es una capacidad propia de NuanSystem.
- SAP no es requisito para emitir, consultar, descargar ni procesar documentos SRI.
- TXT, AddOn SAP, formularios WinForms, importadores y APIs externas solo alimentan una cola SRI.
- El Worker SRI es el unico componente que descarga, autoriza, consulta, procesa y almacena XML.
- WinForms no procesa XML ni llama servicios SRI directamente.
- La API expone comandos, consultas y estado; el trabajo pesado corre en worker.

## Componentes

### Capturadores

Los capturadores reciben datos desde diferentes origenes:

- Documento creado en NuanSystem.
- Archivo TXT.
- AddOn SAP.
- Formulario administrativo.
- Integracion externa autorizada.

Su responsabilidad termina al validar el minimo necesario y crear un registro en la cola SRI.

### Cola SRI

La cola SRI es el contrato central entre capturadores y worker.

Campos minimos recomendados:

- `SriQueueId`
- `CompanyCode`
- `BranchCode`
- `Environment`
- `DocumentType`
- `AccessKey`
- `SourceType`
- `SourceReference`
- `PayloadJson`
- `Status`
- `Priority`
- `RetryCount`
- `NextAttemptAt`
- `TraceId`
- Auditoria de creacion y actualizacion.

Estados recomendados:

- `Pending`
- `Validating`
- `Submitted`
- `Authorized`
- `Rejected`
- `DownloadPending`
- `Downloaded`
- `Processed`
- `Failed`
- `RetryScheduled`
- `DeadLetter`

### Worker SRI

El Worker SRI ejecuta:

- Seleccion de trabajos pendientes.
- Validacion tecnica previa.
- Envio/consulta contra servicios SRI cuando aplique.
- Descarga de XML autorizado.
- Parseo y normalizacion.
- Persistencia de XML, autorizacion, mensajes y estado.
- Reintentos controlados.
- Registro de errores tecnicos y funcionales.

El worker debe ser idempotente. Reprocesar el mismo `AccessKey` no debe duplicar documentos ni XML.

## Flujo objetivo

```text
Origen TXT/AddOn/Form/API
  -> API NuanSystem
  -> SriDocumentQueue
  -> SRI Worker Service
  -> Servicio SRI / descarga XML
  -> SriDocumentStore
  -> Consultas API/WinForms
```

## Almacenamiento

La base definida por la politica de empresa/sucursal debe almacenar:

- Cola SRI.
- XML autorizado.
- Metadatos normalizados.
- Mensajes de autorizacion/rechazo.
- Historial de intentos.
- Relacion con documento comercial local cuando exista.

El XML puede almacenarse en base de datos o storage externo configurado, pero la referencia y trazabilidad deben quedar en base.

## Relacion con documentos comerciales

Un documento comercial puede existir antes de SRI, pero no debe mezclar la logica SRI en `Domain`.

Reglas:

- `Domain` define estados comerciales.
- `Application` coordina la solicitud SRI mediante contratos.
- SRI mantiene su propio estado tecnico.
- La autorizacion SRI puede actualizar referencias del documento local mediante un caso de uso explicito.

## Relacion con SAP

SRI y SAP son integraciones separadas.

- Un documento generado en SAP puede llegar via AddOn/TXT y alimentar la cola SRI.
- Un documento generado en NuanSystem puede enviarse a SRI sin SAP.
- Un documento sincronizado con SAP puede compartir referencia comercial, pero no comparte pipeline tecnico.
- Fallas SAP no bloquean el worker SRI salvo una regla de negocio explicitamente configurada.

## Seguridad y auditoria

- Certificados, claves, tokens y credenciales SRI se guardan protegidos.
- Los logs no deben exponer secretos ni XML completo si contiene datos sensibles.
- Cada intento debe registrar `TraceId`, origen, usuario o proceso, empresa/sucursal y resultado.
- Las descargas y reprocesos manuales requieren permiso backend.

## Pendientes de implementacion

- Crear contratos `Application` para encolar, consultar y reprocesar documentos SRI.
- Crear tablas versionadas para cola, XML, intentos y configuracion SRI.
- Crear `NuanSystem.SriWorker` como Worker Service.
- Definir estrategia de almacenamiento XML por empresa.
- Agregar pantallas WinForms de monitoreo, reproceso y configuracion sin logica SRI local.

