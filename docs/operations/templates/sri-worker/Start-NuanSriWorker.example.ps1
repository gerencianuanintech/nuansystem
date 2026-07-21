[CmdletBinding(SupportsShouldProcess)] param([string]$ServiceName='NuanSystem.SriWorker')
$ErrorActionPreference='Stop'
$service=Get-Service -Name $ServiceName -ErrorAction Stop
if($service.Status -ne 'Stopped'){ throw 'El servicio debe estar detenido antes de este arranque controlado.' }
if($PSCmdlet.ShouldProcess($ServiceName,'Habilitar inicio manual e iniciar')){ Set-Service -Name $ServiceName -StartupType Manual; Start-Service -Name $ServiceName }
