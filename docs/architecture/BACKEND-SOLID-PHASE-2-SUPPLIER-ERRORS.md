# Consolidación backend — errores y cancelación de proveedores SAP

Fecha: 2026-10-23. Alcance: fase 2 del plan `docs/superpowers/plans/2026-10-22-backend-solid-consolidation.md`.

## Decisiones

- Mantener los métodos y estados de importación existentes, identidad de socios, SQL y reglas de negocio. No modificar `Result`, `ApiResponse`, el mapper HTTP ni la política general de retry.
- Propagar `OperationCanceledException` sin transformarla en Failed. Comprobar el token al entrar, entre operaciones y antes de devolver el resumen/avanzar watermark, incluso cuando una dependencia simulada no respete el token.
- Una cancelación no revierte filas ya confirmadas. Puede dejar un Inbox Processing; su recuperación durable sigue perteneciendo al endurecimiento SAP. Esta entrega no inventa una transacción global del lote ni una garantía de replay completa.
- Un fallo inesperado al importar una fila produce Failed y permite continuar el lote; errores del lector, carga inicial, Inbox, log o watermark no se convierten en éxito fabricado.
- Ampliar de manera aditiva el DTO existente de resultado por fila con `ErrorCode` y `CorrelationId`, propiedades no posicionales omitidas si son null. Se preservan constructor, deconstrucción, campos y estados existentes. Los clientes WinForms siguen consumiendo los campos actuales; el mensaje seguro incluye la referencia para esos clientes.
- Usar el código ya establecido `SUPPLIER_IMPORT_FAILED`. Compartir la correlación de opciones con Inbox y logging; generar referencia para llamadas individuales o una correlación vacía.
- Reutilizar `ILogger<T>` ya disponible, con dependencia opcional al final del constructor para preservar instanciaciones existentes. DI productiva suministra el logger; no se crea infraestructura paralela de errores.
- Registrar únicamente código, correlación, empresa cuando se conoce y tipo de excepción. No registrar `Exception.Message`, excepción completa, stack/payload ni datos personales, porque pueden contener conexiones o credenciales.
- Mantener HTTP 200 con resumen parcial en el endpoint manual, y Failed en el resultado del handler worker cuando hay errores por fila. Cancelación no produce ninguno de esos resúmenes finales. La migración de categorías HTTP es fase 3.

## Discovery y preservación

Referencias: habilidades backend/error-logging/SAP/testing y reglas del proyecto; importador actual, handler manual, handler worker, DTO, contratos Inbox/watermark/log, middleware de excepciones y logger del importador de artículos. Se reutilizan decisiones de lectura y baseline de fase 1; no se aplica la captura de cancelación del importador de artículos como patrón.

Se trabaja sobre `codex/auxiliary-master-generator` conservando la edición previa del importador, incluyendo generación de código mediante `SapBusinessPartnerIdentityGenerator`. Huellas y copia previa del archivo en `artifacts/backend-solid-phase2`; no stash/reset/commit de trabajo ajeno. Solo se autoriza modificar el importador y su DTO; nuevos tests/documentación son aditivos. No se activa SAP, SRI, workers, API ni conexiones reales.

## Gates

Pruebas con mocks: no filtración a DTO/Inbox/log público/envelope HTTP; diagnóstico seguro y correlacionado; fallo parcial; cancelación antes y durante el lote, última fila y registro del resumen; no watermark exitoso en error/cancelación; compatibilidad del endpoint manual y del handler worker; llamada individual con correlación propia.

La validación incluye las reglas de arquitectura y composición de fase 1. SQL externo y piloto SAP permanecen excluidos explícitamente y deshabilitados mediante variables del proceso. No se acredita integración SAP real ni se corrigen los tres fallos WinForms previos.
