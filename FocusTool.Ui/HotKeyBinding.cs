namespace FocusTool.Ui;

internal readonly record struct HotKeyBinding(NativeMethods.HotKeyModifiers Modifiers, Keys Key)
{
    public static HotKeyBinding Default => new(
        NativeMethods.HotKeyModifiers.Control |
        NativeMethods.HotKeyModifiers.Shift |
        NativeMethods.HotKeyModifiers.Alt,
        Keys.T);

    public bool IsValid => Key != Keys.None && !IsModifierOnlyKey(Key);

    public uint VirtualKeyCode => (uint)Key;

    public string ToDisplayString()
    {
        var parts = new List<string>(4);

        if (Modifiers.HasFlag(NativeMethods.HotKeyModifiers.Control))
        {
            parts.Add("Ctrl");
        }

        if (Modifiers.HasFlag(NativeMethods.HotKeyModifiers.Shift))
        {
            parts.Add("Shift");
        }

        if (Modifiers.HasFlag(NativeMethods.HotKeyModifiers.Alt))
        {
            parts.Add("Alt");
        }

        parts.Add(Key.ToString());
        return string.Join("+", parts);
    }

    public static bool TryFromKeyData(Keys keyData, out HotKeyBinding binding)
    {
        if (keyData == Keys.Escape)
        {
            binding = default;
            return false;
        }

        var modifiers = NativeMethods.HotKeyModifiers.None;
        if ((keyData & Keys.Control) == Keys.Control)
        {
            modifiers |= NativeMethods.HotKeyModifiers.Control;
        }

        if ((keyData & Keys.Shift) == Keys.Shift)
        {
            modifiers |= NativeMethods.HotKeyModifiers.Shift;
        }

        if ((keyData & Keys.Alt) == Keys.Alt)
        {
            modifiers |= NativeMethods.HotKeyModifiers.Alt;
        }

        var keyCode = keyData & Keys.KeyCode;
        binding = new HotKeyBinding(modifiers, keyCode);
        return binding.IsValid;
    }

    private static bool IsModifierOnlyKey(Keys key)
    {
        return key is Keys.ControlKey
            or Keys.ShiftKey
            or Keys.Menu
            or Keys.LShiftKey
            or Keys.RShiftKey
            or Keys.LControlKey
            or Keys.RControlKey
            or Keys.LMenu
            or Keys.RMenu;
    }
}
