# Plan de migración de NuanSystem a .NET 10 LTS

## Propósito y autorización

Este plan convierte los hallazgos de
[DOTNET-10-MIGRATION-READINESS.md](../architecture/DOTNET-10-MIGRATION-READINESS.md)
en una secuencia ejecutable y reversible.

**Estado:** plan propuesto; no ejecutado.

**Decisión actual:** NO-GO para comenzar hasta contar con aprobación expresa, SDK .NET 10, Visual Studio 2026, DevExpress alineado y las decisiones de build/publicación.

Este documento no autoriza cambios de framework o paquetes, instalaciones, restore, SQL, API, WinForms, workers, SCM, cuentas, certificados, SRI, SAP ni procesamiento documental.

## Reglas no negociables

- Mantener NuanSystem funcional sin SAP.
- Mantener separados API, SRI Worker, SAP Sync y Sync Master/Sucursal.
- No mezclar refactor funcional con el retarget.
- No usar SQL, servicios, certificados o llamadas externas como requisito de compilación.
- `SriWorker:Enabled=false` durante build, publish, instalación y observación inicial.
- No usar QueueId `10004`; no consultar Remigio o Cañaris.
- No copiar secretos, conexiones, claves de acceso ni XML a logs o evidencia.
- Cada commit debe compilar o explicar explícitamente su condición transitoria.
- No reescribir historia ni usar rollback destructivo de datos.

## Decisiones de entrada

| ID | Decisión | Recomendación | Gate |
|---|---|---|---|
| M10-01 | SDK | Último SDK 10.0.x soportado, con política `global.json` aprobada. | Bloqueante |
| M10-02 | IDE | Visual Studio 2026 estable con Managed Desktop y ASP.NET/Web. | Bloqueante WinForms |
| M10-03 | DevExpress | Una sola versión 25.2.x, mínimo 25.2.3; preferir el patch aprobado por el equipo. | Bloqueante WinForms |
| M10-04 | Fuente DevExpress | NuGet/feed reproducible o instalación documentada; no mezcla silenciosa. | Bloqueante |
| M10-05 | Publicación | Elegir framework-dependent o self-contained por host, siempre `win-x64`. | Bloqueante publish |
| M10-06 | Versionado | SemVer de release + commit en `InformationalVersion`. | Bloqueante pilot |
| M10-07 | CI | Agente Windows con SDK/VS/DevExpress/feed y evidencia saneada. | Requerido antes de aceptar |
| M10-08 | SAP HANA | Versión de `Sap.Data.Hana` y declaración oficial para el host. | Bloqueante solo para gate HANA |

## Preparación del toolchain

Ejecutar en un change separado de la modificación del repositorio:

1. Instalar/actualizar Visual Studio 2026 y los workloads aprobados.
2. Instalar el SDK .NET 10 en el último patch soportado.
3. Instalar o configurar el feed de la misma versión DevExpress elegida.
4. Confirmar:

   ```text
   dotnet --info
   dotnet --list-sdks
   dotnet --list-runtimes
   vswhere -all -products * -format json
   ```

5. Comprobar que el CLI y Visual Studio seleccionan el mismo major de SDK.
6. No abrir aún un formulario con el branch modificado hasta capturar el baseline.

Evidencia:

- versiones completas sin usernames ni rutas sensibles innecesarias;
- workloads/componentes instalados;
- versión DevExpress instalada y fuente de paquetes;
- patch ownership y fecha de próxima revisión.

## Baseline reproducible antes del retarget

En un clon/worktree limpio del commit aprobado:

```text
git status --short
git rev-parse HEAD
dotnet --info
dotnet restore NuanSystem.sln
dotnet list NuanSystem.sln package --include-transitive
dotnet build NuanSystem.sln --no-restore
dotnet test NuanSystem.sln --no-build --no-restore
git diff --check
```

El restore necesita autorización de red/feed y debe ejecutarse solo en la fase de implementación. Guardar:

- grafo directo/transitivo;
- warnings NuGet;
- conteo de pruebas;
- hashes de assets o lock si se adopta;
- evidencia de que el tree sigue limpio.

## Orden exacto de migración y commits propuestos

Los mensajes son propuestos; el implementador puede ajustar el texto sin mezclar alcances.

### Commit 1 — fijar toolchain y build reproducible

Propuesto:

```text
build(dotnet): pin .NET 10 toolchain
```

Cambios permitidos:

- `global.json` con SDK 10 aprobado y `rollForward` decidido;
- pipeline Windows o script de build;
- política de feeds/restore sin credenciales;
- opcionalmente centralización de versiones si el equipo la aprueba.

Validaciones:

- `dotnet --version` selecciona 10.x desde raíz y solución;
- restore limpio sin credenciales documentadas;
- `git diff --check`;
- todavía no se cambia ningún TFM.

Rollback: revertir solo este commit; la solución continúa `net9.0`.

### Commit 2 — alinear paquetes de plataforma

Propuesto:

```text
build(deps): align Microsoft packages for .NET 10
```

Orden:

1. `Microsoft.Extensions.*` explícitos a 10.x aprobado.
2. `Microsoft.AspNetCore.*` a 10.x aprobado.
3. `Microsoft.Extensions.Hosting.WindowsServices` y `System.Diagnostics.EventLog` a 10.x.
4. Revisar referencias directas que NuGet 10 marque como podables.
5. Mantener inicialmente Dapper 2.1.72, SqlClient 7.0.1, MediatR 14.1.0, FluentValidation 12.1.1 y Serilog 10.0.0 salvo incompatibilidad demostrada.
6. No agregar Polly.

Validaciones:

- restore sin NU1015, NU1510 no explicado, downgrade o asset incompatible;
- diff del grafo directo/transitivo;
- build/tests aún sobre TFM 9 cuando el paquete lo permita;
- no cambios de código funcional.

Rollback: revertir commit completo; no tocar cache global ni configuración local desde Git.

### Commit 3 — retarget del núcleo y pruebas

Propuesto:

```text
build(dotnet): retarget core projects to net10
```

Orden dentro del commit:

1. `NuanSystem.Domain`
2. `NuanSystem.Shared`
3. `NuanSystem.Application`
4. `NuanSystem.WinForms.Services`
5. `NuanSystem.WinForms.ViewModels`
6. `NuanSystem.Application.Tests`

Validaciones:

```text
dotnet build <cada proyecto> --no-restore
dotnet test tests\NuanSystem.Application.Tests\NuanSystem.Application.Tests.csproj --no-build --no-restore
```

Revisar:

- contratos JSON y DTOs;
- pipelines MediatR/FluentValidation;
- warnings nuevos de C# 14/analyzers;
- 473 pruebas activas como mínimo y las 5 omisiones todavía justificadas.

Rollback: revertir el commit; los proyectos vuelven en conjunto a net9.

### Commit 4 — retarget de infraestructura, persistencia y SAP

Propuesto:

```text
build(dotnet): retarget infrastructure and integrations to net10
```

Orden:

1. `NuanSystem.Infrastructure`
2. `NuanSystem.Persistence`
3. `NuanSystem.SapIntegration`

Validaciones sin servicios externos:

- tests de JWT, protector de secretos y HTTP handlers;
- tests de repositorios/contratos que usan dobles o SQL textual;
- resolución de `Microsoft.Data.SqlClient` 7.0.1 y SNI `win-x64`;
- serialización/cookies/URLs de Service Layer con handlers simulados;
- DI API permanece explícitamente no implementado;
- HANA queda `Blocked` si no se dispone del proveedor aprobado.

No ejecutar conexión SQL, SAP Service Layer, HANA o DI API en este commit.

### Commit 5 — retarget de API

Propuesto:

```text
build(dotnet): retarget api to aspnetcore 10
```

Cambios:

- TFM de `NuanSystem.Api`;
- solo ajustes de compilación/behavior exigidos por ASP.NET Core 10;
- actualización de OpenAPI/Swashbuckle si el contrato lo requiere.

Validaciones:

- build sin warnings;
- tests de endpoints y `Result` -> HTTP;
- 401/403/200, JWT y company context;
- snapshot/diff OpenAPI 3.0/3.1;
- JSON sin conflictos de metadata;
- `SyncProfileExecutionHostedService` deshabilitado y orden de startup probado.

La ejecución local de la API es un gate posterior y debe usar configuración segura, sin SQL real si no está autorizada.

### Commit 6 — retarget de workers

Propuesto:

```text
build(dotnet): retarget workers to net10
```

Orden:

1. `NuanSystem.SyncWorker`
2. `NuanSystem.MasterBranchSyncWorker`
3. `NuanSystem.SriWorker`

Cambios:

- TFM y paquetes Hosting/WindowsServices/EventLog;
- ajustes mínimos demostrados por el nuevo comportamiento de `BackgroundService`;
- ninguna habilitación o nueva acción.

Validaciones automatizadas:

- `Enabled=false` termina o permanece en el estado diseñado;
- el código previo al primer `await` no crea una carrera de startup;
- SRI adquiere una única identidad antes de procesar;
- heartbeat no anuncia estado incorrecto;
- cancellation/drain/shutdown cierran cooperativamente;
- errores de mutex/startup producen estado/evento seguro;
- ningún test inicia red, SQL, SCM o proveedor real.

Punto crítico: desde .NET 10 todo `ExecuteAsync` corre como tarea en background. Si una garantía necesita bloquear startup, moverla deliberadamente a `StartAsync`/lifecycle en un commit de código pequeño y acompañado de tests; no hacerlo por intuición.

### Commit 7 — alinear DevExpress y retarget WinForms

Propuesto:

```text
build(winforms): align DevExpress and target net10 windows
```

Precondiciones:

- Visual Studio 2026 instalado;
- una versión DevExpress 25.2.x aprobada;
- decisión de NuGet vs referencias de instalación;
- baseline de `.Designer.cs`/`.resx`.

Orden:

1. eliminar la mezcla 25.2.5/25.2.6 de forma reproducible;
2. `NuanSystem.WinForms.Controls`;
3. `NuanSystem.WinForms.Forms`;
4. `NuanSystem.WinForms`;
5. decidir `SupportedOSPlatformVersion`;
6. build CLI y Visual Studio.

Validaciones:

- cero warnings WinForms/DevExpress;
- no usar APIs WFDEV004/005/006;
- outputs finales contienen una sola versión por assembly DevExpress;
- 134 Designers y 84 resx permanecen semánticamente intactos salvo cambios aprobados;
- apertura de formularios representativos:
  - `BaseEditForm`
  - un CRUD list derivado de `BaseGridCrudListForm`
  - un editor con `NuanLookupEdit`
  - `SyncMonitorForm`
  - `SriDocumentMonitorForm`
  - `MainForm`
- revisar `InitializeComponent`, disposal, DPI, anchoring/docking, íconos, fuentes, grids y reports;
- cerrar y comparar Git para detectar serialización automática.

Rollback: revertir el commit completo y volver a abrir el mismo conjunto bajo el toolchain anterior.

### Commit 8 — definir publicación y versionado

Propuesto:

```text
build(release): define net10 win-x64 artifacts
```

Debe definir por host:

- `win-x64`;
- framework-dependent o self-contained, nunca implícito;
- configuración Release;
- no trimming/single-file en pilot1;
- rutas de configuración externas;
- exclusión de `appsettings.Local.json`;
- versionado `AssemblyVersion`, `FileVersion`, `InformationalVersion`;
- manifest, hash y SBOM/inventario de dependencias;
- comandos de publish por proyecto, no output único de solución.

Proyectos publicables:

- `NuanSystem.Api`
- `NuanSystem.SyncWorker`
- `NuanSystem.MasterBranchSyncWorker`
- `NuanSystem.SriWorker`
- `NuanSystem.WinForms`

Validaciones:

- inspección negativa de secretos, XML, certificados y configuraciones locales;
- dependencia nativa correcta de SqlClient/DevExpress;
- `appsettings.Production.json` incluido solo donde corresponde;
- configuración local no incluida;
- ejecución deshabilitada en entorno aislado cuando se autorice.

### Commit 9 — actualizar documentación operativa

Propuesto:

```text
docs(dotnet): document net10 deployment and rollback
```

Actualizar referencias `bin\Debug\net9.0` solo después de que existan comandos reales de publish. No convertir plantillas en instaladores automáticos ni ejecutar SCM.

## Matriz de validación por etapa

| Gate | Core | Infra/Persistence | API | Workers | WinForms | Publish |
|---|---:|---:|---:|---:|---:|---:|
| Restore limpio | Sí | Sí | Sí | Sí | Sí | Sí |
| Build 0/0 | Sí | Sí | Sí | Sí | Sí | Sí |
| Unit tests | Sí | Sí | Sí | Sí | Sí | Sí |
| Contract/API tests | N/A | Parcial | Sí | Health | Cliente | N/A |
| Native assets `win-x64` | N/A | SqlClient | SqlClient | SqlClient/EventLog | DevExpress | Sí |
| Lifecycle/cancel | N/A | N/A | Hosted service | Sí | UI close | Sí |
| Designer | N/A | N/A | N/A | N/A | Sí | N/A |
| Sensitive scan | Sí | Sí | Sí | Sí | Sí | Sí |
| External runtime | No | Bajo autorización | Bajo autorización | Deshabilitado primero | API simulada primero | Host aislado |

## Compatibilidad específica que debe probarse

### API y Minimal API

- startup y middleware order;
- auth JWT, permission claims y security stamp;
- `X-Company-Code` y aislamiento;
- `Result<T>`/`ApiResponse<T>`;
- OpenAPI/Swagger y nullability;
- validación FluentValidation sin doble respuesta por la validación nueva de Minimal APIs;
- JSON y `HttpRequestJsonExtensions`.

### Persistence y SQL Client

- carga de `Microsoft.Data.SqlClient.dll` y SNI x64;
- TLS/hostname/trust, sin `TrustServerCertificate=true`;
- Integrated Security/gMSA en un gate autorizado;
- Dapper mappings, stored procedures y cancellation;
- assembly version informativa registrada sin paths/secrets.

### SAP

- Service Layer con `HttpMessageHandler` simulado primero;
- cookies, logout, OData/JSON, timeout y clasificación de errores;
- ningún bypass SSL en producción;
- HANA solo con driver declarado compatible por SAP;
- DI API permanece fuera de aceptación mientras sea stub/no haya SDK COM compatible.

### SOAP/SRI

- envelope exacto, `SOAPAction`, content type y encoding;
- parser con DTD prohibido, resolver nulo y límites;
- checksum/tamaño/duplicados con fixtures sintéticos;
- ningún endpoint SRI real durante el retarget;
- una llamada futura requiere autorización independiente y no se infiere del build.

### Windows Services

- `UseWindowsService` detecta SCM;
- startup, stop, recovery y Event Log;
- identidad sin login interactivo y ACL mínimas;
- heartbeat exacto y ausencia de claims cuando está Disabled;
- comportamiento de servicios simultáneos después del cambio `BackgroundService`;
- cero proceso residual.

### WinForms y Designer

- carga de todos los assemblies DevExpress de una misma versión;
- Designer VS 2026 y runtime;
- controles corporativos, grids, lookups, Ribbon, reports/export;
- DPI/font/autoscale, min size, docking/anchoring;
- manejo global de excepciones;
- clientes tipados sobre `INuanApiClient`, sin acceso externo directo.

## Estrategia pilot1 / pilot2

La publicación no equivale a activación.

### Nombres de versión propuestos

- `7.1.0-dotnet10-pilot1+<commit-corto>`
- `7.1.0-dotnet10-pilot2+<commit-corto>`

`AssemblyVersion` puede permanecer estable durante la serie si se aprueba; `FileVersion` debe distinguir el build y `InformationalVersion` debe conservar SemVer + commit. `WorkerVersionResolver` debe devolver exactamente el valor informativo.

### Pilot1 — artefacto y observación deshabilitada

1. Publicar Release `win-x64` con modalidad aprobada.
2. Crear manifest/hash/SBOM.
3. Escanear el artefacto.
4. Instalar solo mediante change futuro autorizado, con startup Disabled.
5. Iniciar manualmente con `SriWorker:Enabled=false`.
6. Verificar:
   - `WorkerVersion = 7.1.0-dotnet10-pilot1+...`;
   - heartbeat `Disabled`;
   - cero claims, intentos, auditoría documental y llamadas SRI;
   - Event Log/log file saneados;
   - stop cooperativo y cero procesos.
7. Mantener observación de 24 horas según Iteración 7.

### Pilot2 — cambio binario controlado

1. Publicar desde un commit posterior aceptado.
2. Verificar versión `pilot2`, hash y manifest distintos.
3. Detener pilot1 cooperativamente.
4. Cambiar release activa sin tocar datos/configuración secreta.
5. Iniciar deshabilitado y verificar versión exacta.
6. Repetir health/lifecycle/alertas synthetic.
7. No habilitar procesamiento como parte de pilot2.

### Prueba de rollback pilot2 -> pilot1

1. `Enabled=false`.
2. Stop cooperativo y cero procesos.
3. Restaurar puntero/binPath a la release pilot1 inmutable.
4. Iniciar deshabilitado.
5. Confirmar `WorkerVersion` pilot1, heartbeat y TLS/schema.
6. Preservar cola, leases, intentos, XML, auditoría y logs.
7. No usar restore SQL ni editar estados como rollback rutinario.

## Comprobación de WorkerVersion

Gates:

1. Inspectar el assembly publicado: InformationalVersion exacta.
2. Ejecutar tests de `WorkerVersionResolver` para:
   - preferencia de InformationalVersion;
   - distinción pilot1/pilot2;
   - fallback seguro.
3. En ejecución deshabilitada autorizada, comparar:
   - manifest;
   - assembly;
   - heartbeat Master;
   - API health;
   - texto del monitor WinForms.
4. Fallar si se reporta `1.0.0`, TFM, path, versión anterior o valor truncado.

## Apertura del Visual Studio Designer

Procedimiento:

1. Capturar `git status --short`.
2. Abrir solución en Visual Studio 2026 con DevExpress alineado.
3. Abrir, cerrar y volver a abrir cada formulario representativo.
4. Verificar bandeja de componentes, custom controls, images, resources y herencia.
5. Guardar solo si existe un cambio intencional aprobado.
6. Cerrar Visual Studio.
7. Comparar:

   ```text
   git diff -- '*.Designer.cs' '*.resx' '*.csproj'
   git diff --check
   ```

8. Si el Designer reserializa sin intención, NO-GO; restaurar mediante revert del commit/archivo exacto, no `reset --hard`.

## Pruebas de API, WinForms y workers

### Automatizadas obligatorias

```text
dotnet build NuanSystem.sln --no-restore
dotnet test NuanSystem.sln --no-build --no-restore
```

Además:

- contratos API/auth/tenant;
- DTO/JSON frontend;
- Worker operations/health/version;
- SRI provider con HTTP falso;
- SAP Service Layer con HTTP falso;
- SQL contract tests que no conecten;
- exports/formatters que puedan probarse sin UI.

### Manuales controladas

- API smoke con dependencias falsas o ambiente autorizado;
- WinForms login/shell/monitores con API de prueba;
- apertura de Designer;
- worker deshabilitado como consola y, después, SCM bajo change;
- publish FDD/SCD en host objetivo.

Las pruebas que requieren SQL, SAP, SRI, servicios o certificados se mantienen bloqueadas hasta una autorización que nombre ambiente, identidad, datos y rollback.

## Escaneo sensible

Aplicar al diff, documentación y artefacto:

- claves privadas/certificados: `.pfx`, `.p12`, `.pem`, `BEGIN PRIVATE KEY`;
- JWT/tokens/API keys;
- passwords y connection strings con valores;
- XML SRI o claves de acceso completas;
- `appsettings.Local.json`, secrets de usuario y `.env`;
- nombres/rutas de tenants fuera del alcance;
- logs con payloads.

Permitir únicamente nombres de claves/configuración sin valores y URLs oficiales públicas ya versionadas.

## Estrategia de rollback por tipo de fallo

| Falla | Acción |
|---|---|
| SDK/restore | Revertir commit de toolchain/paquetes; conservar logs saneados. |
| Core/API compile | Revertir el commit de esa capa; no parchear todas las capas a la vez. |
| BackgroundService/lifecycle | Revertir commit workers; investigar orden con test mínimo. |
| DevExpress/Designer | Revertir commit WinForms completo; restablecer versión única anterior. |
| Publish/native asset | Descartar artefacto; no cambiar servicio/host. |
| Pilot2 | Volver a release pilot1 inmutable con worker Disabled. |
| Datos externos | Detener; preservar evidencia; seguir runbook específico. No usar rollback binario como reparación de datos. |

## Evidencia necesaria para aprobar

Por commit:

```text
Commit:
Scope:
SDK/IDE/DevExpress:
Restore result:
Build result:
Test result:
Warnings:
Package graph delta:
Sensitive scan:
Manual validation:
Blocked validation:
Rollback verified:
Reviewer:
```

Para cierre:

- HEAD y lista de commits;
- inventario final de 16 proyectos/TFM;
- SDK/runtime/VS/DevExpress exactos;
- paquete directo/transitivo final;
- build 0 errores/0 warnings;
- tests, omitidos y duración;
- resultados API/workers/WinForms/Designer;
- publish commands, RID/modalidad, hashes y manifest;
- WorkerVersion pilot1/pilot2/rollback;
- evidencia negativa de secretos/XML/config local;
- declaración de que no hubo cambios funcionales ni SQL;
- aceptación de Desarrollo, QA, Operaciones y propietario; Seguridad/DBA cuando aplique.

## Gates finales

| Gate | Estado al 2026-07-25 |
|---|---|
| Readiness documental | Validated |
| SDK .NET 10 | Validated: SDK 10.0.302 y runtimes 10.0.10 |
| Visual Studio 2026 | Blocked |
| DevExpress instalado, build CLI | Validated: DevExpress Components 25.2 |
| Visual Studio Designer | Blocked por Visual Studio 2022 |
| Restore/build/test net10 | Validated: 0 warnings, 0 errors; 473 passed, 5 skipped |
| API net10 | Validated para build y publish; runtime no ejecutado |
| Workers net10 | Validated para build y publish; runtime no ejecutado |
| WinForms net10 | Validated para build y publish; smoke visual pendiente |
| Publish `win-x64` | Validated, framework-dependent |
| Pilot1/pilot2/rollback | Not validated |
| SQL/SAP/SRI runtime | Not applicable a esta fase |

## Criterio de cierre

La migración solo se acepta cuando:

- todos los proyectos versionados están homogéneamente en .NET 10;
- no quedan referencias 9.x de plataforma sin justificación;
- build/tests y gates manuales pasan;
- Designer y DevExpress están validados;
- workers conservan lifecycle y versión;
- publicaciones son reproducibles y reversibles;
- no se amplió comportamiento, datos o integraciones;
- el propietario aprueba la promoción.

La ejecución técnica realizada se documenta en
[DOTNET-10-MIGRATION-EXECUTION.md](../architecture/DOTNET-10-MIGRATION-EXECUTION.md).
La migración no concede autorización para instalar servicios, ejecutar SQL, habilitar workers,
conectarse a SAP/SRI ni promover artefactos a producción.
