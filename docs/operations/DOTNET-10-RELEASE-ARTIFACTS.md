# Artefactos de release .NET 10

## Propósito

Este runbook define la publicación reproducible y verificable de NuanSystem para
Windows x64. Complementa `DOTNET-10-MIGRATION-PLAN.md` y no instala servicios,
inicia workers ni habilita integraciones.

## Alcance aprobado

Cada release publica por separado:

- `NuanSystem.Api`;
- `NuanSystem.SyncWorker`;
- `NuanSystem.MasterBranchSyncWorker`;
- `NuanSystem.SriWorker`;
- `NuanSystem.WinForms`.

El contrato es:

- configuración `Release`;
- runtime `win-x64`;
- framework-dependent;
- sin trimming;
- sin single-file;
- `AssemblyVersion`, `FileVersion` e `InformationalVersion` explícitas;
- DevExpress resuelto desde la instalación y referencias existentes del proyecto;
- configuración local y secretos externos al artefacto;
- todos los workers deshabilitados en la copia publicada.

## Herramientas versionadas

| Herramienta | Responsabilidad |
|---|---|
| `tools/release/Publish-NuanSystemRelease.ps1` | Publica los cinco hosts, aplica configuración segura y genera manifiestos. |
| `tools/release/Test-NuanSystemRelease.ps1` | Recalcula hashes, versiones, archivos permitidos y contrato de configuración. |
| `tools/release/Set-NuanSystemActiveRelease.ps1` | Simula selección y rollback mediante un puntero externo, sin mutar releases. |

La publicación exige un working tree limpio. Una carpeta de release existente
es inmutable y no se sobrescribe.

## Comandos

Restaurar una vez para `win-x64`:

```powershell
dotnet restore NuanSystem.sln --runtime win-x64
```

Publicar:

```powershell
.\tools\release\Publish-NuanSystemRelease.ps1 `
    -Pilot pilot1 `
    -OutputRoot "<directorio externo>" `
    -NoRestore
```

Verificar:

```powershell
.\tools\release\Test-NuanSystemRelease.ps1 `
    -ReleaseDirectory "<directorio externo>\<release>"
```

Seleccionar una release para una simulación de despliegue:

```powershell
.\tools\release\Set-NuanSystemActiveRelease.ps1 `
    -ReleaseDirectory "<directorio externo>\<release>" `
    -PointerPath "<directorio externo>\active-release.json"
```

## Evidencia obligatoria

Cada release contiene:

- `release-manifest.json`;
- `dependency-inventory.json`;
- `file-hashes.json`;
- cinco carpetas de aplicación independientes.

El verificador debe confirmar:

- versión informativa exacta en cada entry point;
- conjunto de archivos sin adiciones ni faltantes;
- SHA-256 correcto por archivo;
- ausencia de `appsettings.Local.json`;
- ausencia de certificados, claves privadas, logs, respaldos y XML sensibles;
- conexiones, claves de cifrado y firma sin valores;
- workers y reintentos deshabilitados.

## Evidencia de pilot1

| Evidencia | Resultado |
|---|---|
| Release | `7.1.0-dotnet10-pilot1+9275f7c2` |
| Commit fuente | `9275f7c2fb7bab46afe6ccdff08f3e42e5bc19d1` |
| Proyectos | 5 |
| Archivos inventariados | 645 |
| Dependencias inventariadas | 135 |
| SHA-256 del manifiesto de hashes | `EF7B00AC030849DF18AACB8F24302D6A2D7DCA812EE070012ADC91D5D50A8062` |
| Secretos detectados | No |
| Workers habilitados | No |

## Rollback

El rollback de artefactos no recompila ni altera una release. Debe:

1. validar `pilot1`;
2. seleccionar `pilot1`;
3. validar y seleccionar `pilot2`;
4. volver a validar `pilot1`;
5. restaurar el puntero a `pilot1`;
6. confirmar que los SHA-256 originales no cambiaron.

La instalación SCM, el cambio de configuración externa y el arranque de
procesos pertenecen a una fase operativa posterior.
