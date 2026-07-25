# Ejecución de la migración de NuanSystem a .NET 10

## Estado

- Fecha: 2026-07-25.
- Rama: `refactor/codex-skills-v7-2-dotnet10-migration`.
- Baseline: `bd37ed0b70be180533bb7b70aed45d253e07f9ca`.
- Alcance: toolchain, TFM, paquetes de plataforma, compatibilidad de compilación y publicación local.
- Resultado: **GO técnico**.
- Visual Studio Designer: validado manualmente por el propietario con Visual Studio Enterprise 2026.

No se ejecutaron SQL, API, WinForms, servicios Windows, workers, SAP ni SRI. No se modificaron
bases, configuraciones locales, certificados, secretos, permisos, menús ni datos.

## Toolchain

| Componente | Resultado |
|---|---|
| SDK fijado por `global.json` | `10.0.302`, `latestFeature`, sin prerelease |
| Microsoft.NETCore.App | `10.0.10` |
| Microsoft.AspNetCore.App | `10.0.10` |
| Microsoft.WindowsDesktop.App | `10.0.10` |
| DevExpress | Components 25.2 instalado en la máquina |
| Visual Studio | Enterprise 2026 18.8.1 |

El SDK y los runtimes .NET 9 permanecen instalados para permitir rollback y convivencia.
No se instaló ni actualizó DevExpress.

## Cambios aplicados

- Los 16 proyectos de `NuanSystem.sln` pasan de `net9.0`/`net9.0-windows` a
  `net10.0`/`net10.0-windows`.
- Los paquetes explícitos `Microsoft.AspNetCore.*`, `Microsoft.Extensions.*`,
  `Microsoft.Extensions.Hosting.WindowsServices` y `System.Diagnostics.EventLog`
  se alinean a `10.0.10`.
- `Swashbuckle.AspNetCore` se actualiza a `10.2.3`.
- El grafo resultante resuelve `Microsoft.OpenApi 2.7.5`, versión que corrige
  GHSA-v5pm-xwqc-g5wc.
- La colisión entre `System.Windows.Forms.MenuItem` y el modelo de seguridad se resuelve
  mediante un alias explícito. No cambia comportamiento ni archivos `.Designer.cs`.

## Restore, build y pruebas

| Gate | Resultado |
|---|---|
| Restore neutral | Correcto, 16/16 proyectos |
| Restore `win-x64` | Correcto, 16/16 proyectos |
| Build Debug | 0 advertencias, 0 errores |
| Build Release | 0 advertencias, 0 errores |
| Tests Release | 473 superadas, 5 diagnósticas omitidas, 0 fallidas; 478 total |
| `git diff --check` | Correcto |

Las cinco pruebas omitidas continúan siendo diagnósticos condicionados a infraestructura real;
no son regresiones de la migración.

## Publicaciones locales

Se generaron artefactos framework-dependent `Release/win-x64` fuera del repositorio para:

- `NuanSystem.Api`
- `NuanSystem.SyncWorker`
- `NuanSystem.MasterBranchSyncWorker`
- `NuanSystem.SriWorker`
- `NuanSystem.WinForms`

Los runtime configs declaran .NET 10. Los artefactos no contienen
`appsettings.Local.json`, `.env`, certificados, claves privadas, respaldos, XML SRI,
JWT o marcadores de secretos. Los únicos XML encontrados son archivos de documentación
de assemblies DevExpress.

## DevExpress y WinForms

- El build y publish usan exclusivamente la instalación local DevExpress 25.2 y su paquete
  offline asociado; no se consultó ni instaló otra versión de DevExpress.
- El artefacto WinForms contiene una sola copia por nombre de assembly DevExpress.
- Los file versions observados pertenecen al mismo set instalado y se expresan como
  `25.2.5.26075` o `25.2.6.0`, según el assembly.
- No se modificaron `.Designer.cs`, `.resx`, layout, controles corporativos ni tipografía.
- El propietario abrió y revisó los diseños con Visual Studio Enterprise 2026 18.8.1.
- La revisión no produjo cambios en `.Designer.cs`, `.resx` ni `.csproj`.

## Riesgos y gates pendientes

1. El defecto original del smoke runtime fue corregido mediante la migración tenant `123`,
   validada con materialización Dapper real y apertura del Monitor SRI.
2. El cierre autenticado de Fase 7.2.2 validó 401/403/200, empresa activa, rechazo de empresa no
   disponible y aislamiento entre `DEMO`, `DEMO-REMIGIO` y `DEMO-CANARIS` sobre .NET 10. Consultar
   [DOTNET-10-RUNTIME-SMOKE.md](../operations/DOTNET-10-RUNTIME-SMOKE.md).
3. Fase 7.3 validó manifests, hashes, inventario de dependencias, versionado
   `pilot1`/`pilot2` y rollback inmutable `pilot1 -> pilot2 -> pilot1`.
4. No habilitar procesamiento, SQL, SAP o SRI como consecuencia de esta migración.

## Cierre de artefactos de Fase 7.3

Se incorporaron herramientas versionadas para publicar, verificar y seleccionar
releases externas sin mutar sus archivos. Ambos pilotos publican los cinco hosts
por separado como `Release/win-x64`, framework-dependent, sin trimming ni
single-file.

| Evidencia | Pilot1 | Pilot2 |
|---|---|---|
| Versión | `7.1.0-dotnet10-pilot1+9275f7c2` | `7.1.0-dotnet10-pilot2+664c48a4` |
| Commit fuente | `9275f7c2fb7bab46afe6ccdff08f3e42e5bc19d1` | `664c48a42b9e23b8f4a69dde17eae11d9a3d214a` |
| Proyectos | 5 | 5 |
| Archivos | 645 | 645 |
| Dependencias | 135 | 135 |
| SHA-256 del manifiesto | `EF7B00AC030849DF18AACB8F24302D6A2D7DCA812EE070012ADC91D5D50A8062` | `0ED93ECA6F04D31E82B4A0087955B56DAB0D8223D41825D620E6F1BAC9F53116` |

Los dos artefactos aprobaron verificación independiente de archivos, hashes,
versiones y configuración segura. El puntero externo avanzó de pilot1 a pilot2
y regresó a pilot1 sin modificar ninguna release. No se instalaron servicios,
no se iniciaron procesos y no hubo SQL, SAP ni SRI.

El cierre posterior aprobó build `Release` con cero advertencias y errores, y
la suite completa con 483 pruebas superadas, 5 diagnósticas omitidas y cero
fallos.

El procedimiento reproducible y sus gates están en
[DOTNET-10-RELEASE-ARTIFACTS.md](../operations/DOTNET-10-RELEASE-ARTIFACTS.md).

## Rollback

La implementación es reversible por Git. .NET 9 permanece instalado y no hubo mutaciones de
datos o infraestructura. Ante un fallo visual o runtime, se debe revertir el conjunto de commits
de esta rama; no usar `git reset --hard`, no modificar estados SQL y no reutilizar artefactos
.NET 10 rechazados.
