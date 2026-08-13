[CmdletBinding()]
param(
    [string] $GeneratorPath,
    [string] $RepositoryRoot,
    [string] $FixturesPath,
    [Alias('Manifest')]
    [string] $ManifestPath,
    [string] $GeneratedPath,
    [switch] $KeepArtifacts
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if (-not $GeneratorPath) {
    $GeneratorPath = Join-Path $PSScriptRoot 'New-NuanAuxiliaryMaster.ps1'
}
if (-not $FixturesPath) {
    $FixturesPath = Join-Path (Split-Path $PSScriptRoot -Parent) 'assets\manifests'
}

function Assert-True {
    param([bool] $Condition, [string] $Message)
    if (-not $Condition) { throw $Message }
}

function Get-Utf8Text {
    param([string] $Path)
    return [System.IO.File]::ReadAllText($Path, [System.Text.UTF8Encoding]::new($false, $true))
}

function Get-RelativePath {
    param([string] $BasePath, [string] $Path)
    $baseFullPath = [System.IO.Path]::GetFullPath($BasePath).TrimEnd('\') + '\'
    $targetFullPath = [System.IO.Path]::GetFullPath($Path)
    $baseUri = [Uri]::new($baseFullPath)
    $targetUri = [Uri]::new($targetFullPath)
    return [Uri]::UnescapeDataString($baseUri.MakeRelativeUri($targetUri).ToString()).Replace('\', '/')
}

function Get-DirectorySnapshot {
    param([string] $Path)
    if (-not (Test-Path -LiteralPath $Path)) { return @{} }
    $snapshot = @{}
    foreach ($file in Get-ChildItem -LiteralPath $Path -Recurse -File -Force | Sort-Object FullName) {
        $relative = Get-RelativePath -BasePath $Path -Path $file.FullName
        $snapshot[$relative] = (Get-FileHash -LiteralPath $file.FullName -Algorithm SHA256).Hash
    }
    return $snapshot
}

function Assert-SnapshotsEqual {
    param([hashtable] $Expected, [hashtable] $Actual, [string] $Message)
    $expectedJson = $Expected.GetEnumerator() | Sort-Object Key | ConvertTo-Json -Compress
    $actualJson = $Actual.GetEnumerator() | Sort-Object Key | ConvertTo-Json -Compress
    Assert-True ($expectedJson -ceq $actualJson) $Message
}

function Invoke-Generator {
    param(
        [string] $Manifest,
        [ValidateSet('Validate','Propose','Preview','Diff','Scaffold')][string] $Mode,
        [string] $OutputPath,
        [string] $Root,
        [switch] $ExpectFailure
    )
    $arguments = @('-NoProfile','-File',$GeneratorPath,'-Manifest',$Manifest,'-Mode',$Mode,'-RepositoryRoot',$Root)
    if ($OutputPath) { $arguments += @('-OutputPath',$OutputPath) }
    $oldPreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $output = & powershell.exe @arguments 2>&1 | Out-String
        $exitCode = $LASTEXITCODE
    }
    finally { $ErrorActionPreference = $oldPreference }
    if ($ExpectFailure) {
        Assert-True ($exitCode -ne 0) "Se esperaba fallo en modo $Mode para $Manifest, pero terminó con código 0. Salida: $output"
    }
    else {
        Assert-True ($exitCode -eq 0) "El generador falló en modo $Mode para $Manifest. Código: $exitCode. Salida: $output"
    }
    return [pscustomobject]@{ ExitCode = $exitCode; Output = $output }
}

function Assert-NoResidualTokens {
    param([string] $Path)
    $matches = Get-ChildItem -LiteralPath $Path -Recurse -File | Select-String -Pattern '\{\{[^{}]+\}\}'
    $paths = @($matches | ForEach-Object Path | Sort-Object -Unique)
    Assert-True ($paths.Count -eq 0) "Hay tokens residuales en staging: $($paths -join ', ')"
}

function Assert-NoSecrets {
    param([string] $Path)
    $patterns = @(
        '(?i)password\s*[=:]\s*[^\s"'']+',
        '(?i)(database\s+user|db\s*user|sql\s*user|\buid\b)\s*[=:]\s*[^\s"'']+',
        '(?i)connectionstring\s*[=:]',
        '(?i)server\s*=.+;\s*(database|initial catalog)\s*=',
        '(?i)-----BEGIN (RSA |EC |OPENSSH )?PRIVATE KEY-----',
        '(?i)bearer\s+[A-Za-z0-9._-]{20,}',
        '(?i)(jwt|access[_-]?token|client[_-]?secret)\s*[=:]\s*[^\s"'']+'
    )
    foreach ($pattern in $patterns) {
        $match = Get-ChildItem -LiteralPath $Path -Recurse -File | Select-String -Pattern $pattern | Select-Object -First 1
        if ($null -ne $match) { throw "Posible secreto detectado en $($match.Path):$($match.LineNumber)." }
    }
}

function Assert-PathAndContract {
    param([string] $OutputPath, [pscustomobject] $Manifest)
    foreach ($required in @('manifest.normalized.json','generation-plan.json','integration-checklist.md','files')) {
        Assert-True (Test-Path -LiteralPath (Join-Path $OutputPath $required)) "Falta artefacto de staging: $required"
    }

    $normalized = Get-Utf8Text (Join-Path $OutputPath 'manifest.normalized.json') | ConvertFrom-Json
    $expectedRoute = if ($Manifest.api.route.StartsWith('/')) { $Manifest.api.route } else { '/' + $Manifest.api.route }
    Assert-True ($normalized.api.route -ceq $expectedRoute) 'La ruta normalizada no coincide con el manifiesto.'
    Assert-True ($normalized.api.formKey -ceq $Manifest.api.formKey) 'El FormKey normalizado no coincide con el manifiesto.'
    Assert-True ($normalized.navigation.menuCode -ceq $Manifest.navigation.menuCode) 'El código de menú normalizado no coincide.'

    Assert-True ($normalized.api.route -cmatch '^/api/definitions/inventory/[a-z0-9]+(?:-[a-z0-9]+)*$') 'La ruta API no cumple /api/definitions/inventory/<kebab-case>.'
    Assert-True ($normalized.api.formKey -cmatch '^[a-z0-9]+(?:-[a-z0-9]+)*$') 'El FormKey no es kebab-case.'
    Assert-True ($normalized.entity.singular -cmatch '^[A-Z][A-Za-z0-9]*$') 'entity.singular no es PascalCase.'
    Assert-True ($normalized.entity.plural -cmatch '^[A-Z][A-Za-z0-9]*$') 'entity.plural no es PascalCase.'
    Assert-True ($normalized.entity.table -cmatch '^dbo\.[A-Z][A-Za-z0-9]*$') 'entity.table no cumple dbo.<PascalCase>.'
    Assert-True (($normalized.navigation.path -join '/') -ceq 'Configuration/Definitions/Inventory') 'La navegación no apunta a Configuration/Definitions/Inventory.'

    $filesRoot = Join-Path $OutputPath 'files'
    $allFiles = Get-ChildItem -LiteralPath $filesRoot -Recurse -File
    Assert-True ($allFiles.Count -gt 0) 'El scaffold no generó archivos en files/.'
    $allText = ($allFiles | ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    Assert-True ($allText.Contains($expectedRoute)) 'La ruta API no aparece en los artefactos generados.'
    Assert-True ($allText.Contains($Manifest.api.formKey)) 'El FormKey no aparece en los artefactos generados.'
    Assert-True ($allText.Contains($Manifest.permissions.read)) 'El permiso READ no aparece en los artefactos generados.'
    Assert-True ($allText.Contains($Manifest.permissions.manage)) 'El permiso MANAGE no aparece en los artefactos generados.'
}

function Assert-SqlStaticGates {
    param([string] $OutputPath, [pscustomobject] $Manifest)
    $sqlFiles = @(Get-ChildItem -LiteralPath (Join-Path $OutputPath 'files') -Recurse -File -Filter '*.sql')
    Assert-True ($sqlFiles.Count -ge 2) 'Se esperaban al menos migraciones tenant y navegación Master.'
    $sqlText = ($sqlFiles | ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    foreach ($token in @('GlobalId','CreatedAt','UpdatedAt','IsDeleted','DeletedAt','SYSUTCDATETIME','CREATE OR ALTER PROCEDURE','AuditCatalogChanges')) {
        Assert-True ($sqlText -cmatch [regex]::Escape($token)) "El SQL generado no contiene el contrato obligatorio $token."
    }
    Assert-True ($sqlText -cmatch 'SP_NA_GET_') 'Falta procedimiento GET.'
    Assert-True ($sqlText -cmatch 'SP_NA_POST_') 'Falta procedimiento POST.'
    Assert-True ($sqlText -cmatch 'SP_NA_PUT_') 'Falta procedimiento PUT.'
    Assert-True ($sqlText -cmatch 'SP_NA_DELETE_') 'Falta procedimiento DELETE.'
    Assert-True ($sqlText -cmatch 'BUSCARPORCODIGO') 'Falta validación de código único.'
    Assert-True ($sqlText -cmatch 'HISTORIAL') 'Falta procedimiento de historial.'
    Assert-True ($sqlText -cmatch '(?i)WHERE\s+IsDeleted\s*=\s*0') 'Los listados no evidencian filtro de eliminación lógica.'
    Assert-True ($sqlText -notmatch '(?i)\bTRUNCATE\s+TABLE\b|\bDROP\s+TABLE\b|\bDELETE\s+FROM\b') 'El SQL scaffold contiene una operación destructiva prohibida.'
    Assert-True ($sqlText -notmatch '(?i)\b(password|pwd|connection string|access token|private key)\b') 'El SQL contiene términos sensibles prohibidos.'

    $batchValidator = Join-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) 'nuansystem-sql-standards\scripts\Test-SqlMigrationBatches.ps1'
    Assert-True (Test-Path -LiteralPath $batchValidator) 'No se encontró Test-SqlMigrationBatches.ps1.'
    foreach ($sqlFile in $sqlFiles) {
        & powershell.exe -NoProfile -File $batchValidator -Path $sqlFile.FullName | Out-Host
        Assert-True ($LASTEXITCODE -eq 0) "El SQL generado incumple el gate de lotes SQL Server: $($sqlFile.Name)."
    }

    if ($Manifest.synchronization.mode -eq 'none') {
        Assert-True ($sqlText -notmatch '(?i)SP_NA_.*SYNC_(FULL|APPLY)|LocalOutbox|SyncEntityDefinitions') 'El arquetipo sin sincronización generó contratos sync.'
    }
    else {
        foreach ($token in @('LocalOutbox','SYNC_FULL','SYNC_APPLY','SyncEntityDefinitions')) {
            Assert-True ($sqlText -cmatch [regex]::Escape($token)) "Falta contrato sync $token."
        }
        Assert-True ($sqlText -cmatch '(?is)INSERT\s+dbo\.SyncEntityConfigurations\s*\([^;]*\bIsEnabled\b[^;]*\)\s*SELECT\s+[^;]*,\s*0\s*,') 'La sincronización no evidencia disabled-by-default.'
    }
}

function Assert-DesignerGates {
    param([string] $OutputPath, [pscustomobject] $Manifest)
    $filesRoot = Join-Path $OutputPath 'files'
    $designerFiles = @(Get-ChildItem -LiteralPath $filesRoot -Recurse -File -Filter '*.Designer.cs')
    Assert-True ($designerFiles.Count -ge 1) 'Se esperaba un Designer explícito para el editor.'
    $csFiles = @(Get-ChildItem -LiteralPath $filesRoot -Recurse -File -Filter '*.cs')
    $text = ($csFiles | ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    Assert-True ($text -cmatch 'BaseGridCrudListForm') 'La lista no hereda BaseGridCrudListForm.'
    Assert-True ($text -cmatch 'BaseEditForm') 'El editor no hereda BaseEditForm.'
    Assert-True ($text -cmatch 'NuanActionButton') 'No se generaron acciones corporativas NuanActionButton.'
    Assert-True ($text -cmatch 'INuanApiClient') 'El cliente frontend no usa INuanApiClient.'
    Assert-True ($text -cmatch 'ICommandHandler') 'Faltan handlers de comandos en el scaffold.'
    Assert-True ($text -cmatch 'IQueryHandler') 'Faltan handlers de consultas en el scaffold.'
    Assert-True ($text -notmatch '(?i)new\s+HttpClient\s*\(|SqlConnection|HanaConnection|Company\.GetBusinessObject|SAPbobsCOM') 'El frontend generado contiene transporte o acceso externo directo.'

    foreach ($designer in $designerFiles) {
        $designerText = Get-Utf8Text $designer.FullName
        foreach ($token in @('partial','InitializeComponent','SuspendLayout','ResumeLayout','Dispose')) {
            Assert-True ($designerText -cmatch [regex]::Escape($token)) "$($designer.Name) no contiene $token."
        }
        Assert-True ($designerText -notmatch '(?m)^\s*var\s+|\bforeach\s*\(|\bfor\s*\(|\basync\b|Task\s*<|GetRequiredService|HttpClient') "$($designer.Name) contiene construcción no compatible con Designer."
    }

    Assert-True ($text.Contains($Manifest.api.formKey)) 'El FormKey no está conectado en frontend.'
    Assert-True ($text.Contains($Manifest.permissions.read)) 'El permiso READ no está conectado en frontend.'
    Assert-True ($text.Contains($Manifest.permissions.manage)) 'El permiso MANAGE no está conectado en frontend.'
}

function Assert-ArchetypeContract {
    param([string] $OutputPath, [pscustomobject] $Manifest)
    $text = (Get-ChildItem -LiteralPath (Join-Path $OutputPath 'files') -Recurse -File |
        ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    switch ($Manifest.archetype) {
        'basic' {
            Assert-True ($null -eq $Manifest.PSObject.Properties['classification']) 'El piloto basic no debe declarar classification.'
            Assert-True ($null -eq $Manifest.PSObject.Properties['dependency']) 'El piloto basic no debe declarar dependency.'
        }
        'classified' {
            Assert-True ($null -ne $Manifest.classification) 'El piloto classified requiere classification.'
            Assert-True ($text.Contains($Manifest.classification.field)) 'El scaffold classified no contiene el campo de clasificación.'
            foreach ($value in $Manifest.classification.allowedValues) {
                Assert-True ($text.Contains($value.code)) "Falta el valor cerrado de clasificación $($value.code)."
                Assert-True ($text.Contains($value.label)) "Falta la etiqueta de clasificación $($value.label)."
            }
        }
        'dependent' {
            Assert-True ($null -ne $Manifest.dependency) 'El piloto dependent requiere dependency.'
            foreach ($token in @($Manifest.dependency.field,$Manifest.dependency.parentIdField,$Manifest.dependency.parentGlobalIdField,$Manifest.dependency.lookupRoute,$Manifest.dependency.parentFormKey)) {
                Assert-True ($text.Contains([string]$token)) "El scaffold dependent no contiene $token."
            }
            Assert-True ($text -cmatch 'NuanLookupEdit') 'El maestro dependiente no usa NuanLookupEdit.'
            Assert-True ($text -cmatch 'GlobalId') 'El maestro dependiente no evidencia resolución sync por GlobalId.'
        }
    }
}

$resolvedGenerator = (Resolve-Path -LiteralPath $GeneratorPath -ErrorAction Stop).Path
$GeneratorPath = $resolvedGenerator
$hasSinglePackageInput = -not [string]::IsNullOrWhiteSpace($ManifestPath) -or -not [string]::IsNullOrWhiteSpace($GeneratedPath)
if ($hasSinglePackageInput) {
    Assert-True (-not [string]::IsNullOrWhiteSpace($ManifestPath) -and -not [string]::IsNullOrWhiteSpace($GeneratedPath)) 'Use -Manifest y -GeneratedPath juntos.'
    $singleManifestPath = (Resolve-Path -LiteralPath $ManifestPath -ErrorAction Stop).Path
    $singleGeneratedPath = (Resolve-Path -LiteralPath $GeneratedPath -ErrorAction Stop).Path
    $singleManifest = Get-Utf8Text $singleManifestPath | ConvertFrom-Json
    Assert-NoResidualTokens -Path $singleGeneratedPath
    Assert-NoSecrets -Path $singleGeneratedPath
    Assert-PathAndContract -OutputPath $singleGeneratedPath -Manifest $singleManifest
    Assert-SqlStaticGates -OutputPath $singleGeneratedPath -Manifest $singleManifest
    Assert-DesignerGates -OutputPath $singleGeneratedPath -Manifest $singleManifest
    Assert-ArchetypeContract -OutputPath $singleGeneratedPath -Manifest $singleManifest
    Write-Host "Validación aprobada para el paquete: $singleGeneratedPath"
    exit 0
}
$fixturesRoot = (Resolve-Path -LiteralPath $FixturesPath -ErrorAction Stop).Path
if (-not $RepositoryRoot) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..\..')).Path
}
else { $RepositoryRoot = (Resolve-Path -LiteralPath $RepositoryRoot -ErrorAction Stop).Path }

$requiredFixtures = @('basic-item-line.json','classified-product-type.json','dependent-item-family.json')
foreach ($fixture in $requiredFixtures) {
    Assert-True (Test-Path -LiteralPath (Join-Path $fixturesRoot $fixture)) "Falta fixture obligatorio: $fixture"
}

$workspace = Join-Path ([System.IO.Path]::GetTempPath()) ("nuan-auxiliary-master-tests-" + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $workspace | Out-Null

try {
    $sandboxRepository = Join-Path $workspace 'repository'
    New-Item -ItemType Directory -Path (Join-Path $sandboxRepository 'database\sql') -Force | Out-Null
    [System.IO.File]::WriteAllText((Join-Path $sandboxRepository 'nuansystem.sln'), "`n", [System.Text.UTF8Encoding]::new($false))
    [System.IO.File]::WriteAllText((Join-Path $sandboxRepository 'database\sql\200_existing.sql'), "-- migration marker`n", [System.Text.UTF8Encoding]::new($false))
    $repositoryBefore = Get-DirectorySnapshot -Path $sandboxRepository
    $results = [System.Collections.Generic.List[object]]::new()

    foreach ($fixtureName in $requiredFixtures) {
        $sourceFixture = Join-Path $fixturesRoot $fixtureName
        $manifest = Get-Utf8Text $sourceFixture | ConvertFrom-Json
        $pilotName = [System.IO.Path]::GetFileNameWithoutExtension($fixtureName)
        $fixture = Join-Path $workspace ($pilotName + '.pilot.json')
        $manifest.migrations.tenant = 201
        $manifest.migrations.masterNavigation = 202
        $manifest.migrations.masterSync = 203
        $pilotJson = $manifest | ConvertTo-Json -Depth 20
        [System.IO.File]::WriteAllText($fixture, $pilotJson, [System.Text.UTF8Encoding]::new($false))
        $outA = Join-Path $workspace ($pilotName + '-a')
        $outB = Join-Path $workspace ($pilotName + '-b')

        $validation = Invoke-Generator -Manifest $fixture -Mode Validate -Root $sandboxRepository
        $proposalResult = Invoke-Generator -Manifest $fixture -Mode Propose -Root $sandboxRepository
        $proposal = $proposalResult.Output | ConvertFrom-Json
        Assert-True (([string]$proposal.proposalHash).Trim().Equals(([string]$manifest.designApproval.proposalHash).Trim(), [StringComparison]::OrdinalIgnoreCase)) "La huella aprobada de $pilotName no coincide con Propose. Aprobada=$($manifest.designApproval.proposalHash); Actual=$($proposal.proposalHash)."
        Assert-True (@($proposal.columns).Count -gt @($manifest.fields).Count) "Propose no incluyó campos técnicos para $pilotName."
        Assert-True (-not [string]::IsNullOrWhiteSpace([string]$proposal.mockupBrief)) "Propose no emitió el brief full color para $pilotName."
        Invoke-Generator -Manifest $fixture -Mode Preview -Root $sandboxRepository | Out-Null
        Invoke-Generator -Manifest $fixture -Mode Diff -Root $sandboxRepository | Out-Null
        Invoke-Generator -Manifest $fixture -Mode Scaffold -OutputPath $outA -Root $sandboxRepository | Out-Null

        Assert-NoResidualTokens -Path $outA
        Assert-NoSecrets -Path $outA
        Assert-PathAndContract -OutputPath $outA -Manifest $manifest
        Assert-SqlStaticGates -OutputPath $outA -Manifest $manifest
        Assert-DesignerGates -OutputPath $outA -Manifest $manifest
        Assert-ArchetypeContract -OutputPath $outA -Manifest $manifest

        Invoke-Generator -Manifest $fixture -Mode Scaffold -OutputPath $outA -Root $sandboxRepository -ExpectFailure | Out-Null
        New-Item -ItemType Directory -Path $outB | Out-Null
        Set-Content -LiteralPath (Join-Path $outB 'sentinel.txt') -Value 'must-not-overwrite' -NoNewline
        Invoke-Generator -Manifest $fixture -Mode Scaffold -OutputPath $outB -Root $sandboxRepository -ExpectFailure | Out-Null
        Assert-True ((Get-Content -Raw -LiteralPath (Join-Path $outB 'sentinel.txt')) -ceq 'must-not-overwrite') 'El generador alteró un staging no vacío.'

        Remove-Item -LiteralPath $outB -Recurse -Force
        Invoke-Generator -Manifest $fixture -Mode Scaffold -OutputPath $outB -Root $sandboxRepository | Out-Null
        Assert-SnapshotsEqual -Expected (Get-DirectorySnapshot $outA) -Actual (Get-DirectorySnapshot $outB) -Message "La generación de $pilotName no es determinista."
        $results.Add([pscustomobject]@{ Pilot = $pilotName; Archetype = $manifest.archetype; Status = 'PASS' })
    }

    $invalidFixture = Join-Path $workspace 'invalid-path.json'
    $invalid = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $invalid.api.route = 'api/items'
    $invalid.api.formKey = '../escape'
    $invalid | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $invalidFixture -Encoding utf8
    Invoke-Generator -Manifest $invalidFixture -Mode Validate -Root $sandboxRepository -ExpectFailure | Out-Null

    $secretFixture = Join-Path $workspace 'invalid-secret.json'
    $secret = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $secret.entity.title = 'Password=DoNotGenerateThis'
    $secret | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $secretFixture -Encoding utf8
    Invoke-Generator -Manifest $secretFixture -Mode Validate -Root $sandboxRepository -ExpectFailure | Out-Null

    $pendingFixture = Join-Path $workspace 'pending-design-approval.json'
    $pending = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $pending.PSObject.Properties.Remove('designApproval')
    $pending | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $pendingFixture -Encoding utf8
    Invoke-Generator -Manifest $pendingFixture -Mode Validate -Root $sandboxRepository | Out-Null
    Invoke-Generator -Manifest $pendingFixture -Mode Propose -Root $sandboxRepository | Out-Null
    Invoke-Generator -Manifest $pendingFixture -Mode Preview -Root $sandboxRepository -ExpectFailure | Out-Null
    Invoke-Generator -Manifest $pendingFixture -Mode Scaffold -OutputPath (Join-Path $workspace 'pending-output') -Root $sandboxRepository -ExpectFailure | Out-Null

    $staleFixture = Join-Path $workspace 'stale-design-approval.json'
    $stale = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $stale.fields[1].stringLength = 151
    $stale | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $staleFixture -Encoding utf8
    Invoke-Generator -Manifest $staleFixture -Mode Propose -Root $sandboxRepository | Out-Null
    Invoke-Generator -Manifest $staleFixture -Mode Diff -Root $sandboxRepository -ExpectFailure | Out-Null

    $protectedOutput = Join-Path $sandboxRepository 'src\must-not-write'
    Invoke-Generator -Manifest (Join-Path $fixturesRoot 'basic-item-line.json') -Mode Scaffold -OutputPath $protectedOutput -Root $sandboxRepository -ExpectFailure | Out-Null
    Assert-True (-not (Test-Path -LiteralPath $protectedOutput)) 'El generador escribió dentro de src/ pese al gate de staging.'

    $repositoryAfter = Get-DirectorySnapshot -Path $sandboxRepository
    foreach ($key in @($repositoryBefore.Keys + $repositoryAfter.Keys | Sort-Object -Unique)) {
        Assert-True ($repositoryBefore.ContainsKey($key) -and $repositoryAfter.ContainsKey($key) -and $repositoryBefore[$key] -ceq $repositoryAfter[$key]) "El generador modificó el repositorio de prueba: $key"
    }

    $results | Format-Table Pilot,Archetype,Status -AutoSize | Out-String | Write-Host
    Write-Host 'Validación aprobada: propuesta de tabla, mockup full color, aprobación vigente, artefactos, tokens, staging, rutas/FormKey, SQL, Designer, secretos, determinismo y tres pilotos.'
    exit 0
}
catch {
    Write-Error $_
    exit 1
}
finally {
    if (-not $KeepArtifacts -and (Test-Path -LiteralPath $workspace)) {
        Remove-Item -LiteralPath $workspace -Recurse -Force
    }
    elseif (Test-Path -LiteralPath $workspace) { Write-Host "Artefactos conservados en $workspace" }
}
