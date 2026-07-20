namespace FocusTool.Ui;

internal readonly record struct HotKeySettingsLoadResult(
    HotKeyBinding Binding,
    bool AlwaysOnTop,
    string StatusMessage);
