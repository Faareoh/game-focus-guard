using System.Text.Json;

namespace FocusTool.Ui;

internal sealed class HotKeySettingsStore
{
    private readonly string _settingsPath;

    public HotKeySettingsStore()
    {
        var settingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FocusToolPrototype");
        _settingsPath = Path.Combine(settingsDir, "settings.json");
    }

    public HotKeySettingsLoadResult LoadOrDefault()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, false, "Using default settings");
            }

            var json = File.ReadAllText(_settingsPath);
            var dto = JsonSerializer.Deserialize<HotKeySettingsDto>(json);
            if (dto == null)
            {
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, false, "Settings file invalid, using defaults");
            }

            var binding = new HotKeyBinding((NativeMethods.HotKeyModifiers)dto.Modifiers, (Keys)dto.Key);
            if (!binding.IsValid)
            {
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, dto.AlwaysOnTop, "Saved hotkey invalid, using default hotkey");
            }

            return new HotKeySettingsLoadResult(binding, dto.AlwaysOnTop, "Loaded saved settings");
        }
        catch (Exception ex)
        {
            return new HotKeySettingsLoadResult(HotKeyBinding.Default, false, $"Failed to load settings, using defaults: {ex.Message}");
        }
    }

    public string Save(HotKeyBinding binding, bool alwaysOnTop)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
        var dto = new HotKeySettingsDto
        {
            Modifiers = (uint)binding.Modifiers,
            Key = (int)binding.Key,
            AlwaysOnTop = alwaysOnTop
        };
        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
        return _settingsPath;
    }

    private sealed class HotKeySettingsDto
    {
        public uint Modifiers { get; set; }

        public int Key { get; set; }

        public bool AlwaysOnTop { get; set; }
    }
}
