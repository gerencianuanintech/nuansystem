# Aprovisionamiento del laboratorio SQL y del monitor — fase 7

Estado: preparado para revisión offline. No se han creado bases, usuarios ni permisos; no se han abierto conexiones SQL ni ejecutado SAP. La fase 7 permanece abierta hasta obtener evidencia real autorizada. Preparar el paquete no concede autorización de ejecución.

## 1. Preparar y aprobar el destino

Responsable: administrador del laboratorio. Reservar una instancia SQL Server dedicada, versiones principales 15, 16 o 17, autenticación mixta ya habilitada, TLS válido y `model` sin objetos de usuario ni marcadores de laboratorio. No usar una instancia de producción, réplicas, copias de datos reales ni una instancia compartida. El script rechaza cualquier base de usuario cuyo nombre no corresponda al patrón estricto de laboratorio; admite restos de otros RunId sin reutilizarlos.

Registrar servidor exacto, versión completa, collation, compatibilidad, RCSI, dos CompanyId positivos distintos, administrador original y ventana UTC de hasta 24 horas. Un RunId nuevo determina ambas bases `NuanSystem_Phase7_{RunId:N}_A/B` y las cinco identidades. La semilla utiliza un IdentificationTypeId sintético reservado, igual en ambas bases. Si falta un requisito, detenerse: estos scripts no instalan SQL Server ni cambian configuración global, firewall o autenticación.

Obtener referencias independientes para: aprovisionamiento, ejecución de pruebas y supervisión del monitor. La autorización de preparación de archivos de esta conversación no sustituye ninguna de ellas.

## 2. Generar y revisar sin conexión

Copiar `database/lab/phase7/provisioning-spec.example.json` a un archivo de trabajo; sustituir los datos ficticios por los aprobados, sin secretos. El ejemplo está deliberadamente caducado. Ejecutar únicamente el generador local indicado en el README del laboratorio y las pruebas offline. Cada salida debe estar en una carpeta nueva.

Revisar `bundle-manifest.json`, `source-inventory.json`, hashes de todos los archivos y originales. El paquete conserva 16 scripts tenant, ordenados por dependencias; 028 precede a 030. Omite únicamente las tres bajas legacy iniciales de 024, verificando su forma, y excluye 063 porque mezcla Master y tenant. En 024/028, el paquete difiere la compilación de la restricción CK_BusinessPartnerPurchaseSettings_Values mediante EXEC, después de las adiciones de columnas del mismo lote. El chequeo estático encontró diez referencias en los originales; esta adaptación conserva la condición y expresión exactas sin modificar los archivos originales. No se registra el laboratorio en el historial de despliegues de producción. La consulta de inventario se obtiene del lector de la suite para compartir el protocolo de huellas.

La selección mínima y los hashes congelan qué se revisó; no prueban que SQL compile o que el esquema efectivo sea correcto. Los controles estáticos no sustituyen una ejecución futura sobre el laboratorio.

## 3. Aprovisionamiento futuro, separado de esta preparación

Ejecutar solo después de autorización expresa, usando un cliente administrativo revisado que preserve una sesión, se detenga ante el primer error y respete cada separador GO. No ejecutar todo con un cliente que continúe tras errores. La sesión debe establecer de forma parametrizada los contextos `P7.Provision.RunId`, `AuthorizationId`, `SpecSha256` y `Allow` con prefijo completo `P7.Provision.`; el valor de Allow es `CREATE_ISOLATED_LAB_ONLY`. El hash corresponde a los bytes exactos de `provisioning-spec.json`. No copiar valores de otro paquete.

| Orden | Archivo | Resultado esperado |
| --- | --- | --- |
| 1 | `00-admission.sql` | Servidor, versión, ventana, administrador y nombres libres verificados |
| 2 | `10-create-databases.sql` | Dos bases nuevas, opciones y marcadores exactos |
| 3 | `20-schema-A.sql`, después B | Esquema y diario ordenado; estado SchemaReady |
| 4 | `30-seed-A.sql`, después B | Identificación sintética, sin proveedores ni eventos; Seeded |
| 5 | `40-faults-A.sql`, después B | Tres triggers de fallo por base y RunId; FaultsReady |
| 6 | `50-principals-and-monitor.sql` | Cinco logins separados, usuarios y permisos; Prepared |
| 7 | `90-admin-inventory-A/B.sql` | Inventario de instalación e historial para revisión |
| 8 | `91-inventory-App/Observer-A/B.sql` | Inventario con cada identidad efectiva correspondiente |

Antes de 50, el cliente autorizado inyecta cinco secretos distintos en las claves exactas de `SecretSessionKeys` del manifiesto. Deben tener entre 32 y 128 caracteres y cumplir la política SQL. Nunca pasarlos en argumentos de consola, JSON, scripts, repositorio, salidas o registros. El script los obtiene de SESSION_CONTEXT, valida longitud antes de estrechar el tipo y limpia las cinco claves al terminar o fallar. La gestión segura del cliente y sus registros debe revisarse antes de ejecutar: los scripts no suministran ese cliente ni un almacén de secretos.

Los inventarios 91 usan sesiones de la identidad indicada, la misma vinculación del paquete y Allow=`INVENTORY_ISOLATED_LAB_ONLY`; no usan el administrador. El inventario administrativo, cuya identidad es dbo, no acredita permisos efectivos de aplicación. Los inventarios preparatorios no sustituyen el preflight transaccional de la suite.

Cada etapa verifica destino, RunId, propósito, empresa, estado y hash. El diario impide saltos o repetición de lotes. Las etapas avanzan Created → SchemaReady → Seeded → FaultsReady → Prepared. Un fallo puede dejar bases, objetos o logins parciales: conservarlos, registrar el error y detenerse. No hay rollback global, borrado ni reintento automático. Revisar el incidente y preparar otro RunId si corresponde; la retirada de recursos requiere un procedimiento y autorización separados.

## 4. Permisos y monitor independiente

| Identidad | Base | Permisos previstos |
| --- | --- | --- |
| App A/B | Solo su tenant | CONNECT, SELECT, INSERT, EXECUTE, VIEW DEFINITION |
| Observer A/B | Solo su tenant | CONNECT, SELECT, VIEW DEFINITION |
| Monitor | Ambos tenants; usuario en master | CONNECT, SELECT, VIEW DEFINITION; lectura de sesiones del servidor |

Solo el administrador de aprovisionamiento requiere sysadmin. Las cinco identidades no reciben roles, CONTROL, delegación de permisos ni capacidad de aprovisionar. En SQL Server 2019 el monitor recibe VIEW SERVER STATE; en SQL Server 2022/2025 recibe VIEW SERVER PERFORMANCE STATE. Confirmar las concesiones efectivas, incluidos privilegios heredados de public, antes de aceptar el entorno. Si difieren del contrato, detenerse; no ampliar el verificador para permitirlos.

En SQL Server 2022 los subpermisos VIEW SECURITY DEFINITION y VIEW PERFORMANCE DEFINITION están implicados por VIEW DEFINITION; el verificador reconoce solo esos dos nombres técnicos. Referencias: [permisos de base](https://learn.microsoft.com/en-us/sql/t-sql/statements/grant-database-permissions-transact-sql), [sesiones](https://learn.microsoft.com/en-us/sql/relational-databases/system-dynamic-management-views/sys-dm-exec-sessions-transact-sql), [transacciones](https://learn.microsoft.com/en-us/sql/relational-databases/system-dynamic-management-objects/sys-dm-tran-database-transactions-transact-sql).

El monitor existente recibe una conexión previamente abierta de manera autorizada, propia, sobre master y sin transacción. Debe vincular RunId, aprobación, principal, servidor, versión y ventana; rechaza reutilizar App/Observer. Comprueba las sesiones ApplicationName del RunId, ausencia de transacciones residuales y snapshots globales posteriores de las 16 tablas. Este paquete prepara sus permisos, no instala un servicio ni inicia el monitor.

## 5. Inventario y conexión con la validación existente

Obtener de la instalación real DB_ID, identidades efectivas, opciones, marcadores, permisos, historial, inventario y definiciones de procedimientos. Completar el manifiesto de ejecución con esos datos observados, los cuatro nombres de variables de entorno para conexiones App/Observer y las huellas esperadas. Las cadenas reales solo se suministran posteriormente por el mecanismo autorizado; usar TLS y ApplicationName por RunId, rol y tipo de sesión. No confundir los cinco secretos de creación con las cuatro conexiones de la suite y la conexión independiente del monitor.

Las huellas de esquema y procedimientos usan SHA-256 sobre UTF-16LE sin BOM de la cadena exacta del lector, sin normalizar saltos ni espacios. Los hashes de archivos del paquete son SHA-256 sobre sus bytes; no sirven como huellas SQL. No inventar DB_ID, hashes ni evidencia a partir del ejemplo offline.

Continuar con los planes [preflight](BACKEND-SOLID-PHASE-7-SQL-PREFLIGHT.md), [suite](BACKEND-SOLID-PHASE-7-SQL-SUITE.md) y [evidencias](BACKEND-SOLID-PHASE-7-SQL-EVIDENCE.md): congelar candidato y manifiesto, capturar baseline, ejecutar las 21 pruebas SQL autorizadas, cerrar el proceso, ejecutar el monitor independiente y empaquetar evidencias. Repetir con otro RunId para demostrar repetibilidad. Mantener los controles de ejecución desactivados mientras se prepara todo offline.

Cierre: preflight válido para ambas empresas e identidades; 21 resultados reales revisados; snapshots y transacciones concordantes; monitor aceptado; paquete íntegro y revisión humana de autenticidad. La caracterización R01 del error SQL 3915 continúa vigente y no se corrige mediante estos scripts. Cualquier resultado offline mantiene autenticidad y cierre global pendientes.

## Validación de esta preparación

El generador superó 99 comprobaciones offline, incluidas especificaciones inválidas, integridad, ausencia de sobrescritura, limpieza de secretos y permisos del monitor para versiones 15/16/17. El control estático de lotes aceptó los 15 scripts generados y los 314 cuerpos efectivos del esquema; una muestra negativa fue rechazada. Los originales 024/028 conservan sus advertencias y sus bytes; la adaptación descrita se aplica exclusivamente al paquete. La solución compiló en Release sin advertencias ni errores y la regresión offline aprobó 1689 pruebas, excluyendo las integraciones SQL/SAP. Los resultados de preparación se conservan en `artifacts/backend-solid-phase7-sql-provisioning`; ninguna de estas comprobaciones acredita una instalación SQL real.

## Uso del ejecutor controlado

La entrada es `tools/phase7-sql/Invoke-Phase7SqlProvisioning.ps1`. Sin `-Execute` solo verifica el paquete; exige el SHA-256 externo de `bundle-manifest.json`, obtenido de la revisión aprobada, no calculado automáticamente como aprobación.

```powershell
& tools/phase7-sql/Invoke-Phase7SqlProvisioning.ps1 `
  -BundleDirectory 'E:\laboratorio\paquete-revisado' `
  -ExpectedManifestSha256 '<SHA256 aprobado del manifiesto>'
& tools/phase7-sql/Test-Phase7SqlProvisioningExecutor.ps1
```

El primer ejemplo es de revisión y no abre conexiones. El segundo ejecuta exclusivamente simulaciones offline. La herramienta no localiza servidores, credenciales ni ensamblados por red; no inicia procesos SQL, SAP o servicios.

La rama futura `-Execute` exige, además, un objeto Approval con exactamente estos campos:

| Campo | Condición |
| --- | --- |
| RunId, ManifestSha256, SpecSha256, PreparationAuthorizationId | Coinciden con el paquete congelado |
| AuthorizationId | Referencia independiente y distinta de la preparación |
| Purpose | `ProvisionIsolatedSqlLaboratory` |
| ExecutionAuthorized | Booleano true, concedido por el responsable fuera de esta preparación |
| ServerName, ProductVersion, AdminPrincipal | Coinciden exactamente con la especificación y la identidad SQL observada |
| NotBeforeUtc, ExpiresAtUtc | UTC, ventana contenida en la del paquete, máximo 24 horas |
| CommandTimeoutSeconds | Entero entre 1 y 120 |
| TotalTimeoutSeconds | Entero entre 1 y 3600 |

`Connection` debe ser una Microsoft.Data.SqlClient.SqlConnection ya abierta en master por un responsable autorizado, cifrada, con certificado validado, `Pooling=false`, `Enlist=false`, `FireInfoMessageEventOnUserErrors=false` y sin transacción ambiental. Se rechaza convertir errores SQL en InfoMessage antes de admitir la sesión y antes de cada comando. Los cambios de base usan un comando USE con nombre cerrado, timeout y cancelación, evitando la implementación síncrona heredada de ChangeDatabaseAsync. El ejecutor no ofrece un método Open ni modifica esas opciones. Verifica con una consulta de lectura servidor, versión, login original y actual, sysadmin, ausencia de transacción e instancia dedicada. Aceptada la sesión, asume su propiedad y la cierra al terminar o fallar.

`Secrets` es un diccionario de cinco SecureString, claves AppA, ObserverA, AppB, ObserverB y Monitor, de 32 a 128 caracteres distintos. El origen seguro y la apertura de la conexión permanecen fuera del ejecutor y requieren autorización posterior. No pasar secretos en línea de comandos, JSON, transcripciones ni archivos. Antes de admitir la sesión se comprueba su diferencia mediante hashes transitorios de buffers que se limpian; solo antes de la etapa 50 se convierten temporalmente a cadenas para parámetros SQL; se libera el BSTR. .NET no garantiza borrado inmediato de todas las copias administradas: no habilitar trazas del proveedor ni volcado de parámetros.

`JournalPath` debe ser una ruta absoluta nueva, con padre ya existente y accesible; no se sobrescribe. Cada lote tiene Started antes del envío y Completed después, con hashes y referencias, sin texto SQL, contraseñas ni mensajes del proveedor. Los bytes se verifican y retienen en memoria antes de cualquier transporte. Un fallo, timeout, cancelación, pérdida de auditoría o limpieza incompleta impide declarar éxito; no se reintenta ni continúa con los lotes restantes. Se limpian las cinco claves secretas y cuatro contextos de aprobación con un timeout independiente, después se cierra la sesión sin pooling. Si el proceso se interrumpe abruptamente, un Started sin Completed obliga a revisión manual; no es evidencia de que el lote no se haya ejecutado.

La secuencia automática acaba en 50. Los inventarios 90/91, el preflight, la suite y el monitor independiente siguen separados. No hay borrado, rollback global, instalación de servicio ni recuperación automática de una instalación parcial. Los diarios simulados llevan Origin=OfflineSimulation y SqlExecuted=false; solo la rama real explícita utiliza SqlServerProvisioning, sin acreditar autenticidad ni cierre global. La aprobación es una entrada de confianza del operador: no es una firma criptográfica ni se valida contra un servicio de autorizaciones.

Validación offline del ejecutor (2026-10-25): 68 controles aprobados, más 99 de regresión del generador. Se verificó sintaxis de los tres archivos PowerShell y la política de errores, cambio de base e identidad asíncrona. La revisión independiente detectó y se corrigieron el desvío de errores a InfoMessage y el cambio de base fuera del presupuesto cancelable. Evidencias locales: `artifacts/backend-solid-phase7-sql-provisioning-executor/summary.json`. No se repitió la compilación .NET porque esta entrega solo modifica herramientas PowerShell y documentación. No validados: conexión real, ejecución SQL, permisos efectivos, esquema instalado o monitor real. No se ejecutó SAP.
