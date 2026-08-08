# Game Focus Guard

Search Keywords: prevent game focus loss tool, game focus loss tool, game focus guard, game anti focus loss, game window focus loss, prevent game background pause, game background pause, game focus protection tool, game focus protection, keep game window focused, game window focus protection, game pauses when unfocused, game pauses in background, game pauses when switching windows, Forza Horizon 6 focus loss, FH6 pause when unfocused, multi-monitor game focus, global hotkey focus lock, window always on top, Game Focus Guard. 防止游戏失焦工具, 游戏失焦工具, 游戏防失焦, 游戏窗口失焦, 防止游戏切后台暂停, 游戏后台暂停, 游戏焦点保护工具, 游戏焦点保护, 保持游戏窗口焦点, 游戏窗口焦点保护, 游戏一失焦就暂停, 游戏切后台暂停怎么办, 游戏切窗口就暂停怎么解决, 地平线6失焦暂停, 地平线6切后台暂停, 地平线6切窗口暂停, 多显示器游戏防失焦, 全局热键锁定焦点, 游戏窗口置顶, 防止游戏失焦工具.

`Game Focus Guard` is a local Windows utility designed to reduce the impact of games losing focus, pausing in the background, or behaving unexpectedly after focus changes.

It is intended for games that need to remain logically active, including **Forza Horizon 6**, multi-monitor gaming setups, and desktop games that are easily interrupted by accidental clicks, window switching, or background activity.

[简体中文](./README.md)

## About

The project combines a minimal WinForms desktop interface with a native hook component to provide focus protection for a selected target window.

The current version supports:

- Single-instance operation
- Toggling focus protection from the main button or a global hotkey
- Rebinding the global hotkey from the interface
- Saving and restoring the hotkey across launches
- An optional always-on-top main window setting that persists across launches
- Status information for the hotkey, candidate window, target window, hooks, and protection state

## Use Cases

The utility is intended for local testing and everyday scenarios such as:

- Games such as **Forza Horizon 6** that are sensitive to focus changes
- Multi-monitor environments where accidental window switching is common
- Games where focus loss, background pauses, or interrupted input should be reduced

Actual behavior can vary depending on the game's rendering mode, input handling, anti-cheat system, or anti-tamper restrictions.

## Quick Start

When using a prepared release package:

1. Download the `win-x64` archive.
2. Extract it to a local directory.
3. Run `FocusTool.Ui.exe` as administrator.
4. Bring the target game window to the foreground.
5. Use the global hotkey or the main button to enable or disable focus protection.

The default hotkey is:

```text
Ctrl + Shift + Alt + T
```

To change the hotkey:

1. Click `改热键`.
2. Press a new key combination.
3. The new hotkey takes effect immediately and is saved automatically.
4. Press `Esc` while rebinding to cancel.

If the application is already running, launching it again activates the existing instance instead of opening another one.

Enable `始终置顶` to keep the utility window above other normal windows. This setting is saved automatically.

## Build from Source

Requirements:

- Windows
- A .NET SDK with Windows desktop support
- Visual Studio 2022 Build Tools or Visual Studio 2022 Community with the C++ toolchain installed

Build the UI project:

```powershell
dotnet build .\FocusTool.Ui\FocusTool.Ui.csproj
```

Notes:

- `FocusTool.Ui.csproj` is the recommended build entry point.
- The UI project automatically invokes `FocusTool.Hook\Build-Hook.ps1` after building.
- The build script uses the installed Visual Studio C++ toolchain to compile the native hook DLL and copy it to the UI output directory.

Run the application after building:

```powershell
.\FocusTool.Ui\bin\Debug\net10.0-windows\FocusTool.Ui.exe
```

## Build the Portable Package

The repository provides a unified release script:

```powershell
.\Build-Release.ps1 -Version 0.4.0
```

The script creates a self-contained Windows x64 single-file application and packages the native `FocusTool.Hook.dll`, which must remain as a separate runtime file. The final archive contains only:

- `FocusTool.Ui.exe`
- `FocusTool.Hook.dll`
- `LICENSE`

Users do not need to install the .NET Runtime. Each version tag is built by GitHub Actions from the same tagged source used by the GitHub-generated source archives.

## Repository Layout

- `FocusTool.Ui/`
  - WinForms desktop interface
  - Hotkey management
  - Single-instance coordination
  - Settings persistence
- `FocusTool.Hook/`
  - Native hook component
  - Native DLL build script

## Current Limitations

- The project is still a prototype intended primarily for local experimentation and real-world validation.
- Only one target window can be handled at a time.
- No installer is currently provided.
- Compatibility is not guaranteed for every game.

## License

MIT License. See [LICENSE](./LICENSE).
