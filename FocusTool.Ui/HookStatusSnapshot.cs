namespace FocusTool.Ui;

internal readonly record struct WindowSnapshot(
    IntPtr Handle,
    uint ProcessId,
    uint ThreadId,
    string ProcessName,
    string WindowTitle);

internal readonly record struct HookHandlesSnapshot(
    IntPtr CallWndProc,
    IntPtr CallWndRetProc,
    IntPtr GetMsg,
    IntPtr KeyboardLl);

internal readonly record struct HookStatusSnapshot(
    bool IsEnabled,
    string LastError,
    WindowSnapshot CandidateWindow,
    WindowSnapshot CurrentTarget,
    HookHandlesSnapshot Hooks);
