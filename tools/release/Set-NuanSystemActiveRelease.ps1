[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$ReleaseDirectory,

    [Parameter(Mandatory = $true)]
    [string]$PointerPath
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$resolvedRelease = (Resolve-Path -LiteralPath $ReleaseDirectory).Path
$manifestPath = Join-Path $resolvedRelease "release-manifest.json"
$hashManifestPath = Join-Path $resolvedRelease "file-hashes.json"
if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf) -or
    -not (Test-Path -LiteralPath $hashManifestPath -PathType Leaf)) {
    throw "La release no tiene manifests verificables."
}

$manifest = Get-Content -Raw -LiteralPath $manifestPath | ConvertFrom-Json
$hashManifestSha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $hashManifestPath).Hash
$pointerDirectory = Split-Path -Parent ([System.IO.Path]::GetFullPath($PointerPath))
New-Item -ItemType Directory -Force -Path $pointerDirectory | Out-Null

$pointer = [pscustomobject]@{
    SchemaVersion = 1
    Release = $manifest.Release
    SourceCommit = $manifest.SourceCommit
    ReleaseDirectory = $resolvedRelease
    HashManifestSha256 = $hashManifestSha256
    SelectedAtUtc = [DateTime]::UtcNow.ToString("O")
}

$pointer |
    ConvertTo-Json -Depth 5 |
    Set-Content -LiteralPath $PointerPath -Encoding UTF8

[pscustomobject]@{
    Result = "Selected"
    Release = $pointer.Release
    SourceCommit = $pointer.SourceCommit
    HashManifestSha256 = $pointer.HashManifestSha256
} | ConvertTo-Json -Depth 5
