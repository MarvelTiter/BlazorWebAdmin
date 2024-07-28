using Project.Constraints.Store.Models;

namespace Project.Constraints.Store
{
    public interface IAppStore
    {
        bool Working { get; set; }
        LayoutMode? Mode { get; set; }
        string? AppLanguage { get; set; }
        bool DarkMode { get; set; }
        bool Collapsed { get; set; }
        int SideBarExpandWidth { get; set; }
        string MainThemeColor { get; set; }
        void ApplySetting(IAppStore? app);
    }
}
