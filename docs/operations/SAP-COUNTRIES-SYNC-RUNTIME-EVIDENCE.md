# Evidencia runtime — Países SAP → DEMO

## Resultado

- Fecha: 2026-08-04.
- Empresa y tenant SAP: `DEMO` / `NuanSystem_DEMO`.
- Dirección: SAP Business One → NuanSystem.
- Modo: Full, sin filtros.
- Tabla funcional SAP: `OCRY`; entity set Service Layer: `Countries`.
- Estado: SQL, SAP→DEMO y distribución DEMO→sucursales validados.

## Respaldos

Los siguientes respaldos `COPY_ONLY WITH CHECKSUM` aprobaron
`RESTORE VERIFYONLY WITH CHECKSUM`:

- `/var/opt/mssql/data/NuanSystem_Master_Countries_20260804_145032.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_Countries_20260804_145032.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_REMIGIO_Countries_20260804_145032.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_CANARIS_Countries_20260804_145032.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_CountryLinks_20260804_150434.bak`;
- `/var/opt/mssql/data/NuanSystem_Master_CountryDistribution_20260804_151054.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_CountryDistribution_20260804_151054.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_REMIGIO_CountryDistribution_20260804_151054.bak`;
- `/var/opt/mssql/data/NuanSystem_DEMO_CANARIS_CountryDistribution_20260804_151054.bak`.

## Despliegue SQL

- `083` se ejecutó dos veces en Remigio y Cañaris para instalar el contrato base.
- `168` y `169` se ejecutaron dos veces en DEMO, Remigio y Cañaris.
- `170` y `171` se ejecutaron dos veces en Master.
- Cada versión quedó registrada exactamente una vez.
- Los tres tenants tienen índices únicos de código y referencia externa,
  aplicador por `GlobalId` y snapshot `CountryV1`.
- Master registra Countries como capacidad `SapToErp + Full`, sin Incremental
  ni ERP→SAP.
- La navegación quedó en `Definiciones → General`.
- No se activaron perfiles ni agendas SAP.

## Metadata y preview SAP

La sesión temporal usó la configuración cifrada existente y TLS estricto. No
se registraron URL, usuario, contraseña, cookies ni payload de login.

- Entity set: `Countries`.
- Entity type: `Country`.
- Campos confirmados: `Code`, `Name`, `ISOAlpha2Code`, `ISOAlpha3Code` e
  `ISONumeric`.
- Registros leídos: 250.
- Nuevos en el primer preview: 247.
- Coincidencias por código pendientes de aprobación: `CO`, `EC`, `PE`.
- Conflictos de identidad: 0.

## Full e identidad

Primer Full:

- 247 creados;
- 3 `ApprovalRequired`;
- 0 actualizados, conflictos o fallos.

Segundo Full antes de aprobar vínculos:

- 247 `Unchanged`;
- 3 `ApprovalRequired`;
- 0 creaciones, actualizaciones, conflictos o fallos.

La aprobación expresa vinculó `CO`, `EC` y `PE` con `SAP_B1 + Code` mediante
`UpdateCountryCommand`. Para los tres se comprobó que `GlobalId`,
`PhonePrefix` e `IsActive` permanecieron iguales. Los nombres e ISO coincidían
con SAP antes de vincularlos.

Full final:

- 250 leídos;
- 250 `Unchanged`;
- 0 creaciones, actualizaciones, aprobaciones, conflictos o fallos.

## Distribución DEMO → sucursales

La distribución fue autorizada expresamente y se ejecutó con perfiles
temporales exclusivos para `Countries`. El worker se inició solamente durante
la entrega, con el aplicador permitido `Countries`; SAP y SRI permanecieron
inactivos.

Remigio, modo Incremental mediante `LocalOutbox`:

- perfil `5002`, `COUNTRIES-REMIGIO-20260804`;
- 250 eventos promovidos y 250 targets aplicados;
- 250 países reconciliados contra DEMO;
- 0 faltantes, diferencias, extras, duplicados o fallos.

Durante la preparación del perfil Remigio, el primer JSON usó casing no
compatible con el procedimiento desplegado y dejó solamente la cabecera. La
misma cabecera se reparó mediante el procedimiento oficial de actualización y
el worker no se inició hasta validar toda la configuración.

Cañaris, modo Full mediante `SyncProfileExecutionService`:

- perfil `5003`, `COUNTRIES-CANARIS-20260804`;
- ejecución `6005`, estado `Completed`;
- 250 registros leídos, 250 eventos publicados y 0 errores;
- 250 targets aplicados;
- 250 países reconciliados contra DEMO;
- 0 faltantes, diferencias, extras, duplicados o fallos.

Antes del arranque efectivo de Cañaris hubo dos intentos sin reclamación: el
primero usó un nombre de sección incorrecto y el segundo inició desde una
carpeta que no contenía la configuración Local. En ambos casos los 250 targets
permanecieron `Pending`. Se detuvieron esos procesos y el arranque corregido,
desde la carpeta del worker y con `MasterBranchSyncWorker`, aplicó 250/250.

Las colas ajenas conservaron sus cantidades de control: 1.417 eventos Master,
2.206 targets Master y 9 eventos locales DEMO. El reclamo del worker estuvo
limitado por entidad a `Countries`.

## Estado final

- Países visibles en DEMO: 250.
- Países vinculados a `SAP_B1`: 250.
- Referencias externas duplicadas: 0.
- Eventos `LocalOutbox` Countries: 250 `Applied`.
- Locks SAP Countries: 0.
- Perfiles SAP activos: 0.
- Targets Countries: Remigio 250 `Applied`, Cañaris 250 `Applied`.
- Remigio y Cañaris: 250 países cada una, idénticos a DEMO en identidad,
  código, nombre, ISO, prefijo telefónico, estado y referencia externa.
- Perfiles temporales `5002` y `5003`: inactivos.
- Locks Master/Branch Countries: 0; errores y dead letters: 0.
- `MasterBranchSyncWorker` quedó detenido y su configuración local continúa
  deshabilitada.
- No se llamó SRI ni se escribió hacia SAP.

## Verificación posterior

- Pruebas dirigidas `FullyQualifiedName~Country`: 36 aprobadas, 0 fallidas.
- La utilidad temporal de ejecución fue retirada con sus artefactos `bin/obj`.
- No quedó ningún proceso `NuanSystem.MasterBranchSyncWorker.dll` activo.
- No se realizó commit, push ni integración a `master`.
