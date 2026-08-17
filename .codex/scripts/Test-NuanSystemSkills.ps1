[CmdletBinding()]
param(
    [string]$RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot '../..')).Path,
    [string]$SkillsRoot = (Join-Path $RepositoryRoot '.codex/skills'),
    [switch]$Strict
)

$ErrorActionPreference = 'Stop'
$errors = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()

function Add-Issue {
    param(
        [ValidateSet('Error', 'Warning')]
        [string]$Level,
        [string]$Message
    )

    if ($Level -eq 'Error') {
        $errors.Add($Message)
    }
    else {
        $warnings.Add($Message)
    }
}

function Get-YamlScalar {
    param(
        [string]$Yaml,
        [string]$Name
    )

    $escapedName = [regex]::Escape($Name)
    $match = [regex]::Match($Yaml, "(?m)^\s*${escapedName}:\s*(?<value>.+?)\s*$")
    if (-not $match.Success) {
        return $null
    }

    $value = $match.Groups['value'].Value.Trim()
    if (($value.StartsWith('"') -and $value.EndsWith('"')) -or
        ($value.StartsWith("'") -and $value.EndsWith("'"))) {
        return $value.Substring(1, $value.Length - 2)
    }

    return $value
}

if (-not (Test-Path -LiteralPath $SkillsRoot -PathType Container)) {
    throw "Skills root not found: $SkillsRoot"
}

$skillFiles = @(Get-ChildItem -LiteralPath $SkillsRoot -Directory |
    ForEach-Object { Get-Item -LiteralPath (Join-Path $_.FullName 'SKILL.md') -ErrorAction SilentlyContinue } |
    Sort-Object FullName)

$knownSkills = @{}
foreach ($skillFile in $skillFiles) {
    $knownSkills[$skillFile.Directory.Name] = $skillFile.FullName
}

foreach ($skillFile in $skillFiles) {
    $folderName = $skillFile.Directory.Name
    $relativeSkill = [System.IO.Path]::GetRelativePath($RepositoryRoot, $skillFile.FullName)
    $content = Get-Content -LiteralPath $skillFile.FullName -Raw
    $frontmatter = [regex]::Match($content, '(?s)\A---\r?\n(?<yaml>.*?)\r?\n---(?:\r?\n|$)')

    if (-not $frontmatter.Success) {
        Add-Issue Error "${relativeSkill}: missing or malformed YAML frontmatter."
        continue
    }

    $yaml = $frontmatter.Groups['yaml'].Value
    $name = Get-YamlScalar -Yaml $yaml -Name 'name'
    $description = Get-YamlScalar -Yaml $yaml -Name 'description'

    if ([string]::IsNullOrWhiteSpace($name)) {
        Add-Issue Error "${relativeSkill}: missing frontmatter name."
    }
    elseif ($name -ne $folderName) {
        Add-Issue Error "${relativeSkill}: name '$name' does not match folder '$folderName'."
    }
    elseif ($name -notmatch '^[a-z0-9]+(?:-[a-z0-9]+)*$') {
        Add-Issue Error "${relativeSkill}: name must use lowercase kebab-case."
    }

    if ([string]::IsNullOrWhiteSpace($description)) {
        Add-Issue Error "${relativeSkill}: missing frontmatter description."
    }
    else {
        $rawDescription = [regex]::Match($yaml, '(?m)^description:\s*(?<value>.+?)\s*$').Groups['value'].Value.Trim()
        $isQuoted = ($rawDescription.StartsWith('"') -and $rawDescription.EndsWith('"')) -or
            ($rawDescription.StartsWith("'") -and $rawDescription.EndsWith("'"))
        if (-not $isQuoted -and $rawDescription -match ':\s') {
            Add-Issue Error "${relativeSkill}: quote description because an unquoted ': ' is invalid YAML."
        }
        if ($description.Length -gt 700) {
            Add-Issue Warning "${relativeSkill}: description is unusually long ($($description.Length) characters)."
        }
    }

    if ($content -match '[\uFFFD]|Ã.|Â.') {
        Add-Issue Error "${relativeSkill}: possible mojibake or replacement character."
    }

    foreach ($reference in [regex]::Matches($content, '\$nuansystem-[a-z0-9-]+')) {
        $referencedName = $reference.Value.Substring(1)
        if (-not $knownSkills.ContainsKey($referencedName)) {
            Add-Issue Error "${relativeSkill}: references unknown skill '$referencedName'."
        }
    }

    foreach ($link in [regex]::Matches($content, '\[[^\]]+\]\((?<target>[^)]+)\)')) {
        $target = $link.Groups['target'].Value.Trim()
        if ($target -match '^(?:https?://|mailto:|#)') {
            continue
        }

        $target = $target.Split('#')[0]
        $resolvedTarget = Join-Path $skillFile.Directory.FullName $target
        if (-not (Test-Path -LiteralPath $resolvedTarget)) {
            Add-Issue Error "${relativeSkill}: broken local link '$target'."
        }
    }

    foreach ($pathMatch in [regex]::Matches($content, '`(?<path>(?:src|database|docs|tests|\.codex)/[^`\r\n]+)`')) {
        $candidate = $pathMatch.Groups['path'].Value.Trim().Replace('/', [System.IO.Path]::DirectorySeparatorChar)
        if ($candidate -match '[*?<>|{}]' -or $candidate.Contains('...') -or $candidate -match '^\.codex/skills/\$') {
            continue
        }

        $resolvedCandidate = Join-Path $RepositoryRoot $candidate
        if (-not (Test-Path -LiteralPath $resolvedCandidate)) {
            Add-Issue Error "${relativeSkill}: repository path not found '$($pathMatch.Groups['path'].Value)'."
        }
    }

    $agentFile = Join-Path $skillFile.Directory.FullName 'agents/openai.yaml'
    $isDeprecated = $description -match '(?i)deprecated'
    if (-not $isDeprecated -and -not (Test-Path -LiteralPath $agentFile)) {
        Add-Issue Warning "${relativeSkill}: missing agents/openai.yaml metadata."
    }
    elseif (Test-Path -LiteralPath $agentFile) {
        $agentContent = Get-Content -LiteralPath $agentFile -Raw
        $displayName = Get-YamlScalar -Yaml $agentContent -Name 'display_name'
        $shortDescription = Get-YamlScalar -Yaml $agentContent -Name 'short_description'
        $defaultPrompt = Get-YamlScalar -Yaml $agentContent -Name 'default_prompt'

        if ([string]::IsNullOrWhiteSpace($displayName)) {
            Add-Issue Error "${relativeSkill}: agents/openai.yaml is missing interface.display_name."
        }
        if ([string]::IsNullOrWhiteSpace($shortDescription) -or
            $shortDescription.Length -lt 25 -or $shortDescription.Length -gt 64) {
            Add-Issue Error "${relativeSkill}: short_description must contain 25-64 characters."
        }
        $skillMention = '$' + $folderName
        if ([string]::IsNullOrWhiteSpace($defaultPrompt) -or -not $defaultPrompt.Contains($skillMention)) {
            Add-Issue Error "${relativeSkill}: default_prompt must mention '`$$folderName'."
        }
    }

    $referencesDirectory = Join-Path $skillFile.Directory.FullName 'references'
    if (Test-Path -LiteralPath $referencesDirectory) {
        foreach ($referenceFile in Get-ChildItem -LiteralPath $referencesDirectory -File -Filter '*.md') {
            $referenceContent = Get-Content -LiteralPath $referenceFile.FullName -Raw
            $lineCount = @($referenceContent -split "`r?`n").Count
            if ($lineCount -gt 100 -and $referenceContent -notmatch '(?im)^##\s+(?:Contents|Table of contents|Contenido|Tabla de contenido)\s*$') {
                $relativeReference = [System.IO.Path]::GetRelativePath($RepositoryRoot, $referenceFile.FullName)
                Add-Issue Warning "${relativeReference}: $lineCount lines without a table of contents."
            }
        }
    }
}

foreach ($warning in $warnings) {
    Write-Warning $warning
}
foreach ($validationError in $errors) {
    Write-Error $validationError -ErrorAction Continue
}

Write-Host "Skills checked: $($skillFiles.Count); errors: $($errors.Count); warnings: $($warnings.Count)."

if ($errors.Count -gt 0 -or ($Strict -and $warnings.Count -gt 0)) {
    exit 1
}

exit 0
