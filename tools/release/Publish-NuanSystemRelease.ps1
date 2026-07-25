[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("pilot1", "pilot2")]
    [string]$Pilot,

    [Parameter(Mandatory = $true)]
    [string]$OutputRoot,

    [string]$BaseVersion = "7.1.0-dotnet10",

    [switch]$NoRestore
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

function Get-OptionalJsonValue {
    param(
        [AllowNull()]
        [object]$Object,

        [Parameter(Mandatory = $true)]
        [string[]]$Path
    )

    $current = $Object
    foreach ($segment in $Path) {
        if ($null -eq $current) {
            return $null
        }

        $property = $current.PSObject.Properties[$segment]
        if ($null -eq $property) {
            return $null
        }

        $current = $property.Value
    }

    return $current
}

$repositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "..\..")).Path
$gitStatus = @(& git -C $repositoryRoot status --porcelain)
if ($LASTEXITCODE -ne 0) {
    throw "No fue posible comprobar el estado Git."
}
if ($gitStatus.Count -ne 0) {
    throw "La publicacion exige un working tree limpio y un commit inmutable."
}

$sourceCommit = (& git -C $repositoryRoot rev-parse HEAD).Trim()
if ($LASTEXITCODE -ne 0 -or $sourceCommit -notmatch "^[0-9a-f]{40}$") {
    throw "No fue posible resolver el commit de origen."
}

$shortCommit = $sourceCommit.Substring(0, 8)
$informationalVersion = "$BaseVersion-$Pilot+$shortCommit"
$fileVersion = if ($Pilot -eq "pilot1") { "7.1.0.1" } else { "7.1.0.2" }
$assemblyVersion = "7.1.0.0"
$resolvedOutputRoot = [System.IO.Path]::GetFullPath($OutputRoot)
$releaseDirectory = Join-Path $resolvedOutputRoot $informationalVersion

if (Test-Path -LiteralPath $releaseDirectory) {
    throw "La release ya existe y es inmutable: $informationalVersion"
}

$projects = @(
    [pscustomobject]@{
        Name = "NuanSystem.Api"
        Project = "src\Backend\NuanSystem.Api\NuanSystem.Api.csproj"
        EntryPoint = "NuanSystem.Api.dll"
    },
    [pscustomobject]@{
        Name = "NuanSystem.SyncWorker"
        Project = "src\Backend\NuanSystem.SyncWorker\NuanSystem.SyncWorker.csproj"
        EntryPoint = "NuanSystem.SyncWorker.dll"
    },
    [pscustomobject]@{
        Name = "NuanSystem.MasterBranchSyncWorker"
        Project = "src\Backend\NuanSystem.MasterBranchSyncWorker\NuanSystem.MasterBranchSyncWorker.csproj"
        EntryPoint = "NuanSystem.MasterBranchSyncWorker.dll"
    },
    [pscustomobject]@{
        Name = "NuanSystem.SriWorker"
        Project = "src\Backend\NuanSystem.SriWorker\NuanSystem.SriWorker.csproj"
        EntryPoint = "NuanSystem.SriWorker.dll"
    },
    [pscustomobject]@{
        Name = "NuanSystem.WinForms"
        Project = "src\Frontend\NuanSystem.WinForms\NuanSystem.WinForms.csproj"
        EntryPoint = "NuanSystem.WinForms.dll"
    }
)

New-Item -ItemType Directory -Force -Path $releaseDirectory | Out-Null

try {
    if (-not $NoRestore) {
        & dotnet restore (Join-Path $repositoryRoot "NuanSystem.sln") --runtime win-x64
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet restore fallo."
        }
    }

    $projectResults = @()
    foreach ($project in $projects) {
        $projectPath = Join-Path $repositoryRoot $project.Project
        $projectOutput = Join-Path $releaseDirectory $project.Name

        & dotnet publish $projectPath `
            --configuration Release `
            --runtime win-x64 `
            --self-contained false `
            --no-restore `
            --output $projectOutput `
            "-p:AssemblyVersion=$assemblyVersion" `
            "-p:FileVersion=$fileVersion" `
            "-p:InformationalVersion=$informationalVersion" `
            "-p:IncludeSourceRevisionInInformationalVersion=false" `
            "-p:PublishTrimmed=false" `
            "-p:PublishSingleFile=false"
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet publish fallo para $($project.Name)."
        }

        Get-ChildItem -LiteralPath $projectOutput -Filter "appsettings.Development.json" -File |
            Remove-Item -Force

        if ($project.Name -eq "NuanSystem.SyncWorker") {
            $syncSettingsPath = Join-Path $projectOutput "appsettings.json"
            $syncSettings = Get-Content -Raw -LiteralPath $syncSettingsPath | ConvertFrom-Json
            $syncSettings.Worker.Enabled = $false
            $syncSettings.Retry.Enabled = $false
            $syncSettings |
                ConvertTo-Json -Depth 100 |
                Set-Content -LiteralPath $syncSettingsPath -Encoding UTF8
        }

        $entryPointPath = Join-Path $projectOutput $project.EntryPoint
        if (-not (Test-Path -LiteralPath $entryPointPath -PathType Leaf)) {
            throw "No existe el entry point publicado de $($project.Name)."
        }

        $versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($entryPointPath)
        if ($versionInfo.ProductVersion -ne $informationalVersion) {
            throw "La version informativa de $($project.Name) no coincide con la release."
        }

        $projectResults += [pscustomobject]@{
            Name = $project.Name
            Project = $project.Project.Replace("\", "/")
            Output = $project.Name
            EntryPoint = $project.EntryPoint
            ProductVersion = $versionInfo.ProductVersion
            FileVersion = $versionInfo.FileVersion
            FileCount = @(Get-ChildItem -LiteralPath $projectOutput -Recurse -File).Count
        }
    }

    $forbiddenNames = @(
        @(Get-ChildItem -LiteralPath $releaseDirectory -Recurse -File |
            Where-Object {
                $_.Name -ieq "appsettings.Local.json" -or
                $_.Name -ieq ".env" -or
                $_.Extension -in @(".pfx", ".p12", ".pem", ".key", ".cer", ".crt", ".bak", ".log")
            })
    )
    if ($forbiddenNames.Count -ne 0) {
        throw "La release contiene archivos locales, secretos, certificados, logs o respaldos."
    }

    $configurationChecks = @()
    foreach ($settingsFile in @(Get-ChildItem -LiteralPath $releaseDirectory -Recurse -File -Filter "appsettings*.json")) {
        $settings = Get-Content -Raw -LiteralPath $settingsFile.FullName | ConvertFrom-Json
        $sensitiveValues = @(@(
            [string](Get-OptionalJsonValue -Object $settings -Path @("ConnectionStrings", "SqlServerAdmin")),
            [string](Get-OptionalJsonValue -Object $settings -Path @("Security", "EncryptionKey")),
            [string](Get-OptionalJsonValue -Object $settings -Path @("Jwt", "SigningKey"))
        ) | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })

        if ($sensitiveValues.Count -ne 0) {
            throw "La configuracion publicada contiene un valor sensible no vacio."
        }

        $configurationChecks += [pscustomobject]@{
            Path = $settingsFile.FullName.Substring($releaseDirectory.Length + 1).Replace("\", "/")
            SensitiveValuesPresent = $false
        }
    }

    $syncConfig = Get-Content -Raw -LiteralPath (Join-Path $releaseDirectory "NuanSystem.SyncWorker\appsettings.json") | ConvertFrom-Json
    $masterBranchConfig = Get-Content -Raw -LiteralPath (Join-Path $releaseDirectory "NuanSystem.MasterBranchSyncWorker\appsettings.json") | ConvertFrom-Json
    $sriConfig = Get-Content -Raw -LiteralPath (Join-Path $releaseDirectory "NuanSystem.SriWorker\appsettings.json") | ConvertFrom-Json
    $sriProductionConfig = Get-Content -Raw -LiteralPath (Join-Path $releaseDirectory "NuanSystem.SriWorker\appsettings.Production.json") | ConvertFrom-Json

    if ($syncConfig.Worker.Enabled -ne $false -or $syncConfig.Retry.Enabled -ne $false) {
        throw "El artefacto SyncWorker no quedo deshabilitado."
    }
    if ($masterBranchConfig.MasterBranchSyncWorker.Enabled -ne $false) {
        throw "El artefacto MasterBranchSyncWorker no quedo deshabilitado."
    }
    if ($sriConfig.SriWorker.Enabled -ne $false -or $sriProductionConfig.SriWorker.Enabled -ne $false) {
        throw "El artefacto SriWorker no quedo deshabilitado."
    }

    $textFiles = @(Get-ChildItem -LiteralPath $releaseDirectory -Recurse -File |
        Where-Object { $_.Extension -in @(".json", ".config", ".txt", ".xml") })
    foreach ($textFile in $textFiles) {
        $content = Get-Content -Raw -LiteralPath $textFile.FullName
        if ($content -match "<claveAcceso>\s*\d{49}\s*</claveAcceso>" -or
            $content -match '"AccessKey"\s*:\s*"\d{49}"') {
            throw "La release contiene una clave SRI completa."
        }
    }

    $dependencies = @{}
    foreach ($depsFile in @(Get-ChildItem -LiteralPath $releaseDirectory -Recurse -File -Filter "*.deps.json")) {
        $deps = Get-Content -Raw -LiteralPath $depsFile.FullName | ConvertFrom-Json
        foreach ($library in $deps.libraries.PSObject.Properties) {
            $dependencies[$library.Name] = [pscustomobject]@{
                Identity = $library.Name
                Type = $library.Value.type
                Serviceable = [bool]$library.Value.serviceable
            }
        }
    }

    $dependencyInventoryPath = Join-Path $releaseDirectory "dependency-inventory.json"
    [pscustomobject]@{
        Release = $informationalVersion
        Libraries = @($dependencies.Values | Sort-Object Identity)
    } |
        ConvertTo-Json -Depth 10 |
        Set-Content -LiteralPath $dependencyInventoryPath -Encoding UTF8

    $manifestPath = Join-Path $releaseDirectory "release-manifest.json"
    [pscustomobject]@{
        SchemaVersion = 1
        Release = $informationalVersion
        Pilot = $Pilot
        SourceCommit = $sourceCommit
        ShortCommit = $shortCommit
        Configuration = "Release"
        RuntimeIdentifier = "win-x64"
        DeploymentMode = "framework-dependent"
        SelfContained = $false
        PublishTrimmed = $false
        PublishSingleFile = $false
        AssemblyVersion = $assemblyVersion
        FileVersion = $fileVersion
        InformationalVersion = $informationalVersion
        DotnetSdk = (& dotnet --version).Trim()
        CreatedAtUtc = [DateTime]::UtcNow.ToString("O")
        Projects = $projectResults
        SafeConfiguration = [pscustomobject]@{
            SyncWorkerEnabled = [bool]$syncConfig.Worker.Enabled
            SyncRetryEnabled = [bool]$syncConfig.Retry.Enabled
            MasterBranchSyncWorkerEnabled = [bool]$masterBranchConfig.MasterBranchSyncWorker.Enabled
            SriWorkerEnabled = [bool]$sriConfig.SriWorker.Enabled
            SriWorkerProductionEnabled = [bool]$sriProductionConfig.SriWorker.Enabled
            LocalSettingsIncluded = $false
            SensitiveValuesPresent = $false
        }
        ConfigurationFiles = $configurationChecks
    } |
        ConvertTo-Json -Depth 20 |
        Set-Content -LiteralPath $manifestPath -Encoding UTF8

    $hashManifestPath = Join-Path $releaseDirectory "file-hashes.json"
    $hashes = @(
        Get-ChildItem -LiteralPath $releaseDirectory -Recurse -File |
            Where-Object { $_.FullName -ne $hashManifestPath } |
            ForEach-Object {
                [pscustomobject]@{
                    Path = $_.FullName.Substring($releaseDirectory.Length + 1).Replace("\", "/")
                    Sha256 = (Get-FileHash -Algorithm SHA256 -LiteralPath $_.FullName).Hash
                    SizeBytes = $_.Length
                }
            } |
            Sort-Object Path
    )
    [pscustomobject]@{
        Algorithm = "SHA256"
        Release = $informationalVersion
        Files = $hashes
    } |
        ConvertTo-Json -Depth 10 |
        Set-Content -LiteralPath $hashManifestPath -Encoding UTF8

    $releaseHash = (Get-FileHash -Algorithm SHA256 -LiteralPath $hashManifestPath).Hash
    [pscustomobject]@{
        Result = "Validated"
        Release = $informationalVersion
        Directory = $releaseDirectory
        SourceCommit = $sourceCommit
        ProjectCount = $projectResults.Count
        FileCount = $hashes.Count
        DependencyCount = $dependencies.Count
        HashManifestSha256 = $releaseHash
        SecretsDetected = $false
        WorkersEnabled = $false
    } | ConvertTo-Json -Depth 5
}
catch {
    if (Test-Path -LiteralPath $releaseDirectory) {
        Remove-Item -LiteralPath $releaseDirectory -Recurse -Force
    }
    throw
}
