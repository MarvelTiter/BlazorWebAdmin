using Project.Common.Attributes;
using System.ComponentModel.DataAnnotations;

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
        public string AppLanguage { get; set; } = "zh-CN";
        public bool DarkMode { get; set; }
        public bool Collapsed { get; set; }
        public int SideBarExpandWidth { get; set; } = 260;
        public string MainThemeColor { get; set; } = "#1464ff";
    }
}
