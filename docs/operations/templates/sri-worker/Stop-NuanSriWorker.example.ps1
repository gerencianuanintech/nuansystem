[CmdletBinding(SupportsShouldProcess)] param([string]$ServiceName='NuanSystem.SriWorker',[ValidateRange(30,300)][int]$TimeoutSeconds=60)
$ErrorActionPreference='Stop'
if($PSCmdlet.ShouldProcess($ServiceName,'Detener de forma cooperativa')){ Stop-Service -Name $ServiceName -ErrorAction Stop }
$service=Get-Service -Name $ServiceName -ErrorAction Stop
$service.WaitForStatus([ServiceProcess.ServiceControllerStatus]::Stopped,[TimeSpan]::FromSeconds($TimeoutSeconds))
