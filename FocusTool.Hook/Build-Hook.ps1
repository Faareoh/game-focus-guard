param(
    [Parameter(Mandatory = $true)]
    [string]$OutputDir
)

$ErrorActionPreference = "Stop"

$OutputDir = $OutputDir.Trim().Trim('"').TrimEnd('\')

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$buildDir = Join-Path $scriptDir "build"
$sourceFile = Join-Path $scriptDir "focus_hook.cpp"

# Try common VS 2022 installation layouts in order; BuildTools is the fallback.
$vcVarsCandidates = @(
    "C:\Program Files\Microsoft Visual Studio\2022\Community\VC\Auxiliary\Build\vcvars64.bat",
    "C:\Program Files\Microsoft Visual Studio\2022\Professional\VC\Auxiliary\Build\vcvars64.bat",
    "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\VC\Auxiliary\Build\vcvars64.bat",
    "C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\VC\Auxiliary\Build\vcvars64.bat"
)

$vcVars = $vcVarsCandidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $vcVars) {
    throw "vcvars64.bat introuvable. Chemins essayés :`n$($vcVarsCandidates -join "`n")"
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
