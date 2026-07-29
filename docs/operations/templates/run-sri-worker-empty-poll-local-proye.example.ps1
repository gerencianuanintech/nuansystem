<#
Manual template for validating NuanSystem.SriWorker outside the Codex sandbox.

This test requires zero eligible queue rows and must not call the SRI. It keeps
SQL encryption and certificate validation enabled, does not print secrets, and
enables the worker only in this PowerShell process. Press Ctrl+C after at least
two empty polling cycles (recommended: 6 seconds).
#>

[CmdletBinding()]
param(
    [string]$RepositoryRoot = "E:\Aplicaciones\Nuanintech\nuansystem"
)

$ErrorActionPreference = "Stop"

function Get-Presence {
    param([AllowNull()][string]$Value)
    if ([string]::IsNullOrWhiteSpace($Value)) { return "AUSENTE" }
    return "PRESENTE"
}

$windowsIdentity = [System.Security.Principal.WindowsIdentity]::GetCurrent().Name
if ($windowsIdentity -match "CodexSandboxOffline") {
    throw "Esta validacion debe ejecutarse desde una consola PowerShell normal del usuario Windows, no desde Codex."
}

$workerDirectory = Join-Path $RepositoryRoot "src\Backend\SyncSRI\NuanSystem.SriWorker"
$workerDll = Join-Path $workerDirectory "bin\Debug\net9.0\NuanSystem.SriWorker.dll"
$localSettingsPath = Join-Path $workerDirectory "appsettings.Local.json"

if (-not (Test-Path -LiteralPath $workerDll)) {
    throw "No existe el binario. Ejecute primero: dotnet build NuanSystem.sln --no-restore"
}
if (-not (Test-Path -LiteralPath $localSettingsPath)) {
    throw "Falta appsettings.Local.json ignorado por Git con SqlServerAdmin y Security:EncryptionKey."
}

$localSettingsRaw = Get-Content -Raw -LiteralPath $localSettingsPath
try {
    $localSettings = $localSettingsRaw | ConvertFrom-Json
}
catch {
    throw "appsettings.Local.json no es un documento JSON valido. Debe contener un objeto exterior con ConnectionStrings, Security y SqlConnectionPolicy. No pegue solamente el bloque SqlConnectionPolicy."
}
if ([string]::IsNullOrWhiteSpace($localSettings.ConnectionStrings.SqlServerAdmin) -or
    [string]::IsNullOrWhiteSpace($localSettings.Security.EncryptionKey)) {
    throw "SqlServerAdmin y Security:EncryptionKey son obligatorios en appsettings.Local.json."
}
$sqlServerAdmin = [string]$localSettings.ConnectionStrings.SqlServerAdmin
if ($sqlServerAdmin -match "COLOCA_AQUI|<|>" -or
    $sqlServerAdmin -notmatch "(?i)(^|;)\s*(Server|Data Source)\s*=" -or
    $sqlServerAdmin -notmatch "(?i)(^|;)\s*(Database|Initial Catalog)\s*=") {
    throw "ConnectionStrings:SqlServerAdmin conserva un placeholder o no tiene formato de cadena SQL Server. Copie el valor local real sin imprimirlo."
}
if ([string]$localSettings.Security.EncryptionKey -match "COLOCA_AQUI|<|>") {
    throw "Security:EncryptionKey conserva el placeholder. Copie la clave local real sin imprimirla."
}
if ($localSettings.SqlConnectionPolicy.Encrypt -ne $true -or
    $localSettings.SqlConnectionPolicy.TrustServerCertificate -ne $false) {
    throw "La prueba exige Encrypt=true y TrustServerCertificate=false."
}

$env:DOTNET_ENVIRONMENT = "Local"
$env:SriWorker__Enabled = "true"
$env:SriWorker__EmptyQueueDelaySeconds = "2"

Set-Location -LiteralPath $workerDirectory
Write-Host "WindowsIdentity: $windowsIdentity"
Write-Host "DOTNET_ENVIRONMENT: $env:DOTNET_ENVIRONMENT"
Write-Host "SqlServerAdmin: $(Get-Presence $localSettings.ConnectionStrings.SqlServerAdmin)"
Write-Host "Security:EncryptionKey: $(Get-Presence $localSettings.Security.EncryptionKey)"
Write-Host "SqlConnectionPolicy:Encrypt: $($localSettings.SqlConnectionPolicy.Encrypt)"
Write-Host "SqlConnectionPolicy:TrustServerCertificate: $($localSettings.SqlConnectionPolicy.TrustServerCertificate)"
Write-Warning "Confirme antes que Pending/RetryScheduled elegibles, locks e intentos nuevos sean cero."
Write-Warning "Presione Ctrl+C despues de al menos 6 segundos. No deje el worker ejecutandose."

$confirmation = Read-Host "Escriba EJECUTAR para iniciar el polling vacio"
if ($confirmation -ne "EJECUTAR") {
    Write-Host "Operacion cancelada por el usuario."
    exit 1
}

dotnet .\bin\Debug\net9.0\NuanSystem.SriWorker.dll
exit $LASTEXITCODE
