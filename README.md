# Game Focus Guard

A minimal Windows utility that prevents games from losing focus or pausing when they move to the background.

Built for scenarios like **Forza Horizon 6**, multi-monitor setups, and any game that reacts poorly to focus loss — accidental clicks, alt-tabs, or background activity that interrupts gameplay.

## Features

- **Focus protection** — intercepts window deactivation messages so the target game stays active
- **Global hotkey** — toggle protection from anywhere with a configurable shortcut (default: `Ctrl+Shift+Alt+T`)
- **System tray** — closing the window minimizes to tray; the app keeps running silently in the background
- **Hotkey rebinding** — change the shortcut directly in the UI; saved automatically
- **Single instance** — launching the app twice brings the existing window back to focus
- **Localization** — UI available in English, French and Chinese (saved across sessions)

## Quick Start

1. Download the `win-x64` archive from the [Releases](../../releases) page
2. Extract to any local folder
3. Run `FocusTool.Ui.exe` **as Administrator**
4. Switch to your target game window
5. Press `Ctrl+Shift+Alt+T` (or click the main button) to toggle focus protection

> **Why Administrator?** The app uses low-level Windows hooks (`SetWindowsHookEx`) and a global hotkey (`RegisterHotKey`). These require elevated privileges when targeting protected processes.

### Changing the hotkey

1. Click **Change Hotkey**
2. Press the new key combination
3. The hotkey takes effect immediately and is saved automatically
4. Press `Esc` during rebind to cancel

### System tray

Clicking the **×** button minimizes the app to the system tray — it keeps running and protecting the target window. To restore the window, double-click the tray icon or right-click → **Show window**. To exit completely, right-click → **Quit**.

## How It Works

The app loads a native C++ hook DLL (`FocusTool.Hook.dll`) and installs Windows hooks on the target game's thread:

| Hook | Purpose |
|---|---|
| `WH_CALLWNDPROC` | Intercepts focus-loss messages (`WM_ACTIVATE`, `WM_KILLFOCUS`…) and installs a one-shot subclass to swallow them |
| `WH_KEYBOARD_LL` | Suppresses Alt+Tab while the target window is in the foreground |

When you activate protection, the app also calls `AllowSetForegroundWindow` to let the game reclaim focus if another window tries to steal it.

## Limitations

- Prototype-level quality — suitable for personal use and local experimentation
- One target window at a time
- No installer — extract and run
- Compatibility varies by game (rendering mode, anti-cheat, input handling)
- Some games with kernel-level anti-cheat (EAC, BattlEye) may detect or block the hook injection

## Building from Source

**Requirements:**
- Windows 10 x64 or later
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PowerShell 7+](https://github.com/PowerShell/PowerShell) (`pwsh`)
- Visual Studio 2022 with the **C++ build tools** workload  
  (Community, Professional, Enterprise or Build Tools edition)

**Build:**
```powershell
dotnet build .\FocusTool.Ui\FocusTool.Ui.csproj
```

This compiles the C# UI and automatically triggers `Build-Hook.ps1`, which locates `vcvars64.bat`, compiles `focus_hook.cpp` with MSVC, and copies the DLL to the output folder.

**Run** (must be Administrator):
```powershell
.\FocusTool.Ui\bin\Debug\net10.0-windows\FocusTool.Ui.exe
```

**Publish** (self-contained, redistributable):
```powershell
dotnet publish .\FocusTool.Ui\FocusTool.Ui.csproj -c Release -r win-x64 --self-contained
```

## Repository Structure

```
game-focus-guard/
├── FocusTool.Ui/          # WinForms .NET 10 application
│   ├── Form1.cs/.Designer.cs   — main UI + language selector + tray
│   ├── HookController.cs       — loads the native DLL, installs hooks
│   ├── HotKeyService.cs        — global hotkey registration
│   ├── Strings.cs              — all localized strings (EN/FR/ZH)
│   ├── Language.cs             — Language enum
│   ├── LanguageStore.cs        — language preference persistence
│   ├── HotKeySettingsStore.cs  — hotkey settings (JSON)
│   └── generate-icon.ps1       — generates icon.ico via GDI+
└── FocusTool.Hook/        # Native C++20 hook DLL
    ├── focus_hook.cpp          — hook procedures
    └── Build-Hook.ps1          — MSVC build script
```

## License

MIT License. See [LICENSE](./LICENSE).
