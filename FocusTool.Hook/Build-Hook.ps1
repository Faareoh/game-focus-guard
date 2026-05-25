param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDir
)

$ErrorActionPreference = "Stop"

$OutputDir = $OutputDir.Trim().Trim('"').TrimEnd('\')

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$buildDir = Join-Path $scriptDir "build"
$sourceFile = Join-Path $scriptDir "focus_hook.cpp"
$vcVars = "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat"

if (-not (Test-Path $vcVars)) {
    throw "vcvars64.bat not found: $vcVars"
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
