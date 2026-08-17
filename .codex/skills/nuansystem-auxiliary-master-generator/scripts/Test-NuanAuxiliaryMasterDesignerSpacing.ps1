[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $Manifest,
    [Parameter(Mandatory = $true)]
    [string] $DesignerPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [System.Text.UTF8Encoding]::new($false)

function Assert-True {
    param([bool] $Condition, [string] $Message)
    if (-not $Condition) { throw $Message }
}

function Get-OptionalProperty {
    param([object] $Object, [string] $Name, $Default)
    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property) { return $Default }
    return $property.Value
}

function Get-ControlContract {
    param([pscustomobject] $Field, [pscustomobject] $Model)

    $fieldName = [string]$Field.name
    $role = [string](Get-OptionalProperty $Field 'role' '')
    $controlName = "txt$fieldName"
    $controlType = 'TextEdit'
    if ($role -eq 'description') { $controlName = "mem$fieldName"; $controlType = 'MemoEdit' }
    elseif ($Model.archetype -eq 'dependent' -and $Model.dependency.field -eq $fieldName) { $controlName = "lue$fieldName"; $controlType = 'NuanLookupEdit' }
    elseif ($Model.archetype -eq 'classified' -and $Model.classification.field -eq $fieldName) { $controlName = "lue$fieldName"; $controlType = 'LookUpEdit' }
    elseif ($role -eq 'sortOrder' -or $Field.type -in @('int','long','decimal')) { $controlName = "spn$fieldName"; $controlType = 'SpinEdit' }
    elseif ($role -eq 'active' -or $role -eq 'isActive' -or $Field.type -eq 'bool') { $controlName = "tgl$fieldName"; $controlType = 'NuanToggleSwitch' }

    $height = if ($controlType -eq 'MemoEdit') { 64 } elseif ($controlType -eq 'NuanToggleSwitch') { 20 } else { 22 }
    $controlNames = @($controlName)
    $labelNames = @("lbl$fieldName")
    if ($role -eq 'parentId' -and $fieldName.EndsWith('Id', [StringComparison]::Ordinal)) {
        $parentName = $fieldName.Substring(0, $fieldName.Length - 2)
        $controlNames += "lue$parentName"
        $labelNames += "lbl$parentName"
    }
    return [pscustomobject]@{
        FieldName = $fieldName
        Role = $role
        LabelNames = $labelNames
        ControlNames = $controlNames
        ControlType = $controlType
        ExpectedHeight = $height
    }
}

function Get-ExpectedGeometry {
    param([pscustomobject] $Model)

    $fields = @($Model.fields)
    $result = @{}
    $leftRows = [System.Collections.Generic.List[object]]::new()
    $parentField = if ($Model.archetype -eq 'dependent') { [string]$Model.dependency.field } else { '' }
    $classificationField = if ($Model.archetype -eq 'classified') { [string]$Model.classification.field } else { '' }
    if ($parentField) { $leftRows.Add(($fields | Where-Object name -eq $parentField | Select-Object -First 1)) }
    foreach ($role in @('code','name')) { $leftRows.Add(($fields | Where-Object { ([string](Get-OptionalProperty $_ 'role' '')) -eq $role } | Select-Object -First 1)) }
    foreach ($field in $fields) {
        $role = [string](Get-OptionalProperty $field 'role' '')
        if ($field.name -in @($parentField, $classificationField) -or $role -in @('code','name','description','sortOrder','active','isActive','system')) { continue }
        $leftRows.Add($field)
    }
    if ($classificationField) { $leftRows.Add(($fields | Where-Object name -eq $classificationField | Select-Object -First 1)) }
    $leftRows.Add(($fields | Where-Object { ([string](Get-OptionalProperty $_ 'role' '')) -eq 'description' } | Select-Object -First 1))

    for ($index = 0; $index -lt $leftRows.Count; $index++) {
        $field = $leftRows[$index]
        $role = [string](Get-OptionalProperty $field 'role' '')
        $result[[string]$field.name] = [pscustomobject]@{ LabelX=32; EditorX=154; Y=26+(28*$index); Width=if($role -eq 'code'){180}else{436} }
    }
    $sort = $fields | Where-Object { ([string](Get-OptionalProperty $_ 'role' '')) -eq 'sortOrder' } | Select-Object -First 1
    $active = $fields | Where-Object { ([string](Get-OptionalProperty $_ 'role' '')) -in @('active','isActive') } | Select-Object -First 1
    $result[[string]$sort.name] = [pscustomobject]@{ LabelX=632; EditorX=680; Y=26; Width=150 }
    $result[[string]$active.name] = [pscustomobject]@{ LabelX=632; EditorX=684; Y=54; Width=120 }
    $system = @($fields | Where-Object { ([string](Get-OptionalProperty $_ 'role' '')) -eq 'system' } | Select-Object -First 1)
    if ($system.Count -eq 1) {
        $systemY = if ($classificationField) { $result[$classificationField].Y } else { 82 }
        $result[[string]$system[0].name] = [pscustomobject]@{ LabelX=632; EditorX=684; Y=$systemY; Width=120 }
    }
    return $result
}

function Get-CandidateMatch {
    param([string] $Text, [string[]] $Names, [string] $Property, [string] $ValuePattern, [string] $Message)
    $results = [System.Collections.Generic.List[object]]::new()
    foreach ($name in $Names) {
        $pattern = "^\s*" + [regex]::Escape($name) + "\." + [regex]::Escape($Property) + "\s*=\s*" + $ValuePattern
        foreach ($match in [regex]::Matches($Text, $pattern, [System.Text.RegularExpressions.RegexOptions]::Multiline)) {
            $results.Add([pscustomobject]@{ Name = $name; Match = $match })
        }
    }
    Assert-True ($results.Count -eq 1) $Message
    return $results[0]
}

try {
    $manifestPath = (Resolve-Path -LiteralPath $Manifest -ErrorAction Stop).Path
    $designerFullPath = (Resolve-Path -LiteralPath $DesignerPath -ErrorAction Stop).Path
    $model = [System.IO.File]::ReadAllText($manifestPath, [System.Text.UTF8Encoding]::new($false, $true)) | ConvertFrom-Json
    $designer = [System.IO.File]::ReadAllText($designerFullPath, [System.Text.UTF8Encoding]::new($false, $true))
    $geometry = [System.Collections.Generic.List[object]]::new()
    $issues = [System.Collections.Generic.List[string]]::new()
    $expectedGeometry = Get-ExpectedGeometry -Model $model

    foreach ($shellRule in @(
        @{ Pattern='ClientSize\s*=\s*new Size\(870,\s*\d+\);'; Message='El editor debe usar ClientSize.Width = 870.' },
        @{ Pattern='MinimumSize\s*=\s*new Size\(886,\s*\d+\);'; Message='El editor debe usar MinimumSize.Width = 886.' },
        @{ Pattern='CancelButtonLocation\s*=\s*new Point\(628,\s*\d+\);'; Message='Cancelar heredado debe ubicarse en X=628.' },
        @{ Pattern='SaveButtonLocation\s*=\s*new Point\(734,\s*\d+\);'; Message='Guardar heredado debe ubicarse en X=734.' },
        @{ Pattern='FormBorderStyle\s*=\s*FormBorderStyle\.FixedDialog;'; Message='El editor debe ser FixedDialog.' }
    )) {
        if ($designer -notmatch $shellRule.Pattern) { $issues.Add($shellRule.Message) }
    }
    $expectedTitle = [string]$model.entity.title
    $expectedTitlePattern = 'Text\s*=\s*"' + [regex]::Escape($expectedTitle) + '";'
    if ($designer -notmatch $expectedTitlePattern) { $issues.Add("El título '$expectedTitle' debe configurarse en la barra del formulario mediante Text.") }
    if ($designer -match 'lblGeneralTitle|lineGeneralTitle') { $issues.Add('No se permite repetir el título ni una línea de encabezado dentro del contenido.') }
    if ($designer -match 'btnGeneratedPrimary') { $issues.Add('No se permite btnGeneratedPrimary; use únicamente las acciones heredadas.') }

    foreach ($field in @($model.fields)) {
        $contract = Get-ControlContract -Field $field -Model $model
        $locationResult = Get-CandidateMatch -Text $designer -Names $contract.ControlNames -Property 'Location' -ValuePattern 'new Point\((\d+),\s*(\d+)\);' -Message "Falta una Location explícita y única para el campo $($contract.FieldName)."
        $sizeResult = Get-CandidateMatch -Text $designer -Names @($locationResult.Name) -Property 'Size' -ValuePattern 'new Size\((\d+),\s*(\d+)\);' -Message "Falta un Size explícito y único para $($locationResult.Name)."
        $labelLocationResult = Get-CandidateMatch -Text $designer -Names $contract.LabelNames -Property 'Location' -ValuePattern 'new Point\((\d+),\s*(\d+)\);' -Message "Falta una Location explícita y única para el label de $($contract.FieldName)."

        $editorX = [int]$locationResult.Match.Groups[1].Value
        $editorY = [int]$locationResult.Match.Groups[2].Value
        $editorWidth = [int]$sizeResult.Match.Groups[1].Value
        $editorHeight = [int]$sizeResult.Match.Groups[2].Value
        $labelX = [int]$labelLocationResult.Match.Groups[1].Value
        $labelY = [int]$labelLocationResult.Match.Groups[2].Value
        $expected = $expectedGeometry[$contract.FieldName]
        if ($editorX -ne $expected.EditorX -or $editorY -ne $expected.Y -or $editorWidth -ne $expected.Width) {
            $issues.Add("$($locationResult.Name) debe estar en ($($expected.EditorX),$($expected.Y)) con ancho $($expected.Width); está en ($editorX,$editorY) con ancho $editorWidth.")
        }
        if ($labelX -ne $expected.LabelX) { $issues.Add("$($labelLocationResult.Name) debe usar X=$($expected.LabelX) y usa X=$labelX.") }
        if ($editorHeight -ne $contract.ExpectedHeight) {
            $issues.Add("$($locationResult.Name) debe medir $($contract.ExpectedHeight) px de alto y mide $editorHeight px.")
        }
        if ($labelY -ne ($editorY + 3)) {
            $issues.Add("$($labelLocationResult.Name) debe quedar 3 px debajo del origen del editor; editor Y=$editorY, label Y=$labelY.")
        }

        $geometry.Add([pscustomobject]@{
            FieldName = $contract.FieldName
            ControlName = $locationResult.Name
            Y = $editorY
            Height = $editorHeight
        })
    }

    $rows = @($geometry | Group-Object Y | Sort-Object { [int]$_.Name })
    Assert-True ($rows.Count -gt 0) 'No se detectaron filas de edición para validar.'
    if ([int]$rows[0].Name -ne 26) {
        $issues.Add("La primera fila debe iniciar en Y=26 y comienza en Y=$($rows[0].Name).")
    }

    for ($index = 1; $index -lt $rows.Count; $index++) {
        $previous = $rows[$index - 1]
        $current = $rows[$index]
        $previousY = [int]$previous.Name
        $currentY = [int]$current.Name
        $previousHeight = [int](($previous.Group | Measure-Object Height -Maximum).Maximum)
        $expectedY = if ($previousHeight -gt 22) { $previousY + $previousHeight + 12 } else { $previousY + 28 }
        $previousControls = ($previous.Group | ForEach-Object ControlName) -join ', '
        $currentControls = ($current.Group | ForEach-Object ControlName) -join ', '
        if ($currentY -ne $expectedY) {
            $issues.Add("Cadencia vertical inválida entre [$previousControls] Y=$previousY y [$currentControls] Y=$currentY. Se esperaba Y=$expectedY.")
        }
    }

    Assert-True ($issues.Count -eq 0) ("Separación vertical inválida:`n- " + ($issues -join "`n- "))

    Write-Host "Diseño compacto aprobado: 870 px, columnas corporativas, $($rows.Count) filas, editores de 22 px, cadencia de 28 px y acciones heredadas."
    exit 0
}
catch {
    Write-Error $_
    exit 1
}
