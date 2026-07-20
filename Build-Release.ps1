param(
    [Parameter(Mandatory = $true)]
    [string]$Version,

    [string]$OutputRoot = "artifacts"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectPath = Join-Path $repoRoot "FocusTool.Ui\FocusTool.Ui.csproj"
$licensePath = Join-Path $repoRoot "LICENSE"

[xml]$project = Get-Content -LiteralPath $projectPath
$projectVersion = $project.Project.PropertyGroup.Version | Select-Object -First 1
if ($projectVersion -ne $Version) {
    throw "Requested version $Version does not match project version $projectVersion."
}

$outputRootPath = if ([IO.Path]::IsPathRooted($OutputRoot)) {
    [IO.Path]::GetFullPath($OutputRoot)
}
else {
    [IO.Path]::GetFullPath((Join-Path $repoRoot $OutputRoot))
}

if (-not $outputRootPath.StartsWith($repoRoot + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
    throw "OutputRoot must be inside the repository: $outputRootPath"
}

$packageName = "game-focus-guard-v$Version-win-x64"
$publishDir = Join-Path $outputRootPath "publish"
$packageDir = Join-Path $outputRootPath $packageName
$archivePath = Join-Path $outputRootPath "$packageName.zip"
$checksumPath = "$archivePath.sha256"

foreach ($path in @($publishDir, $packageDir)) {
    if (Test-Path -LiteralPath $path) {
        Remove-Item -LiteralPath $path -Recurse -Force
    }
}

foreach ($path in @($archivePath, $checksumPath)) {
    if (Test-Path -LiteralPath $path) {
        Remove-Item -LiteralPath $path -Force
    }
}

New-Item -ItemType Directory -Force -Path $publishDir, $packageDir | Out-Null

& dotnet publish $projectPath `
    -c Release `
    -r win-x64 `
    --self-contained true `
    -o $publishDir `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:EnableCompressionInSingleFile=true `
    -p:PublishTrimmed=false `
    -p:DebugType=None `
    -p:DebugSymbols=false

if ($LASTEXITCODE -ne 0) {
    throw "dotnet publish failed with exit code $LASTEXITCODE."
}

$requiredFiles = @("FocusTool.Ui.exe", "FocusTool.Hook.dll")
foreach ($file in $requiredFiles) {
    $source = Join-Path $publishDir $file
    if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
        throw "Required release component is missing: $source"
    }

    Copy-Item -LiteralPath $source -Destination $packageDir
}

Copy-Item -LiteralPath $licensePath -Destination $packageDir

Compress-Archive -Path (Join-Path $packageDir "*") -DestinationPath $archivePath -CompressionLevel Optimal

$hash = (Get-FileHash -LiteralPath $archivePath -Algorithm SHA256).Hash.ToLowerInvariant()
Set-Content -LiteralPath $checksumPath -Value "$hash  $([IO.Path]::GetFileName($archivePath))" -Encoding Ascii

[pscustomobject]@{
    Package = $archivePath
    Checksum = $checksumPath
    RuntimeFiles = $requiredFiles.Count
    TotalFiles = @(Get-ChildItem -LiteralPath $packageDir -File).Count
}
