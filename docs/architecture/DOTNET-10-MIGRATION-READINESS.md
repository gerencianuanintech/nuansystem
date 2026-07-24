# Readiness de migración de NuanSystem a .NET 10 LTS

## Estado y decisión

- **Fecha de la auditoría:** 2026-07-24.
- **Baseline Git:** `4df7ce630edb8bcafb92a82bac5737d4be669915`.
- **Rama de auditoría:** `refactor/codex-skills-v7-1-dotnet10-readiness`.
- **Tipo de trabajo:** Discovery, análisis y documentación; no es una implementación.
- **Decisión:** **NO-GO para ejecutar la migración en el entorno actual**.
- **Motivo:** no están instalados el SDK/runtime .NET 10 ni Visual Studio 2026; tampoco existe todavía una prueba `net10.0`, un contrato reproducible de SDK/publicación o una validación del Designer con una instalación DevExpress alineada.
- **Viabilidad arquitectónica:** **GO condicionado**. No se encontró una incompatibilidad estructural que obligue a rediseñar NuanSystem. Los paquetes críticos tienen una ruta oficial compatible, pero los gates de este documento deben cerrarse antes de cambiar un `TargetFramework`.

Esta decisión complementa el gate D7-01 de
[SRI-ITERATION-7-PRODUCTION-READINESS-BLUEPRINT.md](SRI-ITERATION-7-PRODUCTION-READINESS-BLUEPRINT.md)
y no habilita el SRI Worker, un tenant, un servicio Windows ni una llamada externa.

## Escala de evidencia

| Estado | Significado |
|---|---|
| Confirmado oficialmente | Microsoft o el proveedor declara soporte para .NET 10 o Visual Studio requerido. |
| Confirmado por el repositorio | El código, los assets restaurados o una validación local del estado actual sustentan la conclusión. |
| Requiere prueba | La compatibilidad es probable o declarada, pero NuanSystem debe compilarse o ejercitarse en el entorno objetivo. |
| Bloqueado | Falta una herramienta, decisión o evidencia imprescindible. |
| No aplicable | El componente no existe o no participa en la solución. |

## Discovery Record

**Outcome:** determinar si la solución puede migrar de .NET 9 a .NET 10 LTS, identificar bloqueos y definir la secuencia de validación sin modificar código, frameworks, paquetes, SQL ni configuración.

**Work type:** evolución transversal de framework con impacto en API, workers, persistencia, integraciones, WinForms, Designer, pruebas y despliegue.

**Domain:** plataforma técnica compartida. No cambia propiedad de datos, reglas comerciales, tenancy, SAP, SRI ni sincronización.

**Explicit domain decisions and exclusions:**

- NuanSystem continúa siendo ERP independiente; SAP es opcional.
- SRI, SAP y Sync Master/Sucursal conservan workers y contratos separados.
- No se modifican `TargetFramework`, paquetes, producto, SQL, servicios, certificados ni configuraciones.
- No se ejecutan API, WinForms, workers, SRI, SAP o bases de datos.
- No se procesa QueueId `10004` ni se consulta Remigio o Cañaris.
- Esta auditoría no instala SDK, workloads, Visual Studio ni DevExpress.

**Affected layers:**

| Capa | Estado en esta fase | Impacto futuro esperado |
|---|---|---|
| Domain | Verificada sin cambios | Retarget y pruebas puras. |
| Application | Verificada sin cambios | Retarget, alineación de `Microsoft.Extensions`, MediatR y FluentValidation. |
| Persistence | Verificada sin cambios | Retarget y pruebas de `Microsoft.Data.SqlClient`/Dapper por RID. |
| API | Verificada sin cambios | ASP.NET Core 10, Minimal API, JWT y OpenAPI. |
| Workers | Verificada sin cambios | Hosting 10, Windows Services y cambio de comportamiento de `BackgroundService`. |
| SapIntegration | Verificada sin cambios | Service Layer HTTP; HANA y DI API requieren gates separados del proveedor. |
| SRI | Verificada sin cambios | SOAP sobre `HttpClient`, XML seguro y lifecycle del servicio. |
| WinForms | Verificada sin cambios | `net10.0-windows`, DevExpress 25.2 y Designer de Visual Studio 2026. |
| Tests | Verificada sin cambios | Retarget del proyecto y ejecución completa bajo .NET 10. |
| Operations | Documentación | SDK, publicación, versionado, pilot1/pilot2 y rollback. |

**Risk:** bajo para estos dos documentos; alto para la futura migración por el alcance transversal, el Designer, dependencias nativas, servicios Windows e integraciones.

**Evidence inspected:**

- Los 16 proyectos versionados de `NuanSystem.sln`, `Directory.Build.props` y el único `.sln`.
- Ausencia de `.targets`, `global.json`, `Directory.Packages.props`, lock files y perfiles `.pubxml` versionados.
- Un `.csproj` auxiliar ignorado en `artifacts/runtime-audit`, fuera de la solución.
- `dotnet --info`, `--list-sdks`, `--list-runtimes` y Visual Studio Installer mediante `vswhere`.
- Paquetes directos y transitivos mediante `dotnet list NuanSystem.sln package --include-transitive` y `project.assets.json`.
- Código de API, workers, WinForms, SAP Service Layer, SOAP/SRI, versionado y scripts operativos.
- Fuentes oficiales enlazadas en este documento.

**Selected pattern:** migración transversal escalonada dentro de una sola rama de implementación, con commits reversibles, baseline reproducible, capas puras primero, hosts después y WinForms/Designer al final.

**Permitted reuse boundary:** conservar contratos y comportamiento; cambiar únicamente framework/toolchain y las versiones de paquete que el gate de implementación apruebe. No aprovechar la migración para refactor funcional.

**Components to reuse:** `NuanSystem.sln`, arquitectura por capas, pruebas existentes, Generic Host, `UseWindowsService`, health/heartbeat, `WorkerVersionResolver`, transporte HTTP centralizado, controles/formularios corporativos y runbooks SRI existentes.

**Alternatives rejected:**

- Retarget masivo sin baseline ni commits por etapa: dificulta atribuir fallos y rollback.
- Migrar solo el SRI Worker: rompe la coherencia de referencias a proyectos compartidos y duplica toolchains.
- Mantener producción en .NET 9 sin fecha de salida: .NET 9 finaliza soporte el 2026-11-10.
- Asumir soporte DevExpress por compilación actual: el Designer y el IDE tienen requisitos propios.
- Agregar `global.json` o cambiar paquetes en esta fase documental: está expresamente prohibido.

**Gaps/new code:** ninguno autorizado ahora. La futura implementación requerirá cambios declarativos de TFM/paquetes, posiblemente un `global.json`, perfiles o scripts de publicación y solo los ajustes de código demostrados por compilación/pruebas.

**Differences/constraints:** el equipo actual solo puede validar .NET 9; no existe CI versionado; la solución mezcla paquetes `Microsoft.Extensions` 9/10; DevExpress se obtiene a la vez desde rutas absolutas y NuGet.

**Confidence:** alta sobre inventario y baseline actual; media sobre compatibilidad runtime hasta ejecutar .NET 10, Designer, publicaciones y pruebas externas autorizadas.

**Validation required:** todos los gates de [DOTNET-10-MIGRATION-PLAN.md](../operations/DOTNET-10-MIGRATION-PLAN.md).

## Política de soporte y urgencia

La [política oficial de soporte de .NET](https://dotnet.microsoft.com/en-us/platform/support/policy)
vigente al 2026-07-24 registra:

| Versión | Tipo/fase | Último patch publicado | Fin de soporte | Evaluación |
|---|---|---:|---:|---|
| .NET 9 | STS / Maintenance | 9.0.18 | 2026-11-10 | Horizonte insuficiente; el equipo local está en runtime 9.0.5. |
| .NET 10 | LTS / Active | 10.0.10 | 2028-11-14 | Objetivo recomendado, siempre con el último patch soportado. |

Microsoft exige mantenerse en el patch vigente para recibir soporte. Por ello, `9.0.5` no es un baseline productivo soportado al día de la auditoría aunque compile correctamente.

## Inventario del entorno de desarrollo

### .NET detectado

| Elemento | Detectado | Estado para la migración |
|---|---|---|
| SDK activo | 9.0.300; MSBuild 17.14.5 | Confirmado por el entorno actual. |
| SDK adicional | 5.0.416 | No aplicable a NuanSystem. |
| Host/runtime activo | 9.0.5, `win-x64` | Desactualizado respecto de 9.0.18. |
| ASP.NET Core | 3.1.32, 5.0.17, 8.0.16, 8.0.23, 9.0.5 | No existe runtime 10. |
| .NET Core | 3.1.32, 5.0.17, 8.0.16, 8.0.23, 9.0.5 | No existe runtime 10. |
| Windows Desktop | 3.1.32, 5.0.17, 8.0.16, 9.0.5 | No existe Desktop Runtime 10. |
| Workloads | Ninguno | No se requiere workload para la solución actual; debe revalidarse con el SDK 10. |
| `global.json` | No existe | El SDK queda seleccionado por la instalación de cada equipo. Bloquea reproducibilidad. |

La política futura de SDK debe decidirse en implementación. Microsoft documenta en
[`global.json`](https://learn.microsoft.com/en-us/dotnet/core/tools/global-json)
que `10.0.100` con `rollForward: latestFeature` conserva el major 10 y permite bandas/patches posteriores. El valor exacto debe corresponder al SDK aprobado entonces, no al ejemplo de esta auditoría.

### Visual Studio y DevExpress instalados

| Producto | Versión | Evaluación |
|---|---:|---|
| Visual Studio Enterprise 2022 | 17.14.0 / 17.14.36109.1 | Tiene workloads Managed Desktop, ASP.NET/Web y .NET SDK. No es un IDE soportado para apuntar a .NET 10. |
| Visual Studio Enterprise 2019 | 16.11.47 | No aplicable a .NET 9/10 de esta solución. |
| DevExpress Components | 25.2.5.26075 | Rama compatible oficialmente con .NET 10, pero no coincide con el paquete 25.2.6 del proyecto Forms. |

La [matriz SDK/MSBuild/Visual Studio de Microsoft](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs)
indica que apuntar a un runtime nuevo desde un Visual Studio anterior no está soportado y produce advertencia; .NET 10 se alinea con Visual Studio 2026 (18.x). El CLI puede coexistir con Visual Studio 2022, pero eso no valida el Designer.

## Inventario de solución y proyectos

`NuanSystem.sln` contiene 16 proyectos versionados: 15 de producto y 1 de pruebas.

| # | Proyecto | Tipo | TFM actual | Acción futura |
|---:|---|---|---|---|
| 1 | `NuanSystem.Domain` | Biblioteca | `net9.0` | Retarget primero; cero dependencias NuGet directas. |
| 2 | `NuanSystem.Shared` | Biblioteca | `net9.0` | Retarget primero; validar contratos serializados. |
| 3 | `NuanSystem.Application` | Biblioteca | `net9.0` | Retarget después de Domain/Shared; alinear Extensions. |
| 4 | `NuanSystem.Infrastructure` | Biblioteca | `net9.0` | Retarget; validar JWT, HTTP y criptografía. |
| 5 | `NuanSystem.Persistence` | Biblioteca | `net9.0` | Retarget; validar SqlClient, Dapper y assets nativos `win-x64`. |
| 6 | `NuanSystem.SapIntegration` | Biblioteca | `net9.0` | Retarget; validar Service Layer; HANA/DI API quedan gates externos. |
| 7 | `NuanSystem.Api` | Web/Minimal API | `net9.0` | Retarget a ASP.NET Core 10 y validar auth/OpenAPI/endpoints. |
| 8 | `NuanSystem.SyncWorker` | Worker | `net9.0` | Retarget y probar lifecycle/heartbeat/Windows Service. |
| 9 | `NuanSystem.MasterBranchSyncWorker` | Worker | `net9.0` | Retarget y probar modos seguros/lifecycle. |
| 10 | `NuanSystem.SriWorker` | Worker | `net9.0` | Retarget último entre workers; mantener `Enabled=false`. |
| 11 | `NuanSystem.WinForms.Services` | Biblioteca | `net9.0` | Retarget antes de UI; validar HTTP/JSON. |
| 12 | `NuanSystem.WinForms.ViewModels` | Biblioteca | `net9.0` | Retarget después de Services/Shared. |
| 13 | `NuanSystem.WinForms.Controls` | WinForms | `net9.0-windows` | Alinear DevExpress y validar Designer primero. |
| 14 | `NuanSystem.WinForms.Forms` | WinForms | `net9.0-windows` | Retarget tras Controls; abrir formularios representativos. |
| 15 | `NuanSystem.WinForms` | WinExe | `net9.0-windows` | Retarget al final; smoke visual sin integraciones externas. |
| 16 | `NuanSystem.Application.Tests` | xUnit | `net9.0` | Retarget junto al primer commit compilable; ejecutar después de cada etapa. |

El SDK resuelve `net9.0-windows` como `net9.0-windows7.0` en los assets. No hay `TargetPlatformMinVersion` ni `SupportedOSPlatformVersion` explícitos. En la implementación debe decidirse el mínimo real soportado sin confundir el sufijo del TFM con la
[matriz de sistemas operativos soportados por .NET 10](https://github.com/dotnet/core/blob/main/release-notes/10.0/supported-os.md).

Existe además `artifacts/runtime-audit/RuntimeAudit.csproj`, ignorado por Git, fuera de `NuanSystem.sln`, con `net9.0` y `Microsoft.Data.SqlClient 7.0.0`. Se inspeccionó para completar el discovery, pero no es fuente versionada ni parte de la migración. No se modificó ni eliminó.

## Archivos de build, restore y pipeline

| Evidencia | Resultado | Consecuencia |
|---|---|---|
| Soluciones | Solo `NuanSystem.sln`. | Una raíz de compilación clara. |
| Props | Solo `Directory.Build.props`, que excluye `artifacts`. | No centraliza TFM, versiones, analyzers ni warnings. |
| Targets | Ninguno versionado. | Sin lógica MSBuild adicional conocida. |
| `global.json` | Ausente. | Equipos/CI pueden usar SDK majors distintos. |
| `Directory.Packages.props` | Ausente. | Versiones directas distribuidas en proyectos. |
| `packages.lock.json` | Ausente. | El grafo no está bloqueado. |
| `.pubxml` | Ninguno. | No hay modalidad de publicación versionada. |
| GitHub Actions/pipelines | Ninguno versionado. | No existe gate remoto reproducible para .NET 9 o 10. |
| Warnings/analyzers | No hay `TreatWarningsAsErrors`, `AnalysisLevel` o `LangVersion` comunes. | La migración debe capturar warnings nuevos sin silenciarlos globalmente. |

## Paquetes directos y transitivos

El inventario restaurado contiene 150 combinaciones paquete/versión únicas en los `project.assets.json`. La tabla siguiente concentra los paquetes directos y transitivos que gobiernan la migración.

| Componente actual | Evidencia .NET 10 | Estado | Acción |
|---|---|---|---|
| ASP.NET Core / `Microsoft.AspNetCore.Authentication.JwtBearer 9.0.13` | ASP.NET Core forma parte del soporte de .NET 10; la documentación de JWT existe para 10. | Confirmado oficialmente; versión 9 requiere alineación. | Mover paquetes `Microsoft.AspNetCore.*` a 10.x aprobado y probar 401/403/200. |
| Minimal APIs | La [documentación de ASP.NET Core 10](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0) mantiene Minimal APIs e introduce cambios de validación/OpenAPI. | Confirmado oficialmente; requiere prueba. | Mantener el pipeline FluentValidation actual salvo decisión explícita; revisar contratos OpenAPI. |
| `Microsoft.AspNetCore.OpenApi 9.0.5` | OpenAPI 3.1 en .NET 10 cambia modelos y nullability. | Requiere prueba. | Alinear a 10.x y comparar documento generado/consumidores. |
| `Swashbuckle.AspNetCore 10.1.7` | El [paquete 10.1.7](https://www.nuget.org/packages/Swashbuckle.AspNetCore/10.1.7) declara grupo `net10.0`. | Confirmado oficialmente. | Mantener 10.x o actualizar solo con decisión separada; validar generación y UI. |
| `Dapper 2.1.72` | Assets compatibles con .NET moderno; el repositorio usa APIs ADO.NET estándar. | Confirmado por paquete/repositorio; requiere prueba SQL autorizada futura. | Retarget sin cambio funcional y ejecutar contratos de mapeo. |
| `Microsoft.Data.SqlClient 7.0.1` | Microsoft documenta que 7.0 compila/prueba en .NET 10 y soporta .NET 8+ en [SqlClient 7.0](https://learn.microsoft.com/en-us/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace?view=sql-server-ver17). | Confirmado oficialmente. | Conservar 7.0.x aprobado; probar SNI, TLS, Integrated Security y publish `win-x64`. |
| `MediatR 14.1.0` | La [release oficial 14.1.0](https://github.com/LuckyPennySoftware/MediatR/releases) incluye trabajo de .NET 10; el paquete apunta a .NET 8+ y .NET Standard. | Confirmado oficialmente. | Retarget y ejecutar handlers/pipeline tests. |
| `FluentValidation 12.1.1` | La [documentación oficial](https://docs.fluentvalidation.net/en/latest/) declara .NET 8 y posteriores, incluido .NET 10. | Confirmado oficialmente. | Retarget; conservar precedencia del pipeline existente frente a validación Minimal API nueva. |
| `Serilog.AspNetCore/Extensions.Hosting/Settings.Configuration 10.0.0` | [Serilog.AspNetCore 10.0.0](https://www.nuget.org/packages/Serilog.AspNetCore/10.0.0) incluye `net10.0`. | Confirmado oficialmente. | Mantener major 10; validar sinks Console/File/Event Log y redacción. |
| `Serilog.Sinks.Console 6.1.1`, `File 7.0.0` | Assets .NET Standard/.NET moderno. | Confirmado por paquete; requiere smoke. | Probar rotación, paths y shutdown; no cambiar formato sin necesidad. |
| Polly | No existe referencia directa o transitiva ni uso de sus APIs. | No aplicable. | No agregarlo por la migración; NuanSystem conserva `ISapSyncRetryPolicy`. |
| `Microsoft.Extensions.*` 9.0.13 y 10.0.0/10.0.7 | .NET 10 soporta la familia 10.x. El grafo actual mezcla majors. | Compatible, pero desalineado. | Normalizar paquetes explícitos a 10.x aprobado y revisar referencias directas redundantes. |
| `System.Diagnostics.EventLog 9.0.13` | API disponible en .NET 10/Windows. | Confirmado oficialmente; versión requiere alineación. | Alinear a 10.x y validar source/eventos bajo identidad del servicio. |
| `System.ServiceProcess.ServiceController 9.0.13` | Transitivo de WindowsServices; la [API ServiceController](https://learn.microsoft.com/en-us/dotnet/api/system.serviceprocess.servicecontroller) continúa disponible en .NET moderno. | Confirmado oficialmente; requiere prueba SCM. | Alinear mediante Hosting.WindowsServices 10.x; no agregar control remoto desde UI. |
| `Microsoft.Extensions.Hosting.WindowsServices 9.0.13` | Generic Host/Windows Service continúa soportado. | Confirmado oficialmente; requiere prueba runtime. | Alinear a 10.x y ejecutar lifecycle/stop/recovery en piloto autorizado. |
| `System.IdentityModel.Tokens.Jwt 8.17.0` | Assets compatibles con .NET moderno; no es parte del shared framework. | Requiere prueba. | Mantener o actualizar por decisión de seguridad separada; probar emisión/validación y security stamp. |
| `ClosedXML 0.104.2` | El [paquete oficial](https://www.nuget.org/packages/ClosedXML/0.104.2) ofrece assets compatibles; no declara validación específica de NuanSystem en .NET 10. | Requiere prueba. | Probar exportaciones Excel y dependencias OpenXML/System.Drawing/OleDb. |
| Tooling xUnit 2.9.2 / runner 2.8.2 / Test SDK 17.12.0 / coverlet 6.0.2 | Sus TFMs son consumibles por .NET 10; `Microsoft.NET.Test.Sdk 17.12.0` antecede a .NET 10. | Requiere prueba y decisión de actualización. | Ejecutar primero con versiones actuales; actualizar tooling en commit separado solo si el gate lo exige. |

### Riesgo de grafo transitive mixto

El grafo actual incorpora assemblies `Microsoft.Extensions` 9.x y 10.x, `System.*` 8.x/9.x/10.x y varias versiones 8.14/8.16/8.17 de IdentityModel por caminos distintos. El build actual resuelve el conjunto sin warnings, pero esa evidencia no sustituye un restore limpio con SDK 10. La implementación debe:

1. restaurar desde caches/feeds aprobados con SDK 10;
2. capturar `dotnet list package --include-transitive`;
3. investigar `NU1510`, `NU1015`, downgrades y assets sin runtime;
4. no actualizar transitivos arbitrariamente fuera de sus paquetes raíz;
5. comparar el grafo antes/después y guardar una evidencia saneada.

## DevExpress WinForms y Visual Studio Designer

### Estado confirmado

- `NuanSystem.WinForms.Forms` referencia `DevExpress.Win 25.2.6`.
- `NuanSystem.WinForms`, `Controls` y `Forms` también contienen `Reference` con `HintPath` absoluto a `C:\Program Files\DevExpress 25.2\...`.
- El producto instalado es 25.2.5.26075.
- Los outputs actuales demuestran una mezcla:
  - `Controls`: assemblies 25.2.5.26075 desde la instalación local.
  - `Forms` y ejecutable final: assemblies 25.2.6 desde NuGet.
- Hay 134 `.Designer.cs` y 84 `.resx` bajo frontend.

DevExpress declara oficialmente en
[DevExpress v25.2 y .NET 10/Visual Studio 2026](https://supportcenter.devexpress.com/ticket/details/t1306812/devexpress-v25-2-and-visual-studio-2026-net-10-compatibility)
que WinForms v25.2.3+ soporta .NET 10 y Visual Studio 2026. La
[página de novedades v25.2](https://www.devexpress.com/subscriptions/new-2025-2.xml)
también declara soporte para ambos.

### Conclusión DevExpress

| Pregunta | Respuesta |
|---|---|
| ¿La rama 25.2 usada tiene soporte oficial para .NET 10? | Sí; 25.2.5 y 25.2.6 superan el mínimo 25.2.3. |
| ¿La solución está lista para Designer .NET 10 hoy? | No; falta Visual Studio 2026 y hay versiones/fuentes de assembly desalineadas. |
| ¿Compilar con CLI prueba el Designer? | No. El Designer ejecuta componentes en un proceso/toolchain distinto. |
| ¿Puede conservarse 25.2.6? | Probablemente sí, pero debe fijarse una sola fuente/patch y probarse. |

Antes del retarget de WinForms se debe escoger una única versión DevExpress 25.2.x aprobada, alinear instalación/paquetes/referencias y eliminar la dependencia de rutas absolutas o documentarla como prerrequisito reproducible. La decisión concreta pertenece a implementación.

## APIs y comportamientos sensibles a .NET 10

La lista oficial de [breaking changes de .NET 10](https://learn.microsoft.com/en-us/dotnet/core/compatibility/10)
se contrastó con el repositorio.

| Área | Evidencia NuanSystem | Evaluación |
|---|---|---|
| `BackgroundService.ExecuteAsync` | API, SyncWorker, MasterBranchSyncWorker y SriWorker contienen hosted/background services. `SriBackgroundWorker` adquiere el mutex antes del primer `await`. | **Riesgo alto:** en .NET 10 todo `ExecuteAsync` corre en background y ya no bloquea startup. Probar orden, mutex, heartbeat, estado Disabled y errores de inicio. |
| WinForms obsoletions | No se encontraron `OnClosing`, `OnClosed`, `Clipboard.GetData`, `ContextMenu`, `MainMenu`, `StatusBar`, `ToolBar` o `DataGrid` de WinForms. Las coincidencias `StatusBar` son propiedades DevExpress Ribbon. | Sin impacto de código confirmado; compilar con analyzers .NET 10 y abrir Designer. |
| WinForms visual behavior | Hay TreeView/grids/status surfaces y uso amplio de DevExpress/System.Drawing. | Requiere regresión visual por cambios de render, DPI, exceptions de Drawing y controles de terceros. |
| System.Text.Json | No existen converters personalizados con `Utf8JsonReader` ni nombres `$type/$id/$ref` explícitos. | Riesgo bajo; ejecutar tests de contratos API/frontend. |
| `XmlSerializer` + `[Obsolete]` | El SRI Worker usa `XDocument`/`XmlReader`; no se encontraron miembros `[Obsolete]` propios ni uso de `XmlSerializer`. | Cambio oficial no aplicable al flujo SRI actual. |
| SOAP/SRI | SOAP 1.1 manual por `HttpClient`, `SOAPAction`, `text/xml`, `XDocument`; DTD prohibido, resolver nulo y límite de caracteres. | Compatible por API; requiere pruebas unitarias y, después, prueba externa separadamente autorizada. |
| SAP Service Layer | Transporte `HttpClient`/JSON y factory; no hay referencia COM compilada. | Compatible por framework; smoke SAP requiere autorización externa futura. |
| SAP DI API | `SapDiApiClient` devuelve no implementado; no hay `SAPbobsCOM`, `ComImport` o `DllImport`. | No aplicable al retarget compilado; soporte real sigue bloqueado por SDK/COM SAP. |
| SAP HANA | Se resuelve `Sap.Data.Hana` mediante `DbProviderFactories`, sin PackageReference versionado. | Bloqueado para certificación .NET 10 hasta conocer versión/proveedor instalado en el host. |
| APIs obsoletas sensibles | No se encontraron BinaryFormatter, WebClient/WebRequest, ServicePointManager, Thread.Abort, crypto legacy, COM/PInvoke propios. | Riesgo bajo, sujeto a analyzers y dependencias. |
| `AppDomain.UnhandledException` | Usado por `GlobalUiExceptionHandler`; la API sigue disponible. | Requiere smoke de captura de errores UI, no un cambio preventivo. |

Microsoft documenta específicamente el cambio de
[`BackgroundService.ExecuteAsync`](https://learn.microsoft.com/en-us/dotnet/core/compatibility/extensions/10.0/backgroundservice-executeasync-task)
y las [obsolescencias WinForms de .NET 10](https://learn.microsoft.com/en-us/dotnet/core/compatibility/windows-forms/10.0/obsolete-apis).

## Publicación, RID y versionado de assemblies

### Estado actual

- Ningún proyecto define `RuntimeIdentifier`, `RuntimeIdentifiers`, `SelfContained`, `PublishSingleFile`, `PublishTrimmed` o `PublishReadyToRun`.
- No existen perfiles `.pubxml`.
- El build normal es portable/framework-dependent y AnyCPU; no demuestra un artefacto `win-x64`.
- SqlClient introduce `Microsoft.Data.SqlClient.SNI.runtime 6.0.2`; WinForms/DevExpress incorpora dependencias Windows. Deben probarse los assets concretos de `win-x64`.
- Las plantillas operativas locales apuntan a `bin\Debug\net9.0\*.dll`.
- Las plantillas SCM esperan un ejecutable en una release versionada, pero el repositorio no define todavía cómo publicarlo.

La futura publicación debe usar proyecto por proyecto y declarar explícitamente RID y modalidad. Microsoft identifica `win-x64` en el
[catálogo oficial de RID](https://learn.microsoft.com/en-us/dotnet/core/rid-catalog)
y establece que `dotnet publish` es la vía soportada para preparar el artefacto en
[dotnet publish](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish).

### Decisión pendiente de producción

| Modalidad | Ventaja | Riesgo/owner |
|---|---|---|
| Framework-dependent `win-x64` | Artefacto menor; patch centralizado del runtime. | Infra debe instalar/parchear .NET 10 Desktop/ASP.NET/runtime según host. |
| Self-contained `win-x64` | Runtime encapsulado y rollback binario más directo. | Desarrollo/operaciones deben republicar cada patch de seguridad y controlar tamaño/SBOM. |

No se recomienda single-file ni trimming para el primer piloto: reflexión, configuración, DevExpress y native assets amplían el riesgo sin aportar al objetivo de retarget.

### AssemblyVersion, FileVersion e InformationalVersion

Los proyectos no declaran propiedades de versión. El SDK genera actualmente:

- `AssemblyVersion = 1.0.0.0`
- `FileVersion = 1.0.0.0`
- `InformationalVersion = 1.0.0+4df7ce630edb8bcafb92a82bac5737d4be669915`

`NuanSystem.SriWorker` usa `WorkerVersionResolver` para preferir `AssemblyInformationalVersion` y lo expone por heartbeat/monitor. El plan de publicación debe inyectar versiones distintas y verificables para `pilot1`/`pilot2`, sin depender del TFM como versión de producto.

## Matriz de riesgos y bloqueos

| ID | Riesgo/bloqueo | Nivel | Tratamiento requerido |
|---|---|---:|---|
| B10-01 | SDK/runtime .NET 10 ausente. | Bloqueante | Instalar último patch 10.0 aprobado fuera de esta fase. |
| B10-02 | Visual Studio 2026 ausente. | Bloqueante WinForms | Instalar IDE/workloads compatibles y abrir Designers. |
| B10-03 | DevExpress local 25.2.5 vs NuGet 25.2.6 y rutas absolutas. | Alto | Unificar versión/fuente y validar design-time/runtime. |
| B10-04 | No hay `global.json`, lock, CPM ni CI. | Alto | Definir reproducibilidad antes del primer retarget compartido. |
| B10-05 | Microsoft packages mezclan majors 9 y 10. | Alto | Alinear por familia y comparar grafo restore. |
| B10-06 | Cambio de scheduling de `BackgroundService`. | Alto | Tests de orden/startup/mutex/heartbeat/shutdown de todos los hosts. |
| B10-07 | Publicación productiva/RID sin contrato. | Alto | Elegir FDD/SCD, publicar `win-x64`, manifest/SBOM/hash y rollback. |
| B10-08 | HANA provider no versionado. | Alto si se habilita SAP HANA | Inventariar versión y soporte oficial SAP antes del gate SAP. |
| B10-09 | Test SDK y runners anteriores a .NET 10. | Medio | Probar actuales; actualizar tooling en commit aislado si hace falta. |
| B10-10 | Scripts con rutas `net9.0`/Debug. | Medio | Versionar rutas de publish `net10.0` sin ejecutar servicios. |
| B10-11 | OpenAPI 3.1/modelos de ASP.NET Core 10. | Medio | Snapshot/diff del contrato y clientes. |
| B10-12 | Sin smoke visual/Designer .NET 10. | Alto | Abrir formas representativas, revisar `.resx` y diff semántico. |

## Estrategia recomendada

Se recomienda una **migración escalonada dentro de una única línea de entrega**, no múltiples versiones productivas permanentes:

1. preparar toolchain reproducible y baseline;
2. retarget Domain/Shared/Application/Services/ViewModels y tests;
3. retarget Infrastructure/Persistence/SapIntegration;
4. alinear y retarget API;
5. retarget workers, validando el cambio de `BackgroundService`;
6. alinear DevExpress y retarget WinForms;
7. definir/publicar `win-x64` pilot1 y pilot2;
8. completar validación deshabilitada y solicitar aprobación separada para cualquier runtime externo.

La solución debe terminar homogénea en .NET 10. Un estado híbrido solo es aceptable dentro de commits intermedios compilables y no como arquitectura de producción.

## Criterio Go/No-Go

### GO para iniciar implementación

Todos obligatorios:

- SDK .NET 10 LTS en último patch soportado y `dotnet --info` capturado.
- Visual Studio 2026 con Managed Desktop y ASP.NET/Web.
- DevExpress 25.2.x único, 25.2.3 o superior, disponible en build y Designer.
- Decisión de `global.json`, restore/feeds y pipeline.
- Decisión FDD o SCD para API, workers y WinForms; RID `win-x64`.
- Baseline .NET 9 limpio y pruebas conocidas.
- Plan de commits/rollback aceptado.
- Autorización expresa para implementar; esta auditoría no la concede.

### GO para aceptar la migración

- `dotnet restore`, build y 473 pruebas activas (o más) pasan en .NET 10 con cero fallos y cero warnings no aceptados.
- Los 5 tests diagnósticos omitidos siguen clasificados y no se convierten en falsos éxitos.
- API, auth, JSON/OpenAPI y clientes pasan pruebas de contrato.
- Los tres workers y el hosted service API pasan lifecycle, mutex, heartbeat, cancelación y shutdown.
- SRI Worker informa exactamente la versión pilot y permanece `Enabled=false`.
- Publicaciones FDD/SCD aprobadas arrancan en el host objetivo sin dependencias faltantes.
- WinForms abre y renderiza formularios representativos en Designer/ejecución sin cambios involuntarios.
- DevExpress, SqlClient/SNI, EventLog y ServiceProcess usan los assets esperados.
- No hay cambios funcionales, SQL ni activaciones ocultas.

### NO-GO

Cualquiera de los siguientes:

- toolchain fuera de soporte o mezcla no reproducible;
- warnings/errores de restore/build;
- diferencia funcional no explicada;
- fallo de Designer o mutación no intencional de `.Designer.cs`/`.resx`;
- WorkerVersion incorrecta;
- cambio de orden/lifecycle que permita heartbeat, claims o proceso antes del gate;
- artefacto sin RID/modalidad/version/hash;
- dependencia SAP/HANA/DevExpress sin evidencia del proveedor;
- necesidad de SQL, certificados, servicios o llamadas externas sin aprobación separada.

## Validación del estado actual

Comandos ejecutados exclusivamente sobre .NET 9:

```text
dotnet build NuanSystem.sln --no-restore
Compilación correcta. 0 advertencias, 0 errores.

dotnet test NuanSystem.sln --no-build --no-restore
478 total: 473 superadas, 5 omitidas, 0 fallidas.
```

No se ejecutó restore, publish, build `net10.0`, API, WinForms, worker, SQL, SAP ni SRI.

## Referencias oficiales

- [.NET Support Policy](https://dotnet.microsoft.com/en-us/platform/support/policy)
- [.NET 10 breaking changes](https://learn.microsoft.com/en-us/dotnet/core/compatibility/10)
- [.NET SDK, MSBuild and Visual Studio versioning](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs)
- [ASP.NET Core 10 release notes](https://learn.microsoft.com/en-us/aspnet/core/release-notes/aspnetcore-10.0?view=aspnetcore-10.0)
- [Windows Forms obsoletions in .NET 10](https://learn.microsoft.com/en-us/dotnet/core/compatibility/windows-forms/10.0/obsolete-apis)
- [BackgroundService behavior in .NET 10](https://learn.microsoft.com/en-us/dotnet/core/compatibility/extensions/10.0/backgroundservice-executeasync-task)
- [Microsoft.Data.SqlClient 7.0 and .NET 10](https://learn.microsoft.com/en-us/sql/connect/ado-net/introduction-microsoft-data-sqlclient-namespace?view=sql-server-ver17)
- [DevExpress v25.2 and .NET 10/Visual Studio 2026](https://supportcenter.devexpress.com/ticket/details/t1306812/devexpress-v25-2-and-visual-studio-2026-net-10-compatibility)
- [DevExpress v25.2 What's New](https://www.devexpress.com/subscriptions/new-2025-2.xml)
