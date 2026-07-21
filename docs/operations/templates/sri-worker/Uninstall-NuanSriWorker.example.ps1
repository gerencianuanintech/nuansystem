[CmdletBinding(SupportsShouldProcess)] param([string]$ServiceName='NuanSystem.SriWorker')
$ErrorActionPreference='Stop'
$service=Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if($null -eq $service){ return }
if($service.Status -ne 'Stopped'){ throw 'Detenga el servicio antes de desinstalar.' }
if($PSCmdlet.ShouldProcess($ServiceName,'Eliminar solo el registro SCM')){ & sc.exe delete $ServiceName | Out-Null }
# Conserva cuenta, configuracion protegida, logs, releases, certificados y toda evidencia SQL.
