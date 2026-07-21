[CmdletBinding(SupportsShouldProcess)]
param(
    [Parameter(Mandatory)][ValidateNotNullOrEmpty()][string]$ReleaseDirectory,
    [Parameter(Mandatory)][ValidatePattern('^[^\\]+\\[^\\]+$')][string]$ServiceAccount,
    [string]$ServiceName = 'NuanSystem.SriWorker',
    [string]$DisplayName = 'NuanSystem SRI Worker',
    [string]$ProgramDataDirectory = "$env:ProgramData\NuanSystem\SriWorker",
    [switch]$RegisterEventSource
)
$ErrorActionPreference = 'Stop'
$executable = [IO.Path]::GetFullPath((Join-Path $ReleaseDirectory 'NuanSystem.SriWorker.exe'))
if (-not (Test-Path -LiteralPath $executable -PathType Leaf)) { throw 'No existe el ejecutable versionado indicado.' }
if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) { throw 'El servicio ya existe; use la plantilla de actualizacion.' }
$configDirectory = Join-Path $ProgramDataDirectory 'config'
$logDirectory = Join-Path $ProgramDataDirectory 'logs'
if ($PSCmdlet.ShouldProcess($ProgramDataDirectory, 'Crear directorios operativos y ACL')) {
    New-Item -ItemType Directory -Force -Path $configDirectory,$logDirectory | Out-Null
    & icacls.exe $ProgramDataDirectory /inheritance:r /grant:r '*S-1-5-32-544:(OI)(CI)(F)' "$ServiceAccount`:(OI)(CI)(RX)" | Out-Null
    & icacls.exe $logDirectory /grant:r "$ServiceAccount`:(OI)(CI)(M)" | Out-Null
}
if ($RegisterEventSource -and $PSCmdlet.ShouldProcess('Application', 'Registrar source de Windows Event Log')) {
    if (-not [Diagnostics.EventLog]::SourceExists('NuanSystem.SriWorker')) { New-EventLog -LogName Application -Source 'NuanSystem.SriWorker' }
}
if ($PSCmdlet.ShouldProcess($ServiceName, 'Crear servicio Windows deshabilitado')) {
    New-Service -Name $ServiceName -BinaryPathName ('"{0}"' -f $executable) -DisplayName $DisplayName -StartupType Disabled -Credential (Get-Credential -UserName $ServiceAccount -Message 'Credencial de la cuenta dedicada; no se almacena en el script.')
}
# Esta plantilla no crea cuentas ni concede "Log on as a service". Infraestructura prepara ambos previamente.
