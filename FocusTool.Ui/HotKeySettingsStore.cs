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
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, Strings.UsingDefaultHotkey);
            }

            var json = File.ReadAllText(_settingsPath);
            var dto = JsonSerializer.Deserialize<HotKeySettingsDto>(json);
            if (dto == null)
            {
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, Strings.SettingsFileInvalid);
            }

            var binding = new HotKeyBinding((NativeMethods.HotKeyModifiers)dto.Modifiers, (Keys)dto.Key);
            if (!binding.IsValid)
            {
                return new HotKeySettingsLoadResult(HotKeyBinding.Default, Strings.SavedHotkeyInvalid);
            }

            return new HotKeySettingsLoadResult(binding, Strings.LoadedSavedHotkey);
        }
        catch (Exception ex)
        {
            return new HotKeySettingsLoadResult(HotKeyBinding.Default, Strings.FailedLoadSettings(ex.Message));
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
