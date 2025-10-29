using Project.Constraints.Store.Models;

namespace Project.Constraints.Store;

//public enum ThemeMode
//{
//    Light,
//    Dark,
//    OS
//}

public record ThemeMode(string Name, int Value)
{
    public static readonly ThemeMode Light = new("Light", 0);
    public static readonly ThemeMode Dark = new("Dark", 1);
    public static readonly ThemeMode OS = new("OS", 2);
    public override string ToString()
    {
        return Name;
    }

    public static bool operator ==(ThemeMode mode, int value)
    {
        return mode.Value == value;
    }
    public static bool operator !=(ThemeMode mode, int value)
    {
        return mode.Value != value;
    }
}

public interface IAppStore
{
    bool Working { get; set; }
    LayoutMode? Mode { get; set; }
    string? AppLanguage { get; set; }
    ThemeMode Theme { get; set; }
    ThemeMode MenuTheme { get; set; }
    bool Collapsed { get; set; }
    int SideBarExpandWidth { get; set; }
    string? MainThemeColor { get; set; }
    string? MainBackgroundColor { get; set; }
    void ApplySetting(IAppStore? app);
}