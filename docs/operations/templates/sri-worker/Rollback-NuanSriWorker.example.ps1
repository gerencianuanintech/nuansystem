[CmdletBinding(SupportsShouldProcess)] param([Parameter(Mandatory)][string]$PreviousReleaseDirectory,[string]$ServiceName='NuanSystem.SriWorker')
$ErrorActionPreference='Stop'
$service=Get-CimInstance Win32_Service -Filter "Name='$ServiceName'"
if($null -eq $service -or $service.State -ne 'Stopped'){ throw 'El servicio debe estar detenido para rollback.' }
$executable=[IO.Path]::GetFullPath((Join-Path $PreviousReleaseDirectory 'NuanSystem.SriWorker.exe'))
if(-not (Test-Path -LiteralPath $executable -PathType Leaf)){ throw 'Release anterior invalida.' }
if($PSCmdlet.ShouldProcess($ServiceName,'Volver a release anterior')){ & sc.exe config $ServiceName binPath= ('"{0}"' -f $executable) | Out-Null }
# SQL es forward-only; esta plantilla no revierte esquema ni altera cola, XML o auditoria.
