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

    public string LastStatus { get; private set; } = "Unregistered";

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
        LastStatus = "Unregistered";
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
                status = $"{status} | Restored previous binding";
            }
            else
            {
                status = $"{status} | Failed to restore previous binding: {restoreStatus}";
            }
        }

        return false;
    }

    private bool TryRegisterBinding(HotKeyBinding binding, out string status)
    {
        if (!binding.IsValid)
        {
            status = "Invalid hotkey";
            LastStatus = status;
            return false;
        }

        if (NativeMethods.RegisterHotKey(_windowHandle, _hotKeyId, binding.Modifiers, binding.VirtualKeyCode))
        {
            CurrentBinding = binding;
            IsRegistered = true;
            status = $"Registered {binding.ToDisplayString()}";
            LastStatus = status;
            return true;
        }

        var error = Marshal.GetLastWin32Error();
        status = error switch
        {
            1409 => $"Hotkey already registered by another instance or program ({binding.ToDisplayString()})",
            _ => $"RegisterHotKey failed. Win32={error}"
        };
        LastStatus = status;
        return false;
    }
}
