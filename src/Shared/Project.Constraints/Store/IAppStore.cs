using Project.Constraints.Store.Models;

namespace Project.Constraints.Store;

public enum ThemeMode
{
    Light,
    Dark,
    OS
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
    string MainThemeColor { get; set; }
    string MainBackgroundColor { get; set; }
    void ApplySetting(IAppStore? app);
}