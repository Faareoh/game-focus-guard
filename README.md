# Game Focus Guard

Game Focus Guard is a small Windows utility for reducing the impact of games losing focus or pausing when they move to the background.

It is designed as a lightweight local tool for focus-protection experiments on Windows desktop games, including scenarios such as **Forza Horizon 6** and similar titles that react poorly to focus loss.

## About

The project combines a minimal WinForms desktop UI with a native hook component. It lets you toggle focus protection for a selected foreground window, rebind the global hotkey at runtime, and keep that hotkey across future launches.

The goal is practical local use:

- keep the tool small
- keep the workflow simple
- provide a directly usable Windows build

## Features

- Single-instance desktop app
- Minimal Windows UI
- Toggle focus protection with a button or hotkey
- Rebind the hotkey from the UI
- Hotkey persistence across launches
- Status display for:
  - current hotkey
  - hotkey registration state
  - candidate window
  - target window
  - hook state
  - enabled state

## Suitable Scenarios

Game Focus Guard is intended for local testing with games that may pause, stop responding to input, or behave badly when the active window changes.

Example use cases:

- games such as **Forza Horizon 6**
- games used on multi-monitor setups
- games where you want to reduce the impact of accidental focus changes

Behavior can still vary from game to game depending on rendering mode, input handling, and anti-tamper restrictions.

## Quick Start

If you are using a prepared release package:

1. Download the `win-x64` release zip.
2. Extract it to any local folder.
3. Run `FocusTool.Ui.exe` as administrator.
4. Bring the target game window to the foreground.
5. Use the hotkey or the main button to enable or disable focus protection.

Default hotkey:

```text
Ctrl + Shift + Alt + T
```

To change it:

1. Click `改热键`
2. Press a new key combination
3. The new hotkey is saved automatically
4. Press `Esc` while rebinding to cancel

If the app is already running, launching it again will activate the existing instance instead of opening a second copy.

## Build From Source

Requirements:

- Windows
- .NET SDK with Windows desktop support
- Visual Studio 2022 Build Tools or Visual Studio 2022 Community with the C++ toolchain

Build:

```powershell
dotnet build .\FocusTool.Ui\FocusTool.Ui.csproj
```

Notes:

- The recommended build entry point is `FocusTool.Ui.csproj`
- The UI project invokes `FocusTool.Hook\Build-Hook.ps1` after build
- The build script compiles the native hook DLL with the local Visual Studio C++ toolchain and copies it into the UI output directory

Run after build:

```powershell
.\FocusTool.Ui\bin\Debug\net10.0-windows\FocusTool.Ui.exe
```

## Project Layout

- `FocusTool.Ui/`
  - WinForms desktop UI
  - hotkey management
  - single-instance control
  - settings persistence
- `FocusTool.Hook/`
  - native hook component
  - build script for the native DLL

## Current Limitations

- This is still a prototype intended for local experimentation
- Only one target window is handled at a time
- It is not packaged as an installer
- Compatibility is not guaranteed across every game

## License

MIT License. See [LICENSE](./LICENSE).
