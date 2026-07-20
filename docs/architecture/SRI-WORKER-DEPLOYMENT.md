# SRI Worker - despliegue y validacion controlada

## Estado y limite

Este runbook aplica a la Fase 5.3 implementada. El script `117` ya fue desplegado y SQL-validado en los tres tenants piloto. El documento no autoriza por si mismo habilitar el worker, llamar al SRI ni operar en Production; cada accion restante requiere alcance explicito del propietario.

## Evidencia SQL cerrada el 2026-07-20

- script `117` ejecutado dos veces sin duplicados en `NuanSystem_DEMO`, `NuanSystem_DEMO_REMIGIO` y `NuanSystem_DEMO_CANARIS`;
- `NuanSystem_Master` sin version ni objetos de tenant;
- claim concurrente exclusivo y filtrado por ambiente;
- recuperacion de lease vencido y proteccion contra propietario ajeno;
- almacenamiento `Authorized` atomico, repeticion idempotente y conflicto SHA-256 estable;
- rechazo SQL de contenido mayor de 5 MiB;
- fixtures eliminados y working tree limpio;
- worker deshabilitado, cero procesos y cero llamadas al SRI.

## Precondiciones

1. Rama y commit aprobados; working tree limpio.
2. Respaldo y ventana de cambio para cada base tenant objetivo.
3. `115_tenant_sri_document_queue.sql` ya instalado y verificado.
4. Feature `SRI_DOCUMENTS` e integracion `SRI` habilitadas solo en la empresa piloto.
5. `SqlServerAdmin` y `Security:EncryptionKey` provistos fuera del repositorio.
6. Red/TLS hacia los hosts oficiales autorizada; no se permite omitir validacion de certificado.

## Orden de despliegue

1. Mantener `SriWorker:Enabled=false`; iniciar el proceso y comprobar configuracion/health sin claims ni llamadas remotas.
2. Preparar un trabajo controlado cuya llamada de red permanezca bloqueada o sustituida por un proveedor local de prueba aprobado.
3. Habilitar una sola instancia y verificar claim, intento, cancelacion y apagado controlado.
4. Validar reinicio y luego dos instancias sin duplicar claims ni intentos.
5. Deshabilitar el worker al cerrar la ventana y conservar evidencia saneada.
6. Solicitar autorizacion separada antes de cualquier round trip al ambiente oficial Test.

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
