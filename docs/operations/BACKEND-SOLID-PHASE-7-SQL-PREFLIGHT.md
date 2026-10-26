# Fase 7: verificador del entorno SQL — preparación offline

Fecha: 2026-10-25. Estado: **NO LISTO PARA SQL; fase 7 abierta**.

## Alcance y descubrimiento

Cambio de infraestructura de pruebas, sin cambios en Domain, Application, Persistence, API, WinForms, SAP ni scripts de bases. Se reutilizan el manifiesto aprobado, la fixture y `IPhase7SqlDestinationVerifier`. Referencias: tareas SQL 1–6, `Phase7SqlSessionProbe`, `Phase7SqlFaultSession`, contrato `SchemaHistory` de 003 y catálogos de arquitectura/transacciones. Se conserva la rama y el trabajo pendiente; no se requiere aislamiento mediante otro checkout porque estos cambios amplían la fixture existente.

Se aplican las skills de descubrimiento, pruebas, persistencia, estándares SQL, ejecución de planes, verificación y optimización de contexto. Las lecturas rectoras de arquitectura se reutilizan del trabajo precedente y se consultan sus secciones aplicables. No se modifica arquitectura de producto ni se registra un servicio nuevo en DI.

## Implementación

`Phase7SqlDestinationVerifier` implementa el contrato exigido por la fixture. Recibe exactamente la conexión abierta y transacción del escritor, o la conexión sin transacción del observador. No crea, abre, cierra ni dispone conexiones; no inicia transacciones, habilita gates, busca servidores o resuelve credenciales. No se instala como valor predeterminado de la fixture: el futuro harness debe suministrarlo explícitamente.

El lector predeterminado `Phase7SqlDestinationSqlReader` exige `SqlConnection` abierta y ejecutaría únicamente un SELECT de metadatos, con timeout y cancelación, en esos mismos handles. **Ese lector no se ejecutó en esta tarea.** Las pruebas inyectan observaciones simuladas; la prueba del lector predeterminado solo le entrega un proveedor falso, que rechaza antes de crear comandos.

Rechaza discrepancias de servidor/versión, base/id, marcador RunId/propósito, empresa, compatibilidad, collation, RCSI, usuario/login/login original, permisos, huellas y estado de sesión. El escritor exige TRANCOUNT=1 y XACT_STATE=1; el observador exige ambos en0. Rechaza marcadores P7 residuales y transacciones ajenas. No almacena ni reutiliza autorizaciones entre verificaciones.

Los errores de lectura se convierten en un código genérico sin texto/inner exception del proveedor. La cancelación solicitada se propaga. La fixture conserva las guardas de autorización, ventana, presupuesto, endpoint y cadena de conexión: este componente no sustituye esas validaciones.

## Contrato que deberá preparar y aprobar el laboratorio

- Propiedades extendidas de base: `Phase7.RunId`, `Phase7.Purpose=BackendTransactionValidation` y **`Phase7.CompanyId`**. Esta última hace explícita la pertenencia del laboratorio a la empresa del manifiesto; es un marcador autorizado, no una consulta a Master ni una prueba del acceso empresarial de producción. Su ausencia debe impedir ejecutar.
- Login original, login actual y usuario de base deben coincidir exactamente con el principal declarado para escritor/observador. No se admite impersonación, dbo/guest, sysadmin ni membresías de roles de base distintas de public. Los grants deberán prepararse directamente para estos principales de laboratorio.
- A los permisos de negocio del manifiesto se agregan exclusivamente **CONNECT y VIEW DEFINITION** como permisos técnicos obligatorios del harness. El observador continúa limitado a SELECT sobre datos; VIEW DEFINITION permite comprobar metadatos. No se modificó el formato del manifiesto ni su lista de permisos de negocio. No se concedió ningún permiso aquí.
- Permisos efectivos de servidor: CONNECT SQL y opcionalmente VIEW ANY DATABASE. Los de base deben coincidir con el conjunto esperado, permitiendo además presencia o ausencia de VIEW ANY COLUMN MASTER KEY DEFINITION y VIEW ANY COLUMN ENCRYPTION KEY DEFINITION, permisos de metadatos que public recibe por defecto desde SQL2016. Los de objetos/esquemas no pueden exceder los permisos de negocio y VIEW DEFINITION. Cualquier otro grant adicional incompatible detiene la corrida. Referencia: [matriz oficial de permisos Microsoft](https://download.microsoft.com/download/e/8/5/e856641f-48ba-428f-9892-667950569cab/Microsoft_SQL_Server_2017_and_Azure_SQL_Database_permissions_infographic.pdf). Los permisos de visibilidad introducidos en versiones nuevas siguen sujetos a caracterización real; no se admite automáticamente cualquier permiso cuyo nombre comience por VIEW.
- Base ONLINE, MULTI_USER, escribible, sin snapshot de base, TRUSTWORTHY ni encadenamiento entre bases. El laboratorio estricto rechaza módulos EXECUTE AS, firmas, CLR, sinónimos, tablas externas y triggers de ámbito de base. Los triggers de tabla propios se incluyen en el inventario y su revisión previa.

No se deben relajar estas condiciones para aceptar un entorno existente: preparar separadamente un laboratorio compatible y aprobarlo. La consulta no demuestra por sí sola ausencia de llamadas externas dentro de código SQL: las definiciones aprobadas y las barreras del harness siguen requiriendo revisión.

## Protocolo de huellas v1

SHA256 hexadecimal de los bytes **UTF-16LE sin BOM** del texto original. Sin normalización de saltos, espacios o definiciones. Para procedimientos se usa `sys.sql_modules.definition`; definiciones nulas/encriptadas se rechazan. Se comprueban todos los procedimientos del manifiesto, y el inventario global también incluye módulos adicionales.

`SchemaSha256` es el hash del JSON exacto producido por la consulta versionada en `Phase7SqlDestinationSqlReader.Query`: objetos/módulos y opciones, triggers, columnas/tipos/defaults/identidades, índices/columnas, checks, claves foráneas y filas completas de `dbo.SchemaHistory`. Las secciones se ordenan por nombres/ordinales. Se excluyen datos comerciales e identidad consumida. Las fechas e ids de SchemaHistory se incluyen deliberadamente: cada destino requiere su propio inventario y aprobación; no se presupone la misma huella entre bases nuevas.

No usar el hash de un archivo de migración como si fuera esta huella efectiva. Falta de SchemaHistory, visibilidad insuficiente o consulta no compatible debe producir rechazo, no un inventario vacío aprobado. Los hashes deben proceder de un inventario independiente autorizado; nunca se deben sustituir automáticamente con lo observado durante una corrida fallida.

## Verificación y límites

Pruebas offline: aceptación de escritor/observador, aislamiento A/B, handles, permisos excesivos/incompletos, metadata, versiones/marcadores/opciones, huellas/procedimientos, sesiones, sanitización y cancelación. El lector real está preparado en código, pero su sintaxis, mapeo, visibilidad de catálogos, permisos efectivos y comportamiento en las versiones SQL aprobadas **siguen pendientes de validación real autorizada**.

No se ejecutaron SAP, SQL, aprovisionamientos, resolución de secretos ni conexiones de datos. No se modificaron archivos preexistentes. Evidencias locales: `artifacts/backend-solid-phase7-sql-preflight/` (baseline de hashes, compilación y TRX).

Validación final: solución Release sin restauración, 0 errores y 0 advertencias; 1529 pruebas offline aprobadas, 0 fallidas/omitidas, incluidas 44 nuevas. Las 217 rutas del baseline conservan sus hashes. Gates SQL y SAP en0 y suites reales excluidas. La prueba del hallazgo de revisión pasó de fallo reproducido a aprobación en la regresión final.

Siguen pendientes los lectores de snapshots completos, suite SQL ejecutable, inventario/provisión autorizada, revisión de permisos/definiciones y las dos corridas reales exigidas por la tarea6. Implementar este verificador no habilita una ejecución ni cierra la fase.

Revisión independiente: se detectó el rechazo de los dos permisos de metadatos de public; una prueba offline reprodujo el fallo y se corrigió con una excepción limitada a esos dos nombres. Los permisos extra de escritura continúan rechazados. No hubo otros hallazgos importantes.
