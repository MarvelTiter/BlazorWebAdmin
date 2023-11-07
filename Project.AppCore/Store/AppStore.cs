using Microsoft.Extensions.Options;
using Project.AppCore.Options;
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
        private readonly IOptionsMonitor<AppSetting> options;

        public AppStore(IOptionsMonitor<AppSetting> options)
        {
            Mode = options.CurrentValue.LayoutMode ?? LayoutMode.Card;
            AppLanguage = options.CurrentValue.AppLanguage ?? "zh-CN";
            this.options = options;
        }
        public AppStore()
        {
            
        }
        public bool Init { get; set; }
        /// <summary>
        /// 是否在进行长时间任务、防止长时间无动作后自动跳转到登录页
        /// </summary>
        public bool Working { get; set; }
        public LayoutMode? Mode { get; set; }
        public string? AppLanguage { get; set; }
        public bool DarkMode { get; set; }
        public bool Collapsed { get; set; }
        public int SideBarExpandWidth { get; set; } = 260;
        public string MainThemeColor { get; set; } = "#1464ff";
        public void ApplySetting(AppStore? app)
        {
            this.Mode = app?.Mode ?? options.CurrentValue.LayoutMode ?? LayoutMode.Card;
            this.AppLanguage = app?.AppLanguage ?? options.CurrentValue.AppLanguage ?? "zh-CN";
            this.Collapsed = app?.Collapsed ?? false;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(this.AppLanguage);
        }
    }
}
