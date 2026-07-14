<#
Manual template for running NuanSystem.MasterBranchSyncWorker outside Codex.

Purpose:
- Execute the real worker from a normal Windows console using the real Windows user.
- Preserve secrets: do not print connection strings, passwords or Security:EncryptionKey.
- Use this only for the controlled Warehouse final test event OutboxId=20003.

This script intentionally does not use Encrypt=False and does not override SQL policy.
#>

[CmdletBinding()]
param(
    [string]$RepositoryRoot = "E:\Aplicaciones\Nuanintech\nuansystem"
)

$ErrorActionPreference = "Stop"

function Get-Presence {
    param([AllowNull()][string]$Value)

    if ([string]::IsNullOrWhiteSpace($Value)) {
        return "AUSENTE"
    }

    return "PRESENTE"
}

function Read-JsonFile {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "No existe el archivo requerido: $Path"
    }

    return Get-Content -Raw -LiteralPath $Path | ConvertFrom-Json
}

$workerDir = Join-Path $RepositoryRoot "src\Backend\NuanSystem.MasterBranchSyncWorker"
$workerDll = Join-Path $workerDir "bin\Debug\net9.0\NuanSystem.MasterBranchSyncWorker.dll"
$projectLocalSettings = Join-Path $workerDir "appsettings.Local.json"
$outputLocalSettings = Join-Path $workerDir "bin\Debug\net9.0\appsettings.Local.json"

if (-not (Test-Path -LiteralPath $workerDir)) {
    throw "No existe la carpeta del worker: $workerDir"
}

if (-not (Test-Path -LiteralPath $workerDll)) {
    throw "No existe el binario del worker. Ejecute primero: dotnet build NuanSystem.sln --no-restore"
}

$env:DOTNET_ENVIRONMENT = "Local"
Set-Location -LiteralPath $workerDir

$localConfig = Read-JsonFile $projectLocalSettings

Write-Host "CurrentDirectory: $(Get-Location)"
Write-Host "WindowsUser: $([Environment]::UserName)"
Write-Host "DOTNET_ENVIRONMENT: $env:DOTNET_ENVIRONMENT"
Write-Host "Project appsettings.Local.json: $(if (Test-Path -LiteralPath $projectLocalSettings) { 'PRESENTE' } else { 'AUSENTE' })"
Write-Host "Output appsettings.Local.json: $(if (Test-Path -LiteralPath $outputLocalSettings) { 'PRESENTE' } else { 'AUSENTE' })"
Write-Host "ConnectionStrings:SqlServerAdmin: $(Get-Presence $localConfig.ConnectionStrings.SqlServerAdmin)"
Write-Host "Security:EncryptionKey: $(Get-Presence $localConfig.Security.EncryptionKey)"
Write-Host "MasterBranchSyncWorker:Enabled: $($localConfig.MasterBranchSyncWorker.Enabled)"
Write-Host "MasterBranchSyncWorker:SkeletonMode: $($localConfig.MasterBranchSyncWorker.SkeletonMode)"
Write-Host "MasterBranchSyncWorker:BatchSize: $($localConfig.MasterBranchSyncWorker.BatchSize)"
Write-Host "MasterBranchSyncWorker:EnabledEntityAppliers contains Warehouse: $(@($localConfig.MasterBranchSyncWorker.EnabledEntityAppliers) -contains 'Warehouse')"
Write-Host "SqlConnectionPolicy:Encrypt: $($localConfig.SqlConnectionPolicy.Encrypt)"
Write-Host "SqlConnectionPolicy:TrustServerCertificate: $($localConfig.SqlConnectionPolicy.TrustServerCertificate)"

Write-Warning "Este comando ejecutara el worker real. Si OutboxId=20003 sigue Pending, puede procesarlo hacia la sucursal."
Write-Warning "No continue si no verifico antes el estado de SyncOutbox, SyncOutboxTargets, Warehouses y SyncInbox."

$confirmation = Read-Host "Escriba EJECUTAR para iniciar el worker real"
if ($confirmation -ne "EJECUTAR") {
    Write-Host "Operacion cancelada por el usuario."
    exit 1
}

Write-Host "Iniciando worker real..."
dotnet .\bin\Debug\net9.0\NuanSystem.MasterBranchSyncWorker.dll
exit $LASTEXITCODE
