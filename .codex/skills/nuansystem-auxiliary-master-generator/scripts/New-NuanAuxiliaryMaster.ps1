[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Manifest,

    [ValidateSet('Validate', 'Propose', 'Preview', 'Diff', 'Scaffold')]
    [string]$Mode = 'Preview',

    [string]$RepositoryRoot,

    [string]$OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$Utf8Strict = [System.Text.UTF8Encoding]::new($false, $true)

function Stop-Generation([string]$Message) {
    throw "Nuan auxiliary master generator: $Message"
}

function Get-RepositoryRoot([string]$ExplicitRoot) {
    if ($ExplicitRoot) {
        $resolved = (Resolve-Path -LiteralPath $ExplicitRoot).Path
        if (-not (Test-Path -LiteralPath (Join-Path $resolved 'nuansystem.sln'))) {
            Stop-Generation "RepositoryRoot no contiene nuansystem.sln: $resolved"
        }
        return $resolved
    }

    $cursor = [System.IO.DirectoryInfo](Split-Path -Parent $PSScriptRoot)
    while ($null -ne $cursor) {
        if (Test-Path -LiteralPath (Join-Path $cursor.FullName 'nuansystem.sln')) {
            return $cursor.FullName
        }
        $cursor = $cursor.Parent
    }
    Stop-Generation 'No se pudo localizar la raíz de NuanSystem.'
}

function Get-RequiredProperty($Object, [string]$Name, [string]$Context) {
    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property -or $null -eq $property.Value) {
        Stop-Generation "Falta '$Context.$Name'."
    }
    if ($property.Value -is [string] -and [string]::IsNullOrWhiteSpace($property.Value)) {
        Stop-Generation "'$Context.$Name' no puede estar vacío."
    }
    return $property.Value
}

function Get-OptionalProperty($Object, [string]$Name, $Default = $null) {
    if ($null -eq $Object) { return $Default }
    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property -or $null -eq $property.Value) { return $Default }
    return $property.Value
}

function Assert-Matches([string]$Value, [string]$Pattern, [string]$Name) {
    if ($Value -notmatch $Pattern) {
        Stop-Generation "'$Name' tiene un formato inválido: '$Value'."
    }
}

function Convert-ToKebab([string]$Value) {
    $first = [regex]::Replace($Value, '([a-z0-9])([A-Z])', '$1-$2')
    return ($first -replace '[^A-Za-z0-9]+', '-').Trim('-').ToLowerInvariant()
}

function Convert-ToSnake([string]$Value) {
    return (Convert-ToKebab $Value).Replace('-', '_')
}

function Convert-ToSqlName([string]$Table) {
    return ($Table -replace '^dbo\.', '')
}

function Get-CSharpType($Field) {
    $type = ([string](Get-RequiredProperty $Field 'type' "fields[$($Field.name)]")).ToLowerInvariant()
    $nullable = [bool](Get-OptionalProperty $Field 'nullable' $false)
    $resolved = switch ($type) {
        'string'  { 'string' }
        'int'     { 'int' }
        'long'    { 'long' }
        'bool'    { 'bool' }
        'guid'    { 'Guid' }
        'decimal' { 'decimal' }
        'date'    { 'DateTime' }
        'datetime'{ 'DateTime' }
        default   { Stop-Generation "Tipo de campo no soportado en el MVP: '$type'." }
    }
    if ($nullable) { return "${resolved}?" }
    return $resolved
}

function Get-CSharpDefault($Field) {
    $type = ([string]$Field.type).ToLowerInvariant()
    $nullable = [bool](Get-OptionalProperty $Field 'nullable' $false)
    if ($nullable) { return '' }
    if ($type -eq 'string') { return ' = string.Empty;' }
    return ''
}

function Get-FieldByRole($Fields, [string]$Role, [bool]$Required = $true) {
    $result = @($Fields | Where-Object { ([string](Get-OptionalProperty $_ 'role' '')).Equals($Role, [StringComparison]::OrdinalIgnoreCase) })
    if ($result.Count -gt 1) { Stop-Generation "Existe más de un campo con role '$Role'." }
    if ($Required -and $result.Count -eq 0) { Stop-Generation "Falta un campo con role '$Role'." }
    if ($result.Count -eq 0) { return $null }
    return $result[0]
}

function Get-TrackedFiles([string]$Root, [string]$Needle) {
    if ([string]::IsNullOrWhiteSpace($Needle)) { return ,@() }
    if (-not (Test-Path -LiteralPath (Join-Path $Root '.git'))) {
        $searchRoots = @(@('src', 'database/sql', 'tests') | ForEach-Object { Join-Path $Root $_ } | Where-Object { Test-Path -LiteralPath $_ })
        if (@($searchRoots).Count -eq 0) { return ,@() }
        $rootPrefix = [System.IO.Path]::GetFullPath($Root).TrimEnd('\') + '\'
        return ,@(Get-ChildItem -LiteralPath $searchRoots -Recurse -File -ErrorAction SilentlyContinue |
            Select-String -SimpleMatch -Pattern $Needle -List -ErrorAction SilentlyContinue |
            ForEach-Object {
                $fullPath = [System.IO.Path]::GetFullPath($_.Path)
                if ($fullPath.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
                    $fullPath.Substring($rootPrefix.Length).Replace('\', '/')
                }
                else { $fullPath.Replace('\', '/') }
            })
    }
    $lines = @(& git -C $Root grep -l -F -- $Needle -- 'src/**' 'database/sql/**' 'tests/**' 2>$null)
    return ,@($lines | Where-Object { $_ })
}

function Get-MigrationState([string]$Root, $Migrations) {
    $numbers = @(Get-ChildItem -LiteralPath (Join-Path $Root 'database/sql') -File -Filter '*.sql' |
        Where-Object { $_.BaseName -match '^(?<number>\d{3})_' } |
        ForEach-Object { [int]$Matches.number })
    $maximum = if ($numbers.Count -eq 0) { 0 } else { ($numbers | Measure-Object -Maximum).Maximum }
    $requested = [ordered]@{
        tenant = Get-OptionalProperty $Migrations 'tenant' $null
        masterNavigation = Get-OptionalProperty $Migrations 'masterNavigation' $null
        masterSync = Get-OptionalProperty $Migrations 'masterSync' $null
    }
    $collisions = [System.Collections.Generic.List[string]]::new()
    $offset = 1
    $explicitNumbers = @{}
    foreach ($key in @('tenant', 'masterNavigation', 'masterSync')) {
        if ($null -eq $requested[$key]) {
            $requested[$key] = $maximum + $offset
            $offset++
        }
        elseif ([int]$requested[$key] -le $maximum -or $numbers -contains [int]$requested[$key]) {
            $collisions.Add("$key=$($requested[$key])")
        }
        if ($null -ne $requested[$key]) { $explicitNumbers[[int]$requested[$key]] = $true }
    }
    if (@($requested.Values | Select-Object -Unique).Count -ne 3) {
        Stop-Generation 'tenant, masterNavigation y masterSync deben usar números distintos.'
    }
    return [pscustomobject]@{ maximum = $maximum; assigned = [pscustomobject]$requested; collisions = @($collisions) }
}

function Get-SqlType($Field) {
    $type = ([string]$Field.type).ToLowerInvariant()
    switch ($type) {
        'string' {
            $length = [int](Get-OptionalProperty $Field 'stringLength' 150)
            return "nvarchar($length)"
        }
        'int' { return 'int' }
        'long' { return 'bigint' }
        'bool' { return 'bit' }
        'guid' { return 'uniqueidentifier' }
        'decimal' { return 'decimal(19,6)' }
        'date' { return 'date' }
        'datetime' { return 'datetime2(0)' }
    }
}

function Get-Sha256([string]$Text) {
    $algorithm = [System.Security.Cryptography.SHA256]::Create()
    try {
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($Text)
        return (($algorithm.ComputeHash($bytes) | ForEach-Object { $_.ToString('x2') }) -join '')
    }
    finally {
        $algorithm.Dispose()
    }
}

function Get-SqlDefault($Field) {
    $value = Get-OptionalProperty $Field 'default' $null
    if ($null -eq $value) { return $null }
    $type = ([string]$Field.type).ToLowerInvariant()
    if ($type -eq 'bool') { return $(if ([bool]$value) { '1' } else { '0' }) }
    if ($type -in @('int', 'long', 'decimal')) { return [string]$value }
    if ($type -in @('date', 'datetime') -and ([string]$value) -eq 'now') { return 'SYSUTCDATETIME()' }
    return "N'$(([string]$value).Replace("'", "''"))'"
}

function New-DesignProposal($Model) {
    $columns = [System.Collections.Generic.List[object]]::new()
    $columns.Add([pscustomobject][ordered]@{ name = 'Id'; sqlType = 'int IDENTITY(1,1)'; nullable = $false; default = $null; purpose = 'Clave local y PK' })
    $columns.Add([pscustomobject][ordered]@{ name = 'GlobalId'; sqlType = 'uniqueidentifier'; nullable = $false; default = 'NEWSEQUENTIALID()'; purpose = 'Identidad global para sincronización' })
    foreach ($field in $Model.fields) {
        $columns.Add([pscustomobject][ordered]@{
            name = [string]$field.name
            sqlType = Get-SqlType $field
            nullable = [bool](Get-OptionalProperty $field 'nullable' $false)
            default = Get-SqlDefault $field
            purpose = [string](Get-OptionalProperty $field 'role' 'custom')
        })
    }
    $columns.Add([pscustomobject][ordered]@{ name = 'CreatedByUserId'; sqlType = 'int'; nullable = $true; default = $null; purpose = 'Auditoría de creación' })
    $columns.Add([pscustomobject][ordered]@{ name = 'CreatedByUserName'; sqlType = 'nvarchar(120)'; nullable = $true; default = $null; purpose = 'Auditoría de creación' })
    $columns.Add([pscustomobject][ordered]@{ name = 'CreatedAt'; sqlType = 'datetime2(0)'; nullable = $false; default = 'SYSUTCDATETIME()'; purpose = 'Auditoría de creación' })
    $columns.Add([pscustomobject][ordered]@{ name = 'UpdatedByUserId'; sqlType = 'int'; nullable = $true; default = $null; purpose = 'Auditoría de modificación' })
    $columns.Add([pscustomobject][ordered]@{ name = 'UpdatedByUserName'; sqlType = 'nvarchar(120)'; nullable = $true; default = $null; purpose = 'Auditoría de modificación' })
    $columns.Add([pscustomobject][ordered]@{ name = 'UpdatedAt'; sqlType = 'datetime2(0)'; nullable = $true; default = $null; purpose = 'Auditoría de modificación' })
    if ([bool]$Model.softDelete) {
        $columns.Add([pscustomobject][ordered]@{ name = 'IsDeleted'; sqlType = 'bit'; nullable = $false; default = '0'; purpose = 'Borrado lógico' })
        $columns.Add([pscustomobject][ordered]@{ name = 'DeletedByUserId'; sqlType = 'int'; nullable = $true; default = $null; purpose = 'Auditoría de eliminación' })
        $columns.Add([pscustomobject][ordered]@{ name = 'DeletedByUserName'; sqlType = 'nvarchar(120)'; nullable = $true; default = $null; purpose = 'Auditoría de eliminación' })
        $columns.Add([pscustomobject][ordered]@{ name = 'DeletedAt'; sqlType = 'datetime2(0)'; nullable = $true; default = $null; purpose = 'Auditoría de eliminación' })
    }

    $constraints = [System.Collections.Generic.List[string]]::new()
    $constraints.Add('PK: Id')
    $constraints.Add('UQ: GlobalId, incluidos tombstones')
    foreach ($field in @($Model.fields | Where-Object { [bool](Get-OptionalProperty $_ 'unique' $false) })) {
        $constraints.Add("UQ: $([string]$field.name), incluidos tombstones")
    }
    if ($null -ne $Model.dependency) {
        $scope = @((Get-OptionalProperty $Model.dependency 'uniquenessScope' @()) | ForEach-Object { [string]$_ })
        if ($scope.Count -gt 0) { $constraints.Add('UQ compuesta: ' + ($scope -join ', ')) }
        $constraints.Add("FK requerida: $([string]$Model.dependency.field) -> $([string]$Model.dependency.parentEntity)")
    }
    if ($null -ne $Model.classification) {
        $codes = @($Model.classification.allowedValues | ForEach-Object { [string]$_.code })
        $constraints.Add("CHECK: $([string]$Model.classification.field) en {$($codes -join ', ')}")
    }

    $fieldLabels = @($Model.fields | ForEach-Object { [string]$_.name }) -join ', '
    $layout = [string](Get-OptionalProperty $Model.ui 'layout' 'single-section')
    $brief = "Mockup full color de escritorio WinForms DevExpress para NuanSystem. Maestro '$($Model.title)', arquetipo '$($Model.archetype)', layout '$layout', fondo blanco, verde corporativo #00B894, tipografía Segoe UI, títulos con línea lateral, etiquetas a la izquierda, editores de 22 px y ritmo vertical de 28 px. Mostrar exactamente estos campos: $fieldLabels. Sin inventar campos, pestañas, paneles, leyendas ni divisores no declarados. Incluir botones corporativos Cancelar y Guardar."

    $canonicalParts = [System.Collections.Generic.List[string]]::new()
    foreach ($part in @('proposalVersion=1.0', "archetype=$($Model.archetype)", "singular=$($Model.singular)", "title=$($Model.title)", "table=$($Model.table)")) {
        $canonicalParts.Add($part)
    }
    foreach ($column in $columns) {
        $canonicalParts.Add("column=$($column.name)|$($column.sqlType)|$($column.nullable)|$($column.default)|$($column.purpose)")
    }
    foreach ($constraint in $constraints) { $canonicalParts.Add("constraint=$constraint") }
    if ($null -ne $Model.classification) {
        $canonicalParts.Add("classification.field=$([string]$Model.classification.field)")
        $canonicalParts.Add("classification.label=$([string](Get-OptionalProperty $Model.classification 'label' ''))")
        foreach ($value in @($Model.classification.allowedValues)) { $canonicalParts.Add("classification.value=$([string]$value.code)|$([string]$value.label)") }
        foreach ($field in @(Get-OptionalProperty $Model.classification 'systemProtectedFields' @())) { $canonicalParts.Add("classification.protected=$([string]$field)") }
    }
    if ($null -ne $Model.dependency) {
        foreach ($propertyName in @('field','parentEntity','parentPlural','parentIdField','parentGlobalIdField','lookupRoute','parentFormKey','required','allowCreate','allowEdit')) {
            $canonicalParts.Add("dependency.$propertyName=$([string](Get-OptionalProperty $Model.dependency $propertyName ''))")
        }
        foreach ($field in @(Get-OptionalProperty $Model.dependency 'uniquenessScope' @())) { $canonicalParts.Add("dependency.uniqueness=$([string]$field)") }
    }
    foreach ($property in @($Model.ui.PSObject.Properties | Sort-Object Name)) { $canonicalParts.Add("ui.$($property.Name)=$([string]$property.Value)") }
    $canonicalParts.Add("auditEnabled=$([bool](Get-OptionalProperty $Model.audit 'enabled' $true))")
    $canonicalParts.Add("softDeleteEnabled=$([bool]$Model.softDelete)")
    $canonicalParts.Add("mockupBrief=$brief")
    $canonical = $canonicalParts -join "`n"
    return [pscustomobject][ordered]@{
        proposalHash = Get-Sha256 $canonical
        table = $Model.table
        columns = @($columns)
        constraints = @($constraints)
        mockupBrief = $brief
        approvalInstruction = 'Muestre esta estructura como tabla, genere y muestre el mockup full color, y espere aprobación explícita del usuario.'
    }
}

function Get-DesignApprovalStatus($Raw, [string]$ExpectedHash) {
    $approval = Get-OptionalProperty $Raw 'designApproval' $null
    if ($null -eq $approval) {
        return [pscustomobject][ordered]@{ approved = $false; reason = 'Falta designApproval.'; proposalHash = $ExpectedHash }
    }
    $status = [string](Get-OptionalProperty $approval 'status' '')
    $approvedHash = ([string](Get-OptionalProperty $approval 'proposalHash' '')).ToLowerInvariant()
    $tableApproved = [bool](Get-OptionalProperty $approval 'tableApproved' $false)
    $mockupApproved = [bool](Get-OptionalProperty $approval 'mockupApproved' $false)
    $mockupReference = [string](Get-OptionalProperty $approval 'mockupReference' '')
    $evidence = [string](Get-OptionalProperty $approval 'evidence' '')
    $approved = $status -eq 'approved' -and $approvedHash -eq $ExpectedHash -and $tableApproved -and $mockupApproved -and
        -not [string]::IsNullOrWhiteSpace($mockupReference) -and $evidence.Length -ge 10
    $reason = if ($approved) { 'Aprobación vigente para la propuesta actual.' } else { 'La aprobación falta, está incompleta o corresponde a otra estructura/mockup.' }
    return [pscustomobject][ordered]@{ approved = $approved; reason = $reason; proposalHash = $ExpectedHash }
}

function Get-ValidationRule($Field) {
    $name = [string]$Field.name
    $rules = @("validator.RuleFor(x => x.$name)")
    $chain = @()
    if ([bool](Get-OptionalProperty $Field 'required' $false)) { $chain += 'NotEmpty()' }
    if (([string]$Field.type).Equals('string', [StringComparison]::OrdinalIgnoreCase)) {
        $length = Get-OptionalProperty $Field 'stringLength' $null
        if ($null -ne $length) { $chain += "MaximumLength($length)" }
    }
    $minimum = Get-OptionalProperty $Field 'minimum' $null
    if ($null -ne $minimum) { $chain += "GreaterThanOrEqualTo($minimum)" }
    if ($chain.Count -eq 0) { return $null }
    return "        $($rules[0]).$($chain -join '.').WithName(`"$name`");"
}

function New-TokenMap($Model) {
    $fields = @($Model.fields)
    $fieldNames = @($fields | ForEach-Object { $_.name }) -join ', '
    $requestArguments = @($fields | ForEach-Object { "request.$($_.name)" }) -join ', '
    $itemArguments = @($fields | ForEach-Object { "item.$($_.name)" }) -join ', '
    $codeField = Get-FieldByRole $fields 'code'
    $nameField = Get-FieldByRole $fields 'name'
    $activeField = Get-FieldByRole $fields 'active'
    $sqlParameters = @($fields | ForEach-Object {
        $default = Get-OptionalProperty $_ 'default' $null
        $defaultSql = if ($null -eq $default) { if ([bool](Get-OptionalProperty $_ 'nullable' $false)) { '=NULL' } else { '' } }
            elseif ($default -is [bool]) { if ($default) { '=1' } else { '=0' } }
            elseif ($default -is [string]) { "=N'$($default.Replace("'", "''"))'" }
            else { "=$default" }
        "@$($_.name) $(Get-SqlType $_)$defaultSql"
    }) -join ','
    $sqlInsertColumns = @($fields | ForEach-Object { "[$($_.name)]" }) -join ','
    $sqlInsertValues = @($fields | ForEach-Object { "@$($_.name)" }) -join ','
    $sqlUpdateSet = @($fields | ForEach-Object { "[$($_.name)]=@$($_.name)" }) -join ','
    $sqlOldDeclarations = @($fields | ForEach-Object { "@Old$($_.name) $(Get-SqlType $_)" }) -join ','
    $sqlOldSelection = @($fields | ForEach-Object { "@Old$($_.name)=[$($_.name)]" }) -join ','
    $sqlAuditNewValues = @($fields | ForEach-Object { "(N'$($_.name)',CONVERT(nvarchar(max),@$($_.name)))" }) -join ",`n       "
    $sqlAuditChangedValues = @($fields | ForEach-Object { "(N'$($_.name)',CONVERT(nvarchar(max),@Old$($_.name)),CONVERT(nvarchar(max),@$($_.name)))" }) -join ",`n       "
    $dtoProperties = @($fields | ForEach-Object {
        "    public $(Get-CSharpType $_) $($_.name) { get; set; }$(Get-CSharpDefault $_)"
    }) -join "`n"
    $recordParameters = @($fields | ForEach-Object { "$(Get-CSharpType $_) $($_.name)" }) -join ', '
    $validatorLines = @($fields | ForEach-Object { Get-ValidationRule $_ } | Where-Object { $_ }) -join "`n"
    $sqlColumns = @($fields | ForEach-Object {
        $nullable = if ([bool](Get-OptionalProperty $_ 'nullable' $false)) { 'NULL' } else { 'NOT NULL' }
        $default = Get-OptionalProperty $_ 'default' $null
        $defaultSql = if ($null -eq $default) { '' } elseif ($default -is [bool]) { if ($default) { ' CONSTRAINT DF_' + $Model.tableName + '_' + $_.name + ' DEFAULT(1)' } else { ' CONSTRAINT DF_' + $Model.tableName + '_' + $_.name + ' DEFAULT(0)' } } else { '' }
        "        [$($_.name)] $(Get-SqlType $_) $nullable$defaultSql"
    }) -join ",`n"
    $gridColumns = @($fields | Where-Object { $_.role -notin @('description', 'parentGlobalId') } | ForEach-Object {
        $caption = [string](Get-OptionalProperty $_ 'label' $_.name)
        $alignment = if ($_.type -in @('int','long','decimal')) { ', DevExpress.Utils.HorzAlignment.Far' } else { '' }
        "        Column(nameof($($Model.singular)Item.$($_.name)), `"$caption`", $([array]::IndexOf($fields, $_) + 1), 140$alignment);"
    }) -join "`n"
    $classificationBlock = ''
    if ($Model.archetype -eq 'classified') {
        $values = @($Model.classification.allowedValues | ForEach-Object { "        new(`"$($_.code)`", `"$($_.label)`")" }) -join ",`n"
        $classificationBlock = @"
    private static readonly ClassificationOption[] ClassificationOptions =
    [
$values
    ];
"@
    }
    $dependencyBlock = ''
    if ($Model.archetype -eq 'dependent') {
        $dependencyBlock = @"
    // Parent lookup contract: $($Model.dependency.parentPlural) via $($Model.dependency.lookupRoute).
    // Local FK: $($Model.dependency.parentIdField); sync identity: $($Model.dependency.parentGlobalIdField); permission source: $($Model.dependency.parentFormKey).
"@
    }

    $designerDeclarations = [System.Collections.Generic.List[string]]::new()
    $designerCreates = [System.Collections.Generic.List[string]]::new()
    $designerBegin = [System.Collections.Generic.List[string]]::new()
    $designerLayout = [System.Collections.Generic.List[string]]::new()
    $designerAdds = [System.Collections.Generic.List[string]]::new()
    $designerEnd = [System.Collections.Generic.List[string]]::new()
    $editAssignments = [System.Collections.Generic.List[string]]::new()
    $buildArguments = [System.Collections.Generic.List[string]]::new()
    $emptyArguments = [System.Collections.Generic.List[string]]::new()
    $validationLines = [System.Collections.Generic.List[string]]::new()
    $y = 60
    $tabIndex = 0
    foreach ($field in $fields) {
        $fieldName = [string]$field.name
        $role = [string](Get-OptionalProperty $field 'role' '')
        $label = [string](Get-OptionalProperty $field 'label' $fieldName)
        $controlName = "txt$fieldName"
        $controlType = 'TextEdit'
        if ($role -eq 'description') { $controlName = "mem$fieldName"; $controlType = 'MemoEdit' }
        elseif ($Model.archetype -eq 'dependent' -and $Model.dependency.field -eq $fieldName) { $controlName = "lue$fieldName"; $controlType = 'NuanLookupEdit' }
        elseif ($Model.archetype -eq 'classified' -and $Model.classification.field -eq $fieldName) { $controlName = "lue$fieldName"; $controlType = 'LookUpEdit' }
        elseif ($role -eq 'sortOrder' -or $field.type -in @('int','long','decimal')) { $controlName = "spn$fieldName"; $controlType = 'SpinEdit' }
        elseif ($role -eq 'isActive' -or $field.type -eq 'bool') { $controlName = "tgl$fieldName"; $controlType = 'NuanToggleSwitch' }
        $labelName = "lbl$fieldName"
        $designerDeclarations.Add("    private LabelControl $labelName;")
        $designerDeclarations.Add("    private $controlType $controlName;")
        $designerCreates.Add("        $labelName = new LabelControl();")
        $designerCreates.Add("        $controlName = new $controlType();")
        $designerBegin.Add("        ((System.ComponentModel.ISupportInitialize)$controlName.Properties).BeginInit();")
        $height = if ($controlType -eq 'MemoEdit') { 64 } else { if ($controlType -eq 'NuanToggleSwitch') { 20 } else { 22 } }
        $width = if ($controlType -eq 'MemoEdit') { 940 } else { 320 }
        $designerLayout.Add(@"
        $labelName.Appearance.Font = new Font("Segoe UI", 9F);
        $labelName.Appearance.ForeColor = BrandResources.Text;
        $labelName.Appearance.Options.UseFont = true;
        $labelName.Appearance.Options.UseForeColor = true;
        $labelName.Location = new Point(32, $($y + 3));
        $labelName.Name = "$labelName";
        $labelName.Text = "$label`:";
        $controlName.Location = new Point(180, $y);
        $controlName.Name = "$controlName";
        $controlName.Properties.Appearance.Font = new Font("Segoe UI", 9F);
        $controlName.Properties.Appearance.Options.UseFont = true;
        $controlName.Size = new Size($width, $height);
        $controlName.TabIndex = $tabIndex;
"@)
        if ($controlType -in @('TextEdit','SpinEdit','LookUpEdit','NuanLookupEdit')) {
            $designerLayout.Add("        $controlName.Properties.AutoHeight = false;")
        }
        if ($controlType -eq 'NuanToggleSwitch') {
            $designerLayout.Add("        $controlName.ActiveColor = BrandResources.Primary;")
            $designerLayout.Add("        $controlName.InactiveColor = BrandResources.Border;")
            $designerLayout.Add("        $controlName.Properties.OnText = `"Sí`";")
            $designerLayout.Add("        $controlName.Properties.OffText = `"No`";")
        }
        if ($controlType -in @('LookUpEdit','NuanLookupEdit')) {
            $designerLayout.Add("        $controlName.Properties.Buttons.AddRange(new EditorButton[] { new EditorButton(ButtonPredefines.Combo) });")
            $designerLayout.Add("        $controlName.Properties.NullText = `"`";")
            $designerLayout.Add("        $controlName.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;")
        }
        $designerAdds.Add("        Controls.Add($labelName);")
        $designerAdds.Add("        Controls.Add($controlName);")
        $designerEnd.Add("        ((System.ComponentModel.ISupportInitialize)$controlName.Properties).EndInit();")
        switch ($controlType) {
            'MemoEdit' {
                $editAssignments.Add("        $controlName.Text = item.$fieldName;")
                $buildArguments.Add("            Optional($controlName.Text)")
                $emptyArguments.Add('null')
            }
            'SpinEdit' {
                $editAssignments.Add("        $controlName.Value = item.$fieldName;")
                $cast = if ($field.type -eq 'decimal') { 'Convert.ToDecimal' } elseif ($field.type -eq 'long') { 'Convert.ToInt64' } else { 'Convert.ToInt32' }
                $buildArguments.Add("            $cast($controlName.Value)")
                $emptyArguments.Add('0')
            }
            'NuanToggleSwitch' {
                $editAssignments.Add("        $controlName.Checked = item.$fieldName;")
                $buildArguments.Add("            $controlName.Checked")
                $emptyArguments.Add($(if ([bool](Get-OptionalProperty $field 'default' $false)) { 'true' } else { 'false' }))
            }
            default {
                $editAssignments.Add("        $controlName.EditValue = item.$fieldName;")
                if ($field.type -eq 'string') {
                    $buildArguments.Add($(if ([bool](Get-OptionalProperty $field 'nullable' $false)) { "            Optional(Convert.ToString($controlName.EditValue))" } else { "            Convert.ToString($controlName.EditValue)?.Trim() ?? string.Empty" }))
                    $emptyArguments.Add($(if ([bool](Get-OptionalProperty $field 'nullable' $false)) { 'null' } else { 'string.Empty' }))
                }
                elseif ($field.type -eq 'guid') { $buildArguments.Add("            (Guid)$controlName.EditValue"); $emptyArguments.Add('Guid.Empty') }
                else { $buildArguments.Add("            Convert.ToInt32($controlName.EditValue)"); $emptyArguments.Add('0') }
            }
        }
        if ([bool](Get-OptionalProperty $field 'required' $false) -and $field.type -eq 'string') {
            $validationLines.Add("        valid &= Validator.RequireText($controlName, `"Ingrese $($label.ToLowerInvariant()).`");")
        }
        $y += if ($controlType -eq 'MemoEdit') { 76 } else { 28 }
        $tabIndex++
    }
    return [ordered]@{
        SINGULAR = $Model.singular
        PLURAL = $Model.plural
        TITLE = $Model.title
        FORM_KEY = $Model.formKey
        API_ROUTE = $Model.route
        TABLE = $Model.table
        TABLE_NAME = $Model.tableName
        ENTITY_CODE = $Model.entityCode
        PERMISSION_READ = $Model.permissionRead
        PERMISSION_MANAGE = $Model.permissionManage
        MENU_CODE = $Model.menuCode
        MENU_ORDER = [string]$Model.menuOrder
        MIGRATION_TENANT = ('{0:D3}' -f [int]$Model.migrations.tenant)
        MIGRATION_NAVIGATION = ('{0:D3}' -f [int]$Model.migrations.masterNavigation)
        MIGRATION_SYNC = ('{0:D3}' -f [int]$Model.migrations.masterSync)
        DTO_PROPERTIES = $dtoProperties
        RECORD_PARAMETERS = $recordParameters
        FIELD_NAMES = $fieldNames
        REQUEST_ARGUMENTS = $requestArguments
        ITEM_ARGUMENTS = $itemArguments
        CODE_FIELD = $codeField.name
        NAME_FIELD = $nameField.name
        ACTIVE_FIELD = $activeField.name
        SQL_PARAMETERS = $sqlParameters
        SQL_INSERT_COLUMNS = $sqlInsertColumns
        SQL_INSERT_VALUES = $sqlInsertValues
        SQL_UPDATE_SET = $sqlUpdateSet
        SQL_OLD_DECLARATIONS = $sqlOldDeclarations
        SQL_OLD_SELECTION = $sqlOldSelection
        SQL_AUDIT_NEW_VALUES = $sqlAuditNewValues
        SQL_AUDIT_CHANGED_VALUES = $sqlAuditChangedValues
        VALIDATOR_RULES = $validatorLines
        SQL_COLUMNS = $sqlColumns
        GRID_COLUMNS = $gridColumns
        CLASSIFICATION_BLOCK = $classificationBlock
        DEPENDENCY_BLOCK = $dependencyBlock
        DESIGNER_DECLARATIONS = ($designerDeclarations -join "`n")
        DESIGNER_CREATES = ($designerCreates -join "`n")
        DESIGNER_BEGIN_INIT = ($designerBegin -join "`n")
        DESIGNER_LAYOUT = ($designerLayout -join "`n")
        DESIGNER_ADDS = ($designerAdds -join "`n")
        DESIGNER_END_INIT = ($designerEnd -join "`n")
        EDIT_ASSIGNMENTS = ($editAssignments -join "`n")
        BUILD_REQUEST_ARGUMENTS = ($buildArguments -join ",`n")
        EMPTY_REQUEST_ARGUMENTS = ($emptyArguments -join ', ')
        FORM_VALIDATION = ($validationLines -join "`n")
        FORM_HEIGHT = [string]([Math]::Max(360, $y + 100))
        ARCHETYPE = $Model.archetype
        SYNC_ORDER = [string](Get-OptionalProperty $Model.synchronization 'executionOrder' 900)
    }
}

function Expand-Template([string]$Text, $Tokens, [string]$TemplateName) {
    $result = $Text
    foreach ($entry in $Tokens.GetEnumerator()) {
        $result = $result.Replace("{{$($entry.Key)}}", [string]$entry.Value)
    }
    $unresolved = @([regex]::Matches($result, '\{\{[A-Z0-9_]+\}\}') | ForEach-Object { $_.Value } | Select-Object -Unique)
    if ($unresolved.Count -gt 0) {
        Stop-Generation "La plantilla '$TemplateName' conserva tokens sin resolver: $($unresolved -join ', ')."
    }
    return $result.Replace("`r`n", "`n")
}

$root = Get-RepositoryRoot $RepositoryRoot
$manifestPath = (Resolve-Path -LiteralPath $Manifest).Path
$manifestText = [System.IO.File]::ReadAllText($manifestPath, $Utf8Strict)
if ($manifestText -match '(?i)(password|pwd|private[\s_-]*key|connection[\s_-]*string|client[\s_-]*secret|access[\s_-]*token|api[\s_-]*key)\s*["'':=]') {
    Stop-Generation 'El manifiesto parece contener secretos o credenciales; use solo identificadores no sensibles.'
}
try { $raw = $manifestText | ConvertFrom-Json }
catch { Stop-Generation "El manifiesto no es JSON válido. $($_.Exception.Message)" }

$schemaVersion = [string](Get-RequiredProperty $raw 'schemaVersion' 'root')
if ($schemaVersion -ne '1.1') { Stop-Generation "schemaVersion '$schemaVersion' no está soportado; use 1.1." }
$archetype = ([string](Get-RequiredProperty $raw 'archetype' 'root')).ToLowerInvariant()
if ($archetype -notin @('basic', 'classified', 'dependent')) { Stop-Generation "Arquetipo no soportado: '$archetype'." }
$entity = Get-RequiredProperty $raw 'entity' 'root'
$api = Get-RequiredProperty $raw 'api' 'root'
$navigation = Get-RequiredProperty $raw 'navigation' 'root'
$permissions = Get-RequiredProperty $raw 'permissions' 'root'
$fields = @(Get-RequiredProperty $raw 'fields' 'root')
if ($fields.Count -eq 0) { Stop-Generation 'fields debe contener al menos un campo.' }

$singular = [string](Get-RequiredProperty $entity 'singular' 'entity')
$plural = [string](Get-RequiredProperty $entity 'plural' 'entity')
$title = [string](Get-RequiredProperty $entity 'title' 'entity')
$table = [string](Get-RequiredProperty $entity 'table' 'entity')
$entityCode = [string](Get-RequiredProperty $entity 'entityCode' 'entity')
$route = [string](Get-RequiredProperty $api 'route' 'api')
if (-not $route.StartsWith('/')) { $route = '/' + $route }
$formKey = [string](Get-RequiredProperty $api 'formKey' 'api')
$permissionRead = [string](Get-RequiredProperty $permissions 'read' 'permissions')
$permissionManage = [string](Get-RequiredProperty $permissions 'manage' 'permissions')
$menuOrder = [int](Get-RequiredProperty $navigation 'menuOrder' 'navigation')
$menuCode = [string](Get-OptionalProperty $navigation 'menuCode' ("MENU.DEFINITIONS.INVENTORY." + $plural.ToUpperInvariant()))

Assert-Matches $singular '^[A-Z][A-Za-z0-9]*$' 'entity.singular'
Assert-Matches $plural '^[A-Z][A-Za-z0-9]*$' 'entity.plural'
Assert-Matches $entityCode '^[A-Z][A-Za-z0-9]*$' 'entity.entityCode'
Assert-Matches $formKey '^[a-z0-9]+(?:-[a-z0-9]+)*$' 'api.formKey'
Assert-Matches $route '^/api/definitions/inventory/[a-z0-9]+(?:-[a-z0-9]+)*$' 'api.route'
Assert-Matches $table '^(dbo\.)[A-Za-z][A-Za-z0-9_]*$' 'entity.table'
Assert-Matches $permissionRead '^(?:[A-Z0-9]+(?:\.[A-Z0-9]+)+|[A-Z][A-Za-z0-9]+)$' 'permissions.read'
Assert-Matches $permissionManage '^(?:[A-Z0-9]+(?:\.[A-Z0-9]+)+|[A-Z][A-Za-z0-9]+)$' 'permissions.manage'
if ($menuOrder -lt 0) { Stop-Generation 'navigation.menuOrder no puede ser negativo.' }

$fieldNames = @{}
foreach ($field in $fields) {
    $name = [string](Get-RequiredProperty $field 'name' 'fields[]')
    Assert-Matches $name '^[A-Z][A-Za-z0-9]*$' "fields.$name.name"
    if ($fieldNames.ContainsKey($name.ToLowerInvariant())) { Stop-Generation "Campo duplicado: '$name'." }
    $fieldNames[$name.ToLowerInvariant()] = $true
    [void](Get-CSharpType $field)
}
foreach ($role in @('code', 'name', 'description', 'sortOrder', 'active')) { [void](Get-FieldByRole $fields $role $true) }

$classification = Get-OptionalProperty $raw 'classification' $null
$dependency = Get-OptionalProperty $raw 'dependency' $null
if ($archetype -eq 'classified') {
    if ($null -eq $classification) { Stop-Generation 'El arquetipo classified requiere classification.' }
    $classificationField = [string](Get-RequiredProperty $classification 'field' 'classification')
    if (-not $fieldNames.ContainsKey($classificationField.ToLowerInvariant())) { Stop-Generation 'classification.field no existe en fields.' }
    $codeSet = @(Get-RequiredProperty $classification 'allowedValues' 'classification')
    if ($codeSet.Count -lt 2) { Stop-Generation 'classification.allowedValues debe contener al menos dos valores.' }
}
if ($archetype -eq 'dependent') {
    if ($null -eq $dependency) { Stop-Generation 'El arquetipo dependent requiere dependency.' }
    $dependencyField = [string](Get-RequiredProperty $dependency 'field' 'dependency')
    if (-not $fieldNames.ContainsKey($dependencyField.ToLowerInvariant())) { Stop-Generation 'dependency.field no existe en fields.' }
    [void](Get-RequiredProperty $dependency 'parentEntity' 'dependency')
    [void](Get-RequiredProperty $dependency 'parentPlural' 'dependency')
    [void](Get-RequiredProperty $dependency 'lookupRoute' 'dependency')
}

$migrationState = Get-MigrationState $root (Get-OptionalProperty $raw 'migrations' ([pscustomobject]@{}))
$softDeleteValue = Get-OptionalProperty $raw 'softDelete' $true
if ($softDeleteValue -isnot [bool]) { $softDeleteValue = [bool](Get-OptionalProperty $softDeleteValue 'enabled' $true) }
$model = [pscustomobject][ordered]@{
    schemaVersion = '1.1'; archetype = $archetype; singular = $singular; plural = $plural; title = $title
    table = $table; tableName = Convert-ToSqlName $table; entityCode = $entityCode; route = $route; formKey = $formKey
    permissionRead = $permissionRead; permissionManage = $permissionManage; menuCode = $menuCode; menuOrder = $menuOrder
    fields = $fields; classification = $classification; dependency = $dependency
    ui = Get-OptionalProperty $raw 'ui' ([pscustomobject]@{}); audit = Get-OptionalProperty $raw 'audit' ([pscustomobject]@{ enabled = $true })
    softDelete = $softDeleteValue
    synchronization = Get-OptionalProperty $raw 'synchronization' ([pscustomobject]@{ mode = 'none'; enabledByDefault = $false })
    migrations = $migrationState.assigned
}
$designProposal = New-DesignProposal $model
$designApprovalStatus = Get-DesignApprovalStatus $raw $designProposal.proposalHash

$safeSlug = Convert-ToKebab $plural
$sqlSlug = Convert-ToSnake $plural
$targets = [ordered]@{
    'Dtos.cs.tmpl' = "src/Backend/NuanSystem.Application/Features/Definitions/Inventory/$plural/Dtos/${singular}Dtos.cs"
    'Commands.cs.tmpl' = "src/Backend/NuanSystem.Application/Features/Definitions/Inventory/$plural/Commands/${singular}Commands.cs"
    'CommandHandlers.cs.tmpl' = "src/Backend/NuanSystem.Application/Features/Definitions/Inventory/$plural/Commands/${singular}CommandHandlers.cs"
    'Validators.cs.tmpl' = "src/Backend/NuanSystem.Application/Features/Definitions/Inventory/$plural/Commands/${singular}CommandValidators.cs"
    'Queries.cs.tmpl' = "src/Backend/NuanSystem.Application/Features/Definitions/Inventory/$plural/Queries/${singular}Queries.cs"
    'QueryHandlers.cs.tmpl' = "src/Backend/NuanSystem.Application/Features/Definitions/Inventory/$plural/Queries/${singular}QueryHandlers.cs"
    'RepositoryContract.cs.tmpl' = "src/Backend/NuanSystem.Application/Abstractions/Data/I${singular}Repository.cs"
    'Endpoints.cs.tmpl' = "src/Backend/NuanSystem.Api/Endpoints/Definitions/Inventory/$plural/${singular}Endpoints.cs"
    'Repository.cs.tmpl' = "src/Backend/NuanSystem.Persistence/Repositories/Definitions/Inventory/${singular}Repository.cs"
    'FrontendModels.cs.tmpl' = "src/Frontend/NuanSystem.WinForms.Services/Definitions/Inventory/$plural/Models/${singular}Models.cs"
    'FrontendClient.cs.tmpl' = "src/Frontend/NuanSystem.WinForms.Services/Definitions/Inventory/$plural/${singular}Client.cs"
    'ViewModel.cs.tmpl' = "src/Frontend/NuanSystem.WinForms.ViewModels/Definitions/Inventory/$plural/${plural}ViewModel.cs"
    'ListForm.cs.tmpl' = "src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/$plural/${plural}Form.cs"
    'EditForm.cs.tmpl' = "src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/$plural/${singular}EditForm.cs"
    'EditForm.Designer.cs.tmpl' = "src/Frontend/NuanSystem.WinForms.Forms/Definitions/Inventory/$plural/${singular}EditForm.Designer.cs"
    'Tenant.sql.tmpl' = "database/sql/$('{0:D3}' -f [int]$model.migrations.tenant)_tenant_${sqlSlug}_master.sql"
    'Navigation.sql.tmpl' = "database/sql/$('{0:D3}' -f [int]$model.migrations.masterNavigation)_master_definitions_inventory_${sqlSlug}_navigation.sql"
    'Sync.sql.tmpl' = "database/sql/$('{0:D3}' -f [int]$model.migrations.masterSync)_master_${sqlSlug}_sync_registration.sql"
    'ContractTests.cs.tmpl' = "tests/NuanSystem.Application.Tests/Features/Definitions/Inventory/$plural/${singular}GeneratedContractTests.cs"
}

$collisions = [System.Collections.Generic.List[object]]::new()
foreach ($migrationCollision in $migrationState.collisions) {
    $collisions.Add([pscustomobject]@{ kind = 'migration'; value = $migrationCollision })
}
foreach ($target in $targets.Values) {
    if (Test-Path -LiteralPath (Join-Path $root $target)) {
        $collisions.Add([pscustomobject]@{ kind = 'destination'; value = $target })
    }
}
foreach ($identity in @(
    [pscustomobject]@{ kind = 'route'; value = $route },
    [pscustomobject]@{ kind = 'formKey'; value = "`"$formKey`"" },
    [pscustomobject]@{ kind = 'table'; value = $table }
)) {
    foreach ($hit in (Get-TrackedFiles $root $identity.value)) {
        if (-not ($collisions | Where-Object { $_.kind -eq $identity.kind -and $_.value -eq $hit })) {
            $collisions.Add([pscustomobject]@{ kind = $identity.kind; value = $hit })
        }
    }
}

$filePlan = @($targets.GetEnumerator() | ForEach-Object {
    [pscustomobject][ordered]@{
        template = "assets/templates/common/$($_.Key)"
        destination = $_.Value
        state = if (Test-Path -LiteralPath (Join-Path $root $_.Value)) { 'Collision' } else { 'New' }
    }
})
$collisionArray = @($collisions.ToArray())
$plan = [pscustomobject][ordered]@{
    schemaVersion = '1.1'; mode = $Mode; archetype = $archetype; entity = $singular; formKey = $formKey; route = $route
    designProposalHash = $designProposal.proposalHash; designApproved = $designApprovalStatus.approved; designApprovalReason = $designApprovalStatus.reason
    migrationMaximumObserved = $migrationState.maximum; migrations = $model.migrations
    collisionCount = $collisionArray.Count; collisions = $collisionArray; files = $filePlan
    guarantees = @('staging-only', 'no-overwrite', 'no-sql-execution', 'no-worker-activation', 'no-sap-call', 'no-git-mutation')
}

if ($Mode -eq 'Validate') {
    [pscustomobject][ordered]@{ valid = $true; archetype = $archetype; entity = $singular; collisionCount = $collisionArray.Count; migrations = $model.migrations; designProposalHash = $designProposal.proposalHash; designApproved = $designApprovalStatus.approved } |
        ConvertTo-Json -Depth 10
    return
}
if ($Mode -eq 'Propose') {
    $designProposal | ConvertTo-Json -Depth 30
    return
}
if (-not $designApprovalStatus.approved) {
    Stop-Generation "Antes de $Mode debe mostrar la estructura de tabla y el mockup full color, obtener aprobación explícita y registrar designApproval con proposalHash '$($designProposal.proposalHash)'."
}
if ($Mode -in @('Preview', 'Diff')) {
    $plan | ConvertTo-Json -Depth 20
    return
}
if ($collisionArray.Count -gt 0) {
    Stop-Generation "Se detectaron $($collisionArray.Count) colisiones. Ejecute -Mode Diff y cambie las identidades antes de Scaffold."
}

if (-not $OutputPath) { $OutputPath = Join-Path $root ".codex-tmp/auxiliary-master-generator/$safeSlug" }
$outputFull = [System.IO.Path]::GetFullPath($OutputPath)
$protected = @((Join-Path $root 'src'), (Join-Path $root 'database'), (Join-Path $root 'tests'), (Join-Path $root '.codex/skills'))
foreach ($path in $protected) {
    if ($outputFull.StartsWith([System.IO.Path]::GetFullPath($path), [StringComparison]::OrdinalIgnoreCase)) {
        Stop-Generation "OutputPath debe ser staging y no puede estar bajo '$path'."
    }
}
if (Test-Path -LiteralPath $outputFull) { Stop-Generation "OutputPath ya existe; no se sobrescribirá: $outputFull" }

$templateRoot = Join-Path (Split-Path -Parent $PSScriptRoot) 'assets/templates/common'
$variantContract = Join-Path (Split-Path -Parent $PSScriptRoot) "assets/templates/$archetype/archetype.json.tmpl"
if (-not (Test-Path -LiteralPath $variantContract)) { Stop-Generation "Falta la plantilla del arquetipo '$archetype'." }
$tokens = New-TokenMap $model
[void](Expand-Template ([System.IO.File]::ReadAllText($variantContract, $Utf8Strict)) $tokens $variantContract)

New-Item -ItemType Directory -Path $outputFull | Out-Null
$filesRoot = Join-Path $outputFull 'files'
foreach ($entry in $targets.GetEnumerator()) {
    $templatePath = Join-Path $templateRoot $entry.Key
    if (-not (Test-Path -LiteralPath $templatePath)) { Stop-Generation "Falta la plantilla '$templatePath'." }
    $content = Expand-Template ([System.IO.File]::ReadAllText($templatePath, $Utf8Strict)) $tokens $entry.Key
    $destination = Join-Path $filesRoot $entry.Value
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $destination) | Out-Null
    [System.IO.File]::WriteAllText($destination, $content, [System.Text.UTF8Encoding]::new($false))
}

$normalized = [pscustomobject][ordered]@{
    schemaVersion = $model.schemaVersion; archetype = $model.archetype
    entity = [pscustomobject][ordered]@{ singular = $singular; plural = $plural; title = $title; table = $table; entityCode = $entityCode }
    api = [pscustomobject][ordered]@{ route = $route; formKey = $formKey }
    navigation = $navigation; permissions = $permissions; fields = $fields
    classification = $classification; dependency = $dependency; ui = $model.ui; designApproval = (Get-RequiredProperty $raw 'designApproval' 'root'); audit = $model.audit
    softDelete = $model.softDelete; synchronization = $model.synchronization; migrations = $model.migrations
}
[System.IO.File]::WriteAllText((Join-Path $outputFull 'manifest.normalized.json'), ($normalized | ConvertTo-Json -Depth 30) + "`n", [System.Text.UTF8Encoding]::new($false))
[System.IO.File]::WriteAllText((Join-Path $outputFull 'generation-plan.json'), ($plan | ConvertTo-Json -Depth 30) + "`n", [System.Text.UTF8Encoding]::new($false))
$checklist = @"
# Integración manual: $title

- [ ] Revalidar los números de migración contra `database/sql` antes de copiar archivos.
- [ ] Revisar SQL, tenancy, auditoría, borrado lógico y sincronización; no ejecutar scripts desde este paquete.
- [ ] Registrar repositorio, endpoints, clientes, ViewModels, formularios, navegación, permisos y FormKey.
- [ ] Revisar el Designer en Visual Studio, incluido ritmo vertical de 28 px y controles corporativos.
- [ ] Ejecutar pruebas dirigidas y builds después de integrar en una rama limpia.
- [ ] Solicitar autorización separada para despliegue SQL, workers, SAP, commit o push.
"@
[System.IO.File]::WriteAllText((Join-Path $outputFull 'integration-checklist.md'), $checklist.Replace("`r`n", "`n"), [System.Text.UTF8Encoding]::new($false))

[pscustomobject][ordered]@{ scaffolded = $true; outputPath = $outputFull; files = $targets.Count; entity = $singular; archetype = $archetype } |
    ConvertTo-Json -Depth 5
