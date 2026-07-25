[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ReleaseDirectory
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$resolvedRelease = (Resolve-Path -LiteralPath $ReleaseDirectory).Path
$manifestPath = Join-Path $resolvedRelease "release-manifest.json"
$hashManifestPath = Join-Path $resolvedRelease "file-hashes.json"
$inventoryPath = Join-Path $resolvedRelease "dependency-inventory.json"

foreach ($required in @($manifestPath, $hashManifestPath, $inventoryPath)) {
    if (-not (Test-Path -LiteralPath $required -PathType Leaf)) {
        throw "Falta un archivo obligatorio de release."
    }
}

$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
$hashManifest = Get-Content -Raw -LiteralPath $hashManifestPath | ConvertFrom-Json
$inventory = Get-Content -Raw -LiteralPath $inventoryPath | ConvertFrom-Json

if ($manifest.RuntimeIdentifier -ne "win-x64" -or
    $manifest.DeploymentMode -ne "framework-dependent" -or
    $manifest.SelfContained -ne $false -or
    $manifest.PublishTrimmed -ne $false -or
    $manifest.PublishSingleFile -ne $false) {
    throw "El contrato de publicacion no coincide con Fase 7.3."
}

if (@($manifest.Projects).Count -ne 5 -or $inventory.Release -ne $manifest.Release) {
    throw "El inventario de proyectos o dependencias no coincide."
}

if ($manifest.SafeConfiguration.SyncWorkerEnabled -ne $false -or
    $manifest.SafeConfiguration.SyncRetryEnabled -ne $false -or
    $manifest.SafeConfiguration.MasterBranchSyncWorkerEnabled -ne $false -or
    $manifest.SafeConfiguration.SriWorkerEnabled -ne $false -or
    $manifest.SafeConfiguration.SriWorkerProductionEnabled -ne $false -or
    $manifest.SafeConfiguration.LocalSettingsIncluded -ne $false -or
    $manifest.SafeConfiguration.SensitiveValuesPresent -ne $false) {
    throw "La configuracion segura del artefacto no esta cerrada."
}

$expectedPaths = @($hashManifest.Files | ForEach-Object { [string]$_.Path })
$actualPaths = @(
    Get-ChildItem -LiteralPath $resolvedRelease -Recurse -File |
        Where-Object { $_.FullName -ne $hashManifestPath } |
        ForEach-Object { $_.FullName.Substring($resolvedRelease.Length + 1).Replace("\", "/") } |
        Sort-Object
)
if (@(Compare-Object ($expectedPaths | Sort-Object) $actualPaths).Count -ne 0) {
    throw "El conjunto de archivos no coincide con el manifest de hashes."
}

foreach ($file in $hashManifest.Files) {
    $fullPath = Join-Path $resolvedRelease ([string]$file.Path).Replace("/", "\")
    $actualHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $fullPath).Hash
    if ($actualHash -ne [string]$file.Sha256) {
        throw "Hash invalido para $($file.Path)."
    }
}

$forbidden = @(
    Get-ChildItem -LiteralPath $resolvedRelease -Recurse -File |
        Where-Object {
            $_.Name -ieq "appsettings.Local.json" -or
            $_.Name -ieq ".env" -or
            $_.Extension -in @(".pfx", ".p12", ".pem", ".key", ".cer", ".crt", ".bak", ".log")
        }
)
if ($forbidden.Count -ne 0) {
    throw "La release contiene archivos prohibidos."
}

foreach ($project in $manifest.Projects) {
    $entryPointPath = Join-Path $resolvedRelease (Join-Path $project.Output $project.EntryPoint)
    $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($entryPointPath)
    if ($versionInfo.ProductVersion -ne $manifest.InformationalVersion) {
        throw "Version informativa inconsistente en $($project.Name)."
    }
}

[pscustomobject]@{
    Result = "Validated"
    Release = $manifest.Release
    SourceCommit = $manifest.SourceCommit
    ProjectCount = @($manifest.Projects).Count
    FileCount = @($hashManifest.Files).Count
    DependencyCount = @($inventory.Libraries).Count
    HashManifestSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $hashManifestPath).Hash
    SecretsDetected = $false
    WorkersEnabled = $false
} | ConvertTo-Json -Depth 5
