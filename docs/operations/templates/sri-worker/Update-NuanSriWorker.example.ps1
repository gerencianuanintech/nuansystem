[CmdletBinding(SupportsShouldProcess)] param([Parameter(Mandatory)][string]$ReleaseDirectory,[string]$ServiceName='NuanSystem.SriWorker')
$ErrorActionPreference='Stop'
$service=Get-CimInstance Win32_Service -Filter "Name='$ServiceName'"
if($null -eq $service -or $service.State -ne 'Stopped'){ throw 'El servicio debe existir y estar detenido.' }
$executable=[IO.Path]::GetFullPath((Join-Path $ReleaseDirectory 'NuanSystem.SriWorker.exe'))
if(-not (Test-Path -LiteralPath $executable -PathType Leaf)){ throw 'Release versionada invalida.' }
if($PSCmdlet.ShouldProcess($ServiceName,'Apuntar a release versionada')){ & sc.exe config $ServiceName binPath= ('"{0}"' -f $executable) | Out-Null }
# La configuracion externa bajo ProgramData no se copia ni reemplaza.
