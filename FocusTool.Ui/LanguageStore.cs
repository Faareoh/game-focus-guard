namespace FocusTool.Ui;

internal static class LanguageStore
{
    private static readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FocusToolPrototype",
        "language.txt");

    public static Language Load()
    {
        try
        {
            if (!File.Exists(_path))
            {
                return Language.EN;
            }

            var text = File.ReadAllText(_path).Trim();
            return Enum.TryParse<Language>(text, ignoreCase: true, out var lang) ? lang : Language.EN;
        }
        catch
        {
            return Language.EN;
        }
    }

    public static void Save(Language lang)
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
            File.WriteAllText(_path, lang.ToString());
        }
        catch { }
    }
}
