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

1. Ejecutar smokes controlados de API, workers y WinForms en una autorización separada.
2. Definir manifests, hashes, versionado pilot1/pilot2 y rollback antes de promover.
3. No habilitar procesamiento, SQL, SAP o SRI como consecuencia de esta migración.

## Rollback

La implementación es reversible por Git. .NET 9 permanece instalado y no hubo mutaciones de
datos o infraestructura. Ante un fallo visual o runtime, se debe revertir el conjunto de commits
de esta rama; no usar `git reset --hard`, no modificar estados SQL y no reutilizar artefactos
.NET 10 rechazados.
