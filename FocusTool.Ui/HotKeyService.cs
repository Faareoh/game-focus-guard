using System.Runtime.InteropServices;

namespace FocusTool.Ui;

internal sealed class HotKeyService
{
    private readonly IntPtr _windowHandle;
    private readonly int _hotKeyId;

    public HotKeyService(IntPtr windowHandle, int hotKeyId, HotKeyBinding initialBinding)
    {
        _windowHandle = windowHandle;
        _hotKeyId = hotKeyId;
        CurrentBinding = initialBinding;
    }

    public HotKeyBinding CurrentBinding { get; private set; }

    public bool IsRegistered { get; private set; }

    public string LastStatus { get; private set; } = Strings.Unregistered;

    public bool TryRegisterCurrent(out string status)
    {
        return TryRegisterBinding(CurrentBinding, out status);
    }

    public void Unregister()
    {
        if (!IsRegistered)
        {
            return;
        }

        NativeMethods.UnregisterHotKey(_windowHandle, _hotKeyId);
        IsRegistered = false;
        LastStatus = Strings.Unregistered;
    }

    public bool TryUpdateBinding(HotKeyBinding newBinding, out string status)
    {
        var previousBinding = CurrentBinding;
        var wasRegistered = IsRegistered;

        if (wasRegistered)
        {
            Unregister();
        }

        if (TryRegisterBinding(newBinding, out status))
        {
            CurrentBinding = newBinding;
            return true;
        }

        if (wasRegistered)
        {
            var restored = TryRegisterBinding(previousBinding, out var restoreStatus);
            if (restored)
            {
                CurrentBinding = previousBinding;
                status = $"{status} | {Strings.RestoredPreviousBinding}";
            }
            else
            {
                status = $"{status} | {Strings.FailedRestorePreviousBinding}: {restoreStatus}";
            }
        }

        return false;
    }

    private bool TryRegisterBinding(HotKeyBinding binding, out string status)
    {
        if (!binding.IsValid)
        {
            status = Strings.InvalidHotkey;
            LastStatus = status;
            return false;
        }

        if (NativeMethods.RegisterHotKey(_windowHandle, _hotKeyId, binding.Modifiers, binding.VirtualKeyCode))
        {
            CurrentBinding = binding;
            IsRegistered = true;
            status = Strings.RegisteredHotkey(binding.ToDisplayString());
            LastStatus = status;
            return true;
        }

        var error = Marshal.GetLastWin32Error();
        status = error switch
        {
            1409 => Strings.HotkeyAlreadyRegistered(binding.ToDisplayString()),
            _ => Strings.RegisterHotKeyFailed(error)
        };
        LastStatus = status;
        return false;
    }
}
