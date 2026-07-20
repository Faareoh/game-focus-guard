param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDir
)

$ErrorActionPreference = "Stop"

$OutputDir = $OutputDir.Trim().Trim('"').TrimEnd('\')

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$buildDir = Join-Path $scriptDir "build"
$sourceFile = Join-Path $scriptDir "focus_hook.cpp"
$vswhereCandidates = @(
    (Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"),
    (Join-Path $env:ProgramFiles "Microsoft Visual Studio\Installer\vswhere.exe")
)
$vswhere = $vswhereCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $vswhere) {
    throw "vswhere.exe was not found. Install Visual Studio 2022 or Build Tools with the C++ workload."
}

$visualStudioPath = & $vswhere -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
if (-not $visualStudioPath) {
    throw "No Visual Studio installation with the x64 C++ toolchain was found."
}

$vcVars = Join-Path ($visualStudioPath | Select-Object -First 1) "VC\Auxiliary\Build\vcvars64.bat"
if (-not (Test-Path $vcVars)) {
    throw "vcvars64.bat was not found: $vcVars"
}

New-Item -ItemType Directory -Force -Path $buildDir | Out-Null
New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null

$dllPath = Join-Path $buildDir "FocusTool.Hook.dll"
$cmd = "call `"$vcVars`" >nul && cl /nologo /std:c++20 /EHsc /LD /utf-8 /DUNICODE /D_UNICODE /DWIN32_LEAN_AND_MEAN `"$sourceFile`" /link /NOLOGO /DLL /OUT:`"$dllPath`" user32.lib comctl32.lib advapi32.lib gdi32.lib"

Push-Location $buildDir
try {
    cmd.exe /c $cmd
}
finally {
    Pop-Location
}

if ($LASTEXITCODE -ne 0) {
    throw "Native hook build failed with exit code $LASTEXITCODE"
}

Copy-Item -Force $dllPath $OutputDir
