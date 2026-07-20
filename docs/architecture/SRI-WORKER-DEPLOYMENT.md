# SRI Worker - despliegue y validacion controlada

## Estado y limite

Este runbook aplica a la Fase 5.3 implementada. No autoriza por si mismo ejecutar scripts, habilitar el worker, llamar al SRI ni operar en Production. Cada accion requiere el alcance explicito del propietario.

## Precondiciones

1. Rama y commit aprobados; working tree limpio.
2. Respaldo y ventana de cambio para cada base tenant objetivo.
3. `115_tenant_sri_document_queue.sql` ya instalado y verificado.
4. Feature `SRI_DOCUMENTS` e integracion `SRI` habilitadas solo en la empresa piloto.
5. `SqlServerAdmin` y `Security:EncryptionKey` provistos fuera del repositorio.
6. Red/TLS hacia los hosts oficiales autorizada; no se permite omitir validacion de certificado.

## Orden de despliegue

1. Ejecutar `117_tenant_sri_worker_and_document_store.sql` solo en tenants aprobados.
2. Confirmar `SchemaHistory = 20260720.117`, tabla, indices, FKs y cuatro procedimientos.
3. Mantener `SriWorker:Enabled=false`; iniciar el proceso y comprobar configuracion/health sin claims ni llamadas remotas.
4. Insertar o reutilizar un trabajo de ambiente Test aprobado sin exponer la clave en logs.
5. Habilitar una sola instancia y verificar claim, intento, lease, resultado y auditoria.
6. Validar reinicio/lease vencido y luego dos instancias con un trabajo controlado.
7. Validar respuesta repetida, checksum e identidad unica sin duplicar XML.
8. Deshabilitar el worker al cerrar la ventana y conservar evidencia saneada.

## Evidencia minima

- build y pruebas ejecutadas;
- version SQL y objetos instalados;
- valores efectivos no secretos de batch, concurrencia, lease, timeout e intentos;
- una sola consulta por lease vigente con dos instancias;
- recuperacion de lease vencido;
- intento, auditoria y estado final coherentes;
- XML autorizado con `SizeBytes`, SHA-256 e identidad unica;
- logs sin XML, RUC completo, clave completa, secretos ni tokens;
- resultado separado para Test y Production; una prueba Test no valida Production.

## Rollback operativo

Deshabilitar el worker detiene nuevos claims. No eliminar cola, intentos, auditoria ni XML. Script `117` es forward-only; cualquier correccion de esquema requiere un nuevo script versionado.
