[CmdletBinding()]
param(
    [string] $SkillsRoot,
    [string] $QuickValidatorPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [System.Text.UTF8Encoding]::new($false)

if (-not $SkillsRoot) {
    $SkillsRoot = Split-Path (Split-Path $PSScriptRoot -Parent) -Parent
}
if (-not $QuickValidatorPath) {
    $codexRoot = if ($env:CODEX_HOME) { $env:CODEX_HOME } else { Join-Path $env:USERPROFILE '.codex' }
    $QuickValidatorPath = Join-Path $codexRoot 'skills\.system\skill-creator\scripts\quick_validate.py'
}

$SkillsRoot = (Resolve-Path -LiteralPath $SkillsRoot).Path
$QuickValidatorPath = (Resolve-Path -LiteralPath $QuickValidatorPath).Path
$skills = @(Get-ChildItem -LiteralPath $SkillsRoot -Directory |
    Where-Object { Test-Path -LiteralPath (Join-Path $_.FullName 'SKILL.md') } |
    Sort-Object Name)
$issues = [System.Collections.Generic.List[string]]::new()
$results = [System.Collections.Generic.List[object]]::new()
$skillNames = @($skills.Name)

foreach ($skill in $skills) {
    $skillFile = Join-Path $skill.FullName 'SKILL.md'
    $validationOutput = & python -X utf8 $QuickValidatorPath $skill.FullName 2>&1 | Out-String
    $validationExitCode = $LASTEXITCODE
    if ($validationExitCode -ne 0) {
        $issues.Add("$($skill.Name): quick_validate fallo: $($validationOutput.Trim())")
    }

    $lines = @(Get-Content -LiteralPath $skillFile)
    if ($lines.Count -gt 500) {
        $issues.Add("$($skill.Name): SKILL.md supera 500 lineas ($($lines.Count)).")
    }

    $raw = [System.IO.File]::ReadAllText($skillFile, [System.Text.UTF8Encoding]::new($false, $true))
    foreach ($match in [regex]::Matches($raw, '\$(?<skill>nuansystem-[a-z0-9-]+)')) {
        $target = $match.Groups['skill'].Value
        if ($target -notin $skillNames) {
            $issues.Add("$($skill.Name): dependencia inexistente `$$target.")
        }
    }

    $agentFile = Join-Path $skill.FullName 'agents\openai.yaml'
    if (-not (Test-Path -LiteralPath $agentFile)) {
        $issues.Add("$($skill.Name): falta agents/openai.yaml.")
    }
    else {
        $agentText = [System.IO.File]::ReadAllText($agentFile, [System.Text.UTF8Encoding]::new($false, $true))
        foreach ($field in @('display_name', 'short_description', 'default_prompt')) {
            $pattern = '(?m)^\s*' + [regex]::Escape($field) + ':\s*"(?<value>.*)"\s*$'
            $fieldMatch = [regex]::Match($agentText, $pattern)
            if (-not $fieldMatch.Success) {
                $issues.Add("$($skill.Name): falta o no esta entre comillas interface.$field.")
                continue
            }
            $value = $fieldMatch.Groups['value'].Value
            if ($field -eq 'short_description' -and ($value.Length -lt 25 -or $value.Length -gt 64)) {
                $issues.Add("$($skill.Name): short_description debe tener 25-64 caracteres ($($value.Length)).")
            }
            if ($field -eq 'default_prompt' -and -not $value.Contains('$' + $skill.Name)) {
                $issues.Add("$($skill.Name): default_prompt no menciona `$$($skill.Name).")
            }
        }
    }

    $results.Add([pscustomobject]@{
        Skill = $skill.Name
        QuickValidate = if ($validationExitCode -eq 0) { 'PASS' } else { 'FAIL' }
        Lines = $lines.Count
        Agent = if (Test-Path -LiteralPath $agentFile) { 'PASS' } else { 'FAIL' }
    })
}

foreach ($reference in Get-ChildItem -LiteralPath $SkillsRoot -Recurse -File -Filter '*.md' |
    Where-Object { $_.FullName -match '\\references\\' }) {
    $lines = @(Get-Content -LiteralPath $reference.FullName)
    if ($lines.Count -le 100) { continue }
    $head = ($lines | Select-Object -First 35) -join "`n"
    if ($head -notmatch '(?im)^##?\s+(Contents|Contenido|Table of contents|Indice|Índice)\b') {
        $issues.Add("$($reference.FullName): referencia de $($lines.Count) lineas sin indice.")
    }
}

$results | Format-Table -AutoSize | Out-Host
if ($issues.Count -gt 0) {
    $issues | Sort-Object -Unique | ForEach-Object { Write-Error $_ }
    throw "Validacion de skills rechazada: $($issues.Count) hallazgo(s)."
}

Write-Host "Validacion aprobada: $($skills.Count) skills, UTF-8, frontmatter, dependencias, agents/openai.yaml, longitud e indices."
