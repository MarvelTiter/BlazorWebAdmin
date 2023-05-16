using Project.Common.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Project.AppCore.Store
{
    public enum LayoutMode
    {
        [Display(Name = "经典")]
        Classic,
        [Display(Name = "卡片式")]
        Card,
        [Display(Name = "流线型")]
        Line,
    }
    public class AppStore : StoreBase
    {
        public const string KEY = "APP_SETTING";
        public bool Init { get; set; }
        public LayoutMode? Mode { get; set; }
        public string? AppLanguage { get; set; } = "zh-CN";
        public bool DarkMode { get; set; }
        public bool Collapsed { get; set; }
        public int SideBarExpandWidth { get; set; } = 260;
        public string MainThemeColor { get; set; } = "#1464ff";
        public void ApplySetting(AppStore? app)
        {
            this.Mode = app?.Mode ?? LayoutMode.Card;
            this.AppLanguage = app?.AppLanguage ?? "zh-CN";
            this.Collapsed = app?.Collapsed ?? false;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(this.AppLanguage);
        }
    }
}
