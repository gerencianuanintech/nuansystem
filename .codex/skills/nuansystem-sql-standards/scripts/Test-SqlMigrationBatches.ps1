[CmdletBinding()]
param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string[]] $Path
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
[Console]::InputEncoding = [System.Text.UTF8Encoding]::new($false)
[Console]::OutputEncoding = [System.Text.UTF8Encoding]::new($false)
$OutputEncoding = [System.Text.UTF8Encoding]::new($false)

function Remove-SqlCommentsAndStrings {
    param([Parameter(Mandatory = $true)][AllowEmptyString()][string] $Text)

    $builder = [System.Text.StringBuilder]::new($Text.Length)
    $state = 'Code'

    for ($index = 0; $index -lt $Text.Length; $index++) {
        $current = $Text[$index]
        $next = if ($index + 1 -lt $Text.Length) { $Text[$index + 1] } else { [char]0 }

        switch ($state) {
            'Code' {
                if ($current -eq "'"[0]) {
                    [void]$builder.Append(' ')
                    $state = 'String'
                }
                elseif ($current -eq '-' -and $next -eq '-') {
                    [void]$builder.Append('  ')
                    $index++
                    $state = 'LineComment'
                }
                elseif ($current -eq '/' -and $next -eq '*') {
                    [void]$builder.Append('  ')
                    $index++
                    $state = 'BlockComment'
                }
                else {
                    [void]$builder.Append($current)
                }
            }
            'String' {
                if ($current -eq "'"[0] -and $next -eq "'"[0]) {
                    [void]$builder.Append('  ')
                    $index++
                }
                elseif ($current -eq "'"[0]) {
                    [void]$builder.Append(' ')
                    $state = 'Code'
                }
                elseif ($current -eq "`r" -or $current -eq "`n") {
                    [void]$builder.Append($current)
                }
                else {
                    [void]$builder.Append(' ')
                }
            }
            'LineComment' {
                if ($current -eq "`r" -or $current -eq "`n") {
                    [void]$builder.Append($current)
                    $state = 'Code'
                }
                else {
                    [void]$builder.Append(' ')
                }
            }
            'BlockComment' {
                if ($current -eq '*' -and $next -eq '/') {
                    [void]$builder.Append('  ')
                    $index++
                    $state = 'Code'
                }
                elseif ($current -eq "`r" -or $current -eq "`n") {
                    [void]$builder.Append($current)
                }
                else {
                    [void]$builder.Append(' ')
                }
            }
        }
    }

    return $builder.ToString()
}

function Get-LineNumber {
    param(
        [Parameter(Mandatory = $true)][string] $Text,
        [Parameter(Mandatory = $true)][int] $Index
    )

    if ($Index -le 0) { return 1 }
    return 1 + ([regex]::Matches($Text.Substring(0, $Index), "`n")).Count
}

$failures = [System.Collections.Generic.List[object]]::new()

foreach ($candidate in $Path) {
    $resolvedPaths = Resolve-Path -Path $candidate -ErrorAction Stop

    foreach ($resolvedPath in $resolvedPaths) {
        $source = Get-Content -Raw -LiteralPath $resolvedPath.Path
        $batchPattern = [regex]'(?im)^\s*GO(?:\s+\d+)?\s*(?:--.*)?$'
        $batchStart = 0
        $batchNumber = 1

        foreach ($separator in @($batchPattern.Matches($source)) + @($null)) {
            $batchEnd = if ($null -eq $separator) { $source.Length } else { $separator.Index }
            $batch = $source.Substring($batchStart, $batchEnd - $batchStart)
            $sanitized = Remove-SqlCommentsAndStrings -Text $batch
            $alterPattern = [regex]'(?is)\bALTER\s+TABLE\s+(?<table>(?:\[[^\]]+\]|[A-Za-z_][\w$#@]*)(?:\s*\.\s*(?:\[[^\]]+\]|[A-Za-z_][\w$#@]*))?)\s+ADD\s+(?!CONSTRAINT\b|PRIMARY\b|FOREIGN\b|UNIQUE\b|CHECK\b|DEFAULT\b)(?<column>\[[^\]]+\]|[A-Za-z_][\w$#@]*)\s+[^;]*;'

            foreach ($alter in $alterPattern.Matches($sanitized)) {
                $column = $alter.Groups['column'].Value.Trim('[', ']')
                $remainderStart = $alter.Index + $alter.Length
                $remainder = $sanitized.Substring($remainderStart)
                $referencePattern = [regex]::new('(?i)(?<![A-Za-z0-9_$#@])(?:\[' + [regex]::Escape($column) + '\]|' + [regex]::Escape($column) + ')(?![A-Za-z0-9_$#@])')
                $reference = $referencePattern.Match($remainder)

                if ($reference.Success) {
                    $absoluteReference = $batchStart + $remainderStart + $reference.Index
                    $failures.Add([pscustomobject]@{
                        File = $resolvedPath.Path
                        Batch = $batchNumber
                        Table = ($alter.Groups['table'].Value -replace '\s', '')
                        Column = $column
                        AddLine = Get-LineNumber -Text $source -Index ($batchStart + $alter.Index)
                        ReferenceLine = Get-LineNumber -Text $source -Index $absoluteReference
                    })
                }
            }

            if ($null -eq $separator) { break }
            $batchStart = $separator.Index + $separator.Length
            $batchNumber++
        }
    }
}

if ($failures.Count -gt 0) {
    $failures | Format-Table File, Batch, Table, Column, AddLine, ReferenceLine -AutoSize | Out-String | Write-Host
    Write-Host "ERROR: Se detectaron columnas agregadas y referenciadas estaticamente dentro del mismo lote SQL. Separe la evolucion y el consumo con GO."
    exit 1
}

Write-Host "Validacion aprobada: no se detectaron columnas nuevas consumidas estaticamente en el mismo lote."
exit 0
