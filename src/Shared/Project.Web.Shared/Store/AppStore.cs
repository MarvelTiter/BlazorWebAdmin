using AutoInjectGenerator;
using Microsoft.Extensions.Options;
using Project.Constraints.Options;
using Project.Constraints.Store;
using Project.Constraints.Store.Models;
using System.Globalization;

namespace Project.Web.Shared.Store;

[AutoInject]
public class AppStore : StoreBase, IAppStore
{
    private readonly IOptionsMonitor<AppSetting>? options;

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
    public string MainThemeColor { get; set; } = "#1464ff";//#141414
    public void ApplySetting(IAppStore? app)
    {
        Mode = app?.Mode ?? options?.CurrentValue.LayoutMode ?? LayoutMode.Card;
        AppLanguage = app?.AppLanguage ?? options?.CurrentValue.AppLanguage ?? "zh-CN";
        Collapsed = app?.Collapsed ?? false;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(AppLanguage);
    }
}
