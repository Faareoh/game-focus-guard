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
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, "Using default hotkey");
            }

            var json = File.ReadAllText(_settingsPath);
            var dto = JsonSerializer.Deserialize<HotKeySettingsDto>(json);
            if (dto == null)
            {
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, "Settings file invalid, using default hotkey");
            }

            var binding = new HotKeyBinding((NativeMethods.HotKeyModifiers)dto.Modifiers, (Keys)dto.Key);
            if (!binding.IsValid)
            {
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, "Saved hotkey invalid, using default hotkey");
            }

            return new HotKeySettingsLoadResult(binding, "Loaded saved hotkey");
        }
        catch (Exception ex)
        {
            return new HotKeySettingsLoadResult(HotKeyBinding.Default, $"Failed to load settings, using default hotkey: {ex.Message}");
        }
    }

    public string Save(HotKeyBinding binding)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
        var dto = new HotKeySettingsDto
        {
            Modifiers = (uint)binding.Modifiers,
            Key = (int)binding.Key
        };
        var json = JsonSerializer.Serialize(dto, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_settingsPath, json);
        return _settingsPath;
    }

    private sealed class HotKeySettingsDto
    {
        public uint Modifiers { get; set; }

        public int Key { get; set; }
    }
}
