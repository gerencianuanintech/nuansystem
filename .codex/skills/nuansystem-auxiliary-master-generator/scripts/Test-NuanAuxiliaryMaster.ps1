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
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$pwshCommand = Get-Command pwsh.exe -ErrorAction SilentlyContinue | Select-Object -First 1
$PowerShellExecutable = if ($null -ne $pwshCommand) { $pwshCommand.Source } else { 'powershell.exe' }

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

function Get-OptionalProperty {
    param([object] $Object, [string] $Name, $Default)
    if ($null -eq $Object) { return $Default }
    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property) { return $Default }
    return $property.Value
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
        $output = & $PowerShellExecutable @arguments 2>&1 | Out-String
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
    $expectedRoute = $Manifest.api.route
    Assert-True ($normalized.api.route -ceq $expectedRoute) 'La ruta normalizada no coincide con el manifiesto.'
    Assert-True ($normalized.api.formKey -ceq $Manifest.api.formKey) 'El FormKey normalizado no coincide con el manifiesto.'
    Assert-True ($normalized.navigation.menuCode -ceq $Manifest.navigation.menuCode) 'El código de menú normalizado no coincide.'

    Assert-True ($normalized.api.route -cmatch '^/api/[a-z0-9]+(?:-[a-z0-9]+)*(?:/[a-z0-9]+(?:-[a-z0-9]+)*)*$') 'La ruta API no es una ruta absoluta /api en kebab-case.'
    Assert-True ($normalized.api.formKey -cmatch '^[a-z0-9]+(?:-[a-z0-9]+)*$') 'El FormKey no es kebab-case.'
    Assert-True (($normalized.api.route -split '/')[-1] -ceq $normalized.api.formKey) 'El FormKey no coincide con el último segmento de la ruta API.'
    Assert-True ($normalized.entity.singular -cmatch '^[A-Z][A-Za-z0-9]*$') 'entity.singular no es PascalCase.'
    Assert-True ($normalized.entity.plural -cmatch '^[A-Z][A-Za-z0-9]*$') 'entity.plural no es PascalCase.'
    Assert-True ($normalized.entity.table -cmatch '^dbo\.[A-Z][A-Za-z0-9]*$') 'entity.table no cumple dbo.<PascalCase>.'
    Assert-True (($normalized.navigation.path -join '/') -ceq 'Configuration/Definitions/Inventory') 'La navegación no apunta a Configuration/Definitions/Inventory.'

    $expectedFeaturePath = if ($null -ne $Manifest.PSObject.Properties['placement']) {
        @($Manifest.placement.featurePath)
    }
    else {
        @('Definitions', 'Inventory', [string]$Manifest.entity.plural)
    }
    Assert-True (($normalized.placement.featurePath -join '/') -ceq ($expectedFeaturePath -join '/')) 'La ubicación física normalizada no coincide con el manifiesto.'
    $featureRelativePath = $expectedFeaturePath -join '/'
    Assert-True (Test-Path -LiteralPath (Join-Path $OutputPath "files/src/Backend/NuanSystem.Application/Features/$featureRelativePath")) 'La ubicación física de Application no coincide con placement.featurePath.'
    Assert-True (Test-Path -LiteralPath (Join-Path $OutputPath "files/src/Backend/NuanSystem.Api/Endpoints/$featureRelativePath")) 'La ubicación física de API no coincide con placement.featurePath.'
    Assert-True (Test-Path -LiteralPath (Join-Path $OutputPath "files/src/Frontend/NuanSystem.WinForms.Forms/$featureRelativePath")) 'La ubicación física de WinForms no coincide con placement.featurePath.'

    $filesRoot = Join-Path $OutputPath 'files'
    $allFiles = Get-ChildItem -LiteralPath $filesRoot -Recurse -File
    Assert-True ($allFiles.Count -gt 0) 'El scaffold no generó archivos en files/.'
    $allText = ($allFiles | ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    Assert-True ($allText.Contains($expectedRoute)) 'La ruta API no aparece en los artefactos generados.'
    Assert-True ($allText.Contains($Manifest.api.formKey)) 'El FormKey no aparece en los artefactos generados.'
    Assert-True ($allText.Contains($Manifest.permissions.read)) 'El permiso READ no aparece en los artefactos generados.'
    Assert-True ($allText.Contains($Manifest.permissions.manage)) 'El permiso MANAGE no aparece en los artefactos generados.'

    $contractTests = @(Get-ChildItem -LiteralPath $filesRoot -Recurse -File -Filter '*GeneratedContractTests.cs')
    Assert-True ($contractTests.Count -eq 1) 'Se esperaba exactamente una prueba contractual generada.'
    $contractText = Get-Utf8Text $contractTests[0].FullName
    Assert-True ($contractText.Contains('"NuanSystem.WinForms", "Program.cs"')) 'La prueba contractual no valida el registro en Program.cs.'
    Assert-True ($contractText.Contains('"NuanSystem.WinForms.Forms", "Shell", "MainForm.cs"')) 'La prueba contractual no valida el registro en MainForm.CreateModuleForm.'
    Assert-True ($contractText.Contains('"NuanSystem.WinForms.ViewModels", "Shell", "ShellViewModel.cs"')) 'La prueba contractual no valida el registro en ShellViewModel.'
    Assert-True ($contractText.Contains('generalInventoryCatalogFormFactory(module.Key)')) 'La prueba contractual no exige la segunda etapa de navegación de MainForm.'
    Assert-True ($contractText.Contains('foreach (var field in PersistedFields)')) 'La prueba contractual no protege todas las columnas persistidas del maestro.'
    Assert-True ($contractText.Contains('NavigationSql_RegistersApplicableOperationsSeparatelyFromRoleGrants')) 'La prueba contractual no separa operaciones aplicables y concesiones del rol.'
    Assert-True ($contractText.Contains('dbo.SecurityFormOperations')) 'La prueba contractual no protege las operaciones aplicables del formulario.'
    Assert-True ($contractText.Contains('dbo.SecurityRoleFormOperations')) 'La prueba contractual no protege las concesiones por rol.'
    Assert-True ($contractText.Contains('foreach (var operation in CanonicalOperations)')) 'La prueba contractual no protege las doce operaciones canónicas.'
    Assert-True ($contractText.Contains('GeneratedContracts_PreserveSqlResultsAndDesignerSafety')) 'La prueba contractual no protege resultados SQL ni seguridad del Designer.'
    Assert-True ($contractText.Contains('DECLARE @Affected int=@@ROWCOUNT;')) 'La prueba contractual no protege el conteo de eliminación antes de auditar.'
    Assert-True ($contractText.Contains('if (session is null) return;')) 'La prueba contractual no protege el constructor sin sesión usado por Visual Studio Designer.'
}

function Assert-SqlStaticGates {
    param([string] $OutputPath, [pscustomobject] $Manifest)
    $sqlFiles = @(Get-ChildItem -LiteralPath (Join-Path $OutputPath 'files') -Recurse -File -Filter '*.sql')
    Assert-True ($sqlFiles.Count -ge 2) 'Se esperaban al menos migraciones tenant y navegación Master.'
    $sqlText = ($sqlFiles | ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    $versionDate = [string](Get-OptionalProperty $Manifest.migrations 'versionDate' '')
    if ([string]::IsNullOrWhiteSpace($versionDate) -and [string]$Manifest.schemaVersion -in @('1.1','1.2')) {
        $versionDate = '20260813'
    }
    Assert-True ($versionDate -match '^20[0-9]{6}$') 'El manifiesto generado no conserva migrations.versionDate en formato yyyyMMdd.'
    Assert-True ($sqlText.Contains("$versionDate.$('{0:D3}' -f [int]$Manifest.migrations.tenant)")) 'La migración tenant no usa la fecha de versión aprobada.'
    Assert-True ($sqlText.Contains("$versionDate.$('{0:D3}' -f [int]$Manifest.migrations.masterNavigation)")) 'La migración Master no usa la fecha de versión aprobada.'
    foreach ($token in @('GlobalId','CreatedAt','UpdatedAt','IsDeleted','DeletedAt','SYSUTCDATETIME','CREATE OR ALTER PROCEDURE','AuditCatalogChanges')) {
        Assert-True ($sqlText -cmatch [regex]::Escape($token)) "El SQL generado no contiene el contrato obligatorio $token."
    }
    foreach ($auditColumn in @('CreatedByUserId','CreatedByUserName','UpdatedByUserId','UpdatedByUserName','DeletedByUserId','DeletedByUserName')) {
        Assert-True ($sqlText -cmatch "\[$auditColumn\]\s+(?:int|nvarchar\(120\))\s+NULL") "La tabla generada no declara la columna de auditoría $auditColumn."
    }
    $tableName = ([string]$Manifest.entity.table -split '\.')[-1]
    Assert-True ($sqlText.Contains("UQ_${tableName}_GlobalId")) 'Falta el índice único global, incluidos tombstones.'
    foreach ($field in @($Manifest.fields | Where-Object { [bool]$_.unique })) {
        Assert-True ($sqlText.Contains("UQ_${tableName}_$($field.name)")) "Falta el índice único para $($field.name), incluidos tombstones."
    }
    foreach ($field in @($Manifest.fields | Where-Object { $_.type -eq 'string' -and [bool]$_.required })) {
        Assert-True ($sqlText.Contains("CK_${tableName}_$($field.name)_NotBlank")) "Falta el CHECK no vacío para $($field.name)."
    }
    foreach ($field in @($Manifest.fields | Where-Object { $null -ne $_.PSObject.Properties['minimum'] })) {
        Assert-True ($sqlText.Contains("CK_${tableName}_$($field.name)")) "Falta el CHECK de mínimo para $($field.name)."
        if ($null -ne $field.PSObject.Properties['default']) {
            Assert-True ($sqlText -cmatch "DF_${tableName}_$($field.name)\s+DEFAULT\($([regex]::Escape([string]$field.default))\)") "Falta el default declarado para $($field.name)."
        }
    }
    Assert-True ($sqlText -cmatch 'SP_NA_GET_') 'Falta procedimiento GET.'
    Assert-True ($sqlText -cmatch 'SP_NA_POST_') 'Falta procedimiento POST.'
    Assert-True ($sqlText -cmatch 'SP_NA_PUT_') 'Falta procedimiento PUT.'
    Assert-True ($sqlText -cmatch 'SP_NA_DELETE_') 'Falta procedimiento DELETE.'
    Assert-True ($sqlText -cmatch 'BUSCARPORCODIGO') 'Falta validación de código único.'
    Assert-True ($sqlText -cmatch 'HISTORIAL') 'Falta procedimiento de historial.'
    Assert-True ($sqlText -cmatch "OBJECT_ID\(N'dbo\.AuditCatalogChanges',\s*N'U'\)\s+IS\s+NULL") 'La migración tenant no valida AuditCatalogChanges antes de crear procedimientos de auditoría.'
    Assert-True ($sqlText -cmatch 'DECLARE\s+@Affected\s+int\s*=\s*@@ROWCOUNT') 'DELETE no conserva el número de filas afectadas antes de insertar auditoría.'
    Assert-True ($sqlText -cmatch 'SELECT\s+@Affected\s*;') 'DELETE no devuelve el número de filas realmente afectadas.'
    Assert-True ($sqlText -cmatch '(?is)SP_NA_DELETE_.*?DECLARE\s+@OwnTransaction.*?BEGIN\s+TRANSACTION.*?INSERT\s+dbo\.AuditCatalogChanges.*?COMMIT') 'DELETE y su auditoría no comparten una transacción owned-or-ambient.'
    Assert-True ($sqlText -cmatch 'dbo\.SecurityFormOperations') 'La navegación Master no registra operaciones aplicables al formulario.'
    Assert-True ($sqlText -cmatch 'dbo\.SecurityRoleFormOperations') 'La navegación Master no registra por separado las concesiones del rol.'
    Assert-True ($sqlText -cmatch "OBJECT_ID\(N'dbo\.MasterSchemaHistory',N'U'\)\s+IS\s+NULL") 'La navegación Master no valida MasterSchemaHistory.'
    Assert-True ($sqlText -cmatch '(?is)UPDATE\s+dbo\.SecurityMenus\s+SET.*?IsDeleted\s*=\s*0') 'La navegación Master no reactiva el menú existente.'
    Assert-True ($sqlText -cmatch '(?is)UPDATE\s+dbo\.SecurityRoleMenus\s+SET\s+IsAllowed\s*=\s*1\s*,\s*IsDeleted\s*=\s*0') 'La navegación Master no reactiva el acceso de menú del rol aprobado.'
    foreach ($operationCode in @(
        'ACTION.REFRESH','ACTION.CONSULT','ACTION.CREATE','ACTION.UPDATE',
        'ACTION.DELETE','ACTION.COPY','ACTION.HISTORY','ACTION.CUSTOMIZE_COLUMNS',
        'ACTION.EXPORT_EXCEL','ACTION.EXPORT_PDF','ACTION.EXPORT_JSON','ACTION.EXPORT_XML')) {
        Assert-True ($sqlText -cmatch [regex]::Escape($operationCode)) "La navegación Master no incluye la operación canónica $operationCode."
    }
    Assert-True ($sqlText -cmatch 'HasListView\s*,\s*HasEditView') 'El formulario no declara sus vistas de listado y edición.'
    Assert-True ($sqlText -cmatch '(?i)WHERE\s+IsDeleted\s*=\s*0') 'Los listados no evidencian filtro de eliminación lógica.'
    Assert-True ($sqlText -notmatch '(?i)\bTRUNCATE\s+TABLE\b|\bDROP\s+TABLE\b|\bDELETE\s+FROM\b') 'El SQL scaffold contiene una operación destructiva prohibida.'
    Assert-True ($sqlText -notmatch '(?i)\b(password|pwd|connection string|access token|private key)\b') 'El SQL contiene términos sensibles prohibidos.'

    $batchValidator = Join-Path (Split-Path (Split-Path $PSScriptRoot -Parent) -Parent) 'nuansystem-sql-standards\scripts\Test-SqlMigrationBatches.ps1'
    Assert-True (Test-Path -LiteralPath $batchValidator) 'No se encontró Test-SqlMigrationBatches.ps1.'
    foreach ($sqlFile in $sqlFiles) {
        & $PowerShellExecutable -NoProfile -File $batchValidator -Path $sqlFile.FullName | Out-Host
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
    $resxFiles = @(Get-ChildItem -LiteralPath $filesRoot -Recurse -File -Filter '*.resx')
    Assert-True ($resxFiles.Count -eq 2) 'Se esperaban recursos .resx para el listado y el editor.'
    $csFiles = @(Get-ChildItem -LiteralPath $filesRoot -Recurse -File -Filter '*.cs')
    $text = ($csFiles | ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    $listFiles = @($csFiles | Where-Object { (Get-Utf8Text $_.FullName) -cmatch 'BaseGridCrudListForm' })
    Assert-True ($listFiles.Count -eq 1) 'Se esperaba exactamente un formulario de listado CRUD.'
    $listText = Get-Utf8Text $listFiles[0].FullName
    Assert-True ($text -cmatch 'BaseGridCrudListForm') 'La lista no hereda BaseGridCrudListForm.'
    Assert-True ($listText -cmatch 'GridView\.Columns\.AddField\(field\)') 'La lista no declara columnas cuando el origen de datos está vacío.'
    Assert-True ($listText -cmatch 'if\s*\(session\s+is\s+null\)\s+return\s*;') 'El constructor de Visual Studio Designer puede evaluar permisos con ApiSession nula.'
    foreach ($field in @($Manifest.fields)) {
        Assert-True ($listText -cmatch "Item\.$([regex]::Escape([string]$field.name))\)") "La personalización del listado no ofrece la columna persistida $($field.name)."
    }
    foreach ($auditField in @('CreatedByUserId','CreatedByUserName','CreatedAt','UpdatedByUserId','UpdatedByUserName','UpdatedAt','IsDeleted','DeletedByUserId','DeletedByUserName','DeletedAt')) {
        Assert-True ($listText -cmatch "HiddenColumn\(nameof\([^)]*Item\.$auditField\)") "La personalización del listado no ofrece la columna $auditField."
    }
    if (@($Manifest.fields | Where-Object { $_.role -eq 'description' }).Count -gt 0) {
        Assert-True ($text -cmatch 'Item\.Description\),\s*"Descripción"') 'La lista no muestra el campo Descripción.'
    }
    Assert-True ($text -cmatch 'BaseEditForm') 'El editor no hereda BaseEditForm.'
    Assert-True ($text -cmatch 'CancelButtonLocation' -and $text -cmatch 'SaveButtonLocation') 'El editor no posiciona las acciones heredadas Cancelar/Guardar.'
    Assert-True ($text -cnotmatch 'btnGeneratedPrimary') 'El editor generó una acción primaria local duplicada.'
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

    $editDesigners = @($designerFiles | Where-Object Name -Like '*EditForm.Designer.cs')
    Assert-True ($editDesigners.Count -eq 1) 'Se esperaba exactamente un Designer del editor para validar su geometría.'
    $layoutValidator = Join-Path $PSScriptRoot 'Test-NuanAuxiliaryMasterDesignerSpacing.ps1'
    Assert-True (Test-Path -LiteralPath $layoutValidator) 'No se encontró Test-NuanAuxiliaryMasterDesignerSpacing.ps1.'
    $normalizedManifest = Join-Path $OutputPath 'manifest.normalized.json'
    & $PowerShellExecutable -NoProfile -File $layoutValidator -Manifest $normalizedManifest -DesignerPath $editDesigners[0].FullName | Out-Host
    Assert-True ($LASTEXITCODE -eq 0) "$($editDesigners[0].Name) incumple la separación vertical corporativa."

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
    [System.IO.File]::WriteAllText((Join-Path $sandboxRepository 'database\sql\000_existing.sql'), "-- migration marker`n", [System.Text.UTF8Encoding]::new($false))
    $repositoryBefore = Get-DirectorySnapshot -Path $sandboxRepository
    $results = [System.Collections.Generic.List[object]]::new()

    foreach ($fixtureName in $requiredFixtures) {
        $sourceFixture = Join-Path $fixturesRoot $fixtureName
        $manifest = Get-Utf8Text $sourceFixture | ConvertFrom-Json
        $pilotName = [System.IO.Path]::GetFileNameWithoutExtension($fixtureName)
        $fixture = Join-Path $workspace ($pilotName + '.pilot.json')
        $pilotJson = $manifest | ConvertTo-Json -Depth 20
        [System.IO.File]::WriteAllText($fixture, $pilotJson, [System.Text.UTF8Encoding]::new($false))
        $outA = Join-Path $workspace ($pilotName + '-a')
        $outB = Join-Path $workspace ($pilotName + '-b')

        $validation = Invoke-Generator -Manifest $fixture -Mode Validate -Root $sandboxRepository
        $proposalResult = Invoke-Generator -Manifest $fixture -Mode Propose -Root $sandboxRepository
        $proposal = $proposalResult.Output | ConvertFrom-Json
        Assert-True (([string]$proposal.proposalHash).Trim().Equals(([string]$manifest.designApproval.proposalHash).Trim(), [StringComparison]::OrdinalIgnoreCase)) "La huella aprobada de $pilotName no coincide con Propose. Aprobada=$($manifest.designApproval.proposalHash); Actual=$($proposal.proposalHash)."
        Assert-True (@($proposal.columns).Count -gt @($manifest.fields).Count) "Propose no incluyó campos técnicos para $pilotName."
        foreach ($constraintPattern in @('CHECK: Code no*Trim','CHECK: Name no*Trim','CHECK: SortOrder >= 0')) {
            Assert-True (@($proposal.constraints | Where-Object { $_ -like $constraintPattern }).Count -eq 1) "Propose no incluyó el patrón '$constraintPattern' para $pilotName."
        }
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

        if ($pilotName -eq 'basic-item-line') {
            $designerPath = (Get-ChildItem -LiteralPath (Join-Path $outA 'files') -Recurse -File -Filter '*EditForm.Designer.cs').FullName
            $originalDesigner = Get-Utf8Text $designerPath
            $invalidDesigner = $originalDesigner.Replace('txtName.Location = new Point(154, 54);', 'txtName.Location = new Point(154, 57);')
            Assert-True ($invalidDesigner -cne $originalDesigner) 'No se pudo preparar el fixture negativo de separación vertical.'
            [System.IO.File]::WriteAllText($designerPath, $invalidDesigner, [System.Text.UTF8Encoding]::new($false))
            $layoutValidator = Join-Path $PSScriptRoot 'Test-NuanAuxiliaryMasterDesignerSpacing.ps1'
            $oldPreference = $ErrorActionPreference
            $ErrorActionPreference = 'Continue'
            try {
                & $PowerShellExecutable -NoProfile -File $layoutValidator -Manifest (Join-Path $outA 'manifest.normalized.json') -DesignerPath $designerPath *> $null
                $invalidLayoutExitCode = $LASTEXITCODE
            }
            finally { $ErrorActionPreference = $oldPreference }
            Assert-True ($invalidLayoutExitCode -ne 0) 'El validador aceptó una fila desplazada 3 px fuera de la cadencia corporativa.'
            [System.IO.File]::WriteAllText($designerPath, $originalDesigner, [System.Text.UTF8Encoding]::new($false))
            Assert-DesignerGates -OutputPath $outA -Manifest $manifest

            $listPath = (Get-ChildItem -LiteralPath (Join-Path $outA 'files') -Recurse -File -Filter '*Form.cs' |
                Where-Object { (Get-Utf8Text $_.FullName) -cmatch 'BaseGridCrudListForm' }).FullName
            $originalList = Get-Utf8Text $listPath
            $invalidList = $originalList.Replace('        if (session is null) return;', '')
            Assert-True ($invalidList -cne $originalList) 'No se pudo preparar el fixture negativo de sesión nula del Designer.'
            [System.IO.File]::WriteAllText($listPath, $invalidList, [System.Text.UTF8Encoding]::new($false))
            $designerSafetyRejected = $false
            try { Assert-DesignerGates -OutputPath $outA -Manifest $manifest }
            catch { $designerSafetyRejected = $true }
            Assert-True $designerSafetyRejected 'El validador aceptó un constructor de listado que evalúa permisos con ApiSession nula.'
            [System.IO.File]::WriteAllText($listPath, $originalList, [System.Text.UTF8Encoding]::new($false))

            $tenantSqlPath = (Get-ChildItem -LiteralPath (Join-Path $outA 'files/database/sql') -File -Filter '*_tenant_*').FullName
            $originalTenantSql = Get-Utf8Text $tenantSqlPath
            $invalidTenantSql = $originalTenantSql.Replace('  DECLARE @Affected int=@@ROWCOUNT;', '  -- affected row count intentionally removed by negative fixture')
            Assert-True ($invalidTenantSql -cne $originalTenantSql) 'No se pudo preparar el fixture negativo del conteo DELETE.'
            [System.IO.File]::WriteAllText($tenantSqlPath, $invalidTenantSql, [System.Text.UTF8Encoding]::new($false))
            $deleteContractRejected = $false
            try { Assert-SqlStaticGates -OutputPath $outA -Manifest $manifest }
            catch { $deleteContractRejected = $true }
            Assert-True $deleteContractRejected 'El validador aceptó un DELETE que pierde @@ROWCOUNT después de auditar.'
            [System.IO.File]::WriteAllText($tenantSqlPath, $originalTenantSql, [System.Text.UTF8Encoding]::new($false))

            $navigationSqlPath = (Get-ChildItem -LiteralPath (Join-Path $outA 'files/database/sql') -File -Filter '*_master_*_navigation.sql').FullName
            $originalNavigationSql = Get-Utf8Text $navigationSqlPath
            $invalidNavigationSql = $originalNavigationSql.Replace(' UPDATE dbo.SecurityRoleMenus SET IsAllowed=1,IsDeleted=0,DeletedByUserId=NULL,DeletedByUserName=NULL,DeletedAt=NULL,', ' -- role-menu reactivation intentionally removed by negative fixture')
            Assert-True ($invalidNavigationSql -cne $originalNavigationSql) 'No se pudo preparar el fixture negativo de reactivación del menú.'
            [System.IO.File]::WriteAllText($navigationSqlPath, $invalidNavigationSql, [System.Text.UTF8Encoding]::new($false))
            $navigationRecoveryRejected = $false
            try { Assert-SqlStaticGates -OutputPath $outA -Manifest $manifest }
            catch { $navigationRecoveryRejected = $true }
            Assert-True $navigationRecoveryRejected 'El validador aceptó navegación sin reactivar el acceso de menú eliminado lógicamente.'
            [System.IO.File]::WriteAllText($navigationSqlPath, $originalNavigationSql, [System.Text.UTF8Encoding]::new($false))

            Assert-SqlStaticGates -OutputPath $outA -Manifest $manifest
            Assert-DesignerGates -OutputPath $outA -Manifest $manifest
        }

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

    $customPathFixture = Join-Path $workspace 'custom-independent-paths.json'
    $customPathManifest = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $customPathManifest.placement.featurePath = @('Commercial', 'Catalogs', 'ItemLines')
    $customPathManifest.api.route = '/api/catalog/item-lines'
    $customPathManifest.migrations.tenant = 204
    $customPathManifest.migrations.masterNavigation = 205
    $customPathManifest.migrations.masterSync = $null
    [System.IO.File]::WriteAllText($customPathFixture, ($customPathManifest | ConvertTo-Json -Depth 20), [System.Text.UTF8Encoding]::new($false))
    Invoke-Generator -Manifest $customPathFixture -Mode Preview -Root $sandboxRepository -ExpectFailure | Out-Null
    $customProposal = (Invoke-Generator -Manifest $customPathFixture -Mode Propose -Root $sandboxRepository).Output | ConvertFrom-Json
    $customPathManifest.designApproval.proposalHash = $customProposal.proposalHash
    [System.IO.File]::WriteAllText($customPathFixture, ($customPathManifest | ConvertTo-Json -Depth 20), [System.Text.UTF8Encoding]::new($false))
    $customPathOutput = Join-Path $workspace 'custom-independent-paths-output'
    Invoke-Generator -Manifest $customPathFixture -Mode Validate -Root $sandboxRepository | Out-Null
    Invoke-Generator -Manifest $customPathFixture -Mode Scaffold -OutputPath $customPathOutput -Root $sandboxRepository | Out-Null
    Assert-PathAndContract -OutputPath $customPathOutput -Manifest $customPathManifest
    $customGeneratedText = (Get-ChildItem -LiteralPath (Join-Path $customPathOutput 'files') -Recurse -File -Filter '*.cs' | ForEach-Object { Get-Utf8Text $_.FullName }) -join "`n"
    Assert-True ($customGeneratedText.Contains('NuanSystem.Application.Features.Commercial.Catalogs.ItemLines')) 'Los namespaces no respetaron placement.featurePath.'
    Assert-True ($customGeneratedText.Contains('/api/catalog/item-lines')) 'La ruta HTTP personalizada no se generó de forma independiente.'
    $results.Add([pscustomobject]@{ Pilot = 'custom-independent-paths'; Archetype = $customPathManifest.archetype; Status = 'PASS' })

    $legacyFixture = Join-Path $workspace 'legacy-1.1.json'
    $legacyManifest = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $legacyManifest.schemaVersion = '1.1'
    $legacyManifest.PSObject.Properties.Remove('placement')
    $legacyManifest.migrations.tenant = 207
    $legacyManifest.migrations.masterNavigation = 208
    $legacyManifest.migrations.masterSync = $null
    [System.IO.File]::WriteAllText($legacyFixture, ($legacyManifest | ConvertTo-Json -Depth 20), [System.Text.UTF8Encoding]::new($false))
    Invoke-Generator -Manifest $legacyFixture -Mode Preview -Root $sandboxRepository -ExpectFailure | Out-Null
    $legacyProposal = (Invoke-Generator -Manifest $legacyFixture -Mode Propose -Root $sandboxRepository).Output | ConvertFrom-Json
    $legacyManifest.designApproval.proposalHash = $legacyProposal.proposalHash
    [System.IO.File]::WriteAllText($legacyFixture, ($legacyManifest | ConvertTo-Json -Depth 20), [System.Text.UTF8Encoding]::new($false))
    $legacyOutput = Join-Path $workspace 'legacy-1.1-output'
    Invoke-Generator -Manifest $legacyFixture -Mode Scaffold -OutputPath $legacyOutput -Root $sandboxRepository | Out-Null
    Assert-PathAndContract -OutputPath $legacyOutput -Manifest $legacyManifest
    Assert-True (Test-Path -LiteralPath (Join-Path $legacyOutput 'files/src/Backend/NuanSystem.Persistence/Repositories/Definitions/Inventory/ItemLineRepository.cs')) 'La compatibilidad 1.1 cambió la ubicación histórica del repositorio.'
    $results.Add([pscustomobject]@{ Pilot = 'legacy-1.1'; Archetype = $legacyManifest.archetype; Status = 'PASS' })

    $invalidFixture = Join-Path $workspace 'invalid-path.json'
    $invalid = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $invalid.api.route = 'api/items'
    $invalid.api.formKey = '../escape'
    $invalid | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $invalidFixture -Encoding utf8
    Invoke-Generator -Manifest $invalidFixture -Mode Validate -Root $sandboxRepository -ExpectFailure | Out-Null

    $unsafeSyncFixture = Join-Path $workspace 'unsafe-sync.json'
    $unsafeSync = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $unsafeSync.synchronization.mode = 'full-source-local-outbox'
    $unsafeSync.synchronization.executionOrder = 1
    $unsafeSync.migrations.masterSync = 210
    $unsafeSync | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $unsafeSyncFixture -Encoding utf8
    Invoke-Generator -Manifest $unsafeSyncFixture -Mode Validate -Root $sandboxRepository -ExpectFailure | Out-Null

    $invalidPlacementFixture = Join-Path $workspace 'invalid-physical-path.json'
    $invalidPlacement = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $invalidPlacement.placement.featurePath = @('Definitions', '..', 'ItemLines')
    $invalidPlacement | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $invalidPlacementFixture -Encoding utf8
    Invoke-Generator -Manifest $invalidPlacementFixture -Mode Validate -Root $sandboxRepository -ExpectFailure | Out-Null

    $invalidVersionDateFixture = Join-Path $workspace 'invalid-migration-version-date.json'
    $invalidVersionDate = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $invalidVersionDate.migrations.versionDate = '15-08-2026'
    $invalidVersionDate | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $invalidVersionDateFixture -Encoding utf8
    Invoke-Generator -Manifest $invalidVersionDateFixture -Mode Validate -Root $sandboxRepository -ExpectFailure | Out-Null

    $missingVersionDateFixture = Join-Path $workspace 'missing-migration-version-date.json'
    $missingVersionDate = Get-Utf8Text (Join-Path $fixturesRoot 'basic-item-line.json') | ConvertFrom-Json
    $missingVersionDate.migrations.PSObject.Properties.Remove('versionDate')
    $missingVersionDate | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath $missingVersionDateFixture -Encoding utf8
    Invoke-Generator -Manifest $missingVersionDateFixture -Mode Validate -Root $sandboxRepository -ExpectFailure | Out-Null

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
    Write-Host 'Validación aprobada: esquema 1.3, fecha SQL explícita, propuesta de tabla, mockup full color, aprobación vigente, artefactos, tokens, staging, rutas física/API independientes, FormKey, SQL idempotente, Designer seguro, separación vertical medida, pruebas negativas, secretos, determinismo, tres arquetipos, ruta personalizada y compatibilidad 1.1.'
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
