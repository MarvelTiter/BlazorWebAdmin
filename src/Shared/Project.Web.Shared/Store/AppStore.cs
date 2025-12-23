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
    private readonly ILanguageService? languageService;

    public AppStore(IOptionsMonitor<AppSetting> options, ILanguageService languageService)
    {
        Mode = options.CurrentValue.LayoutMode ?? LayoutMode.Card;
        AppLanguage = options.CurrentValue.AppLanguage ?? "zh-CN";
        this.options = options;
        this.languageService = languageService;
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
    public ThemeMode Theme { get; set; } = ThemeMode.Light;
    public ThemeMode MenuTheme { get; set; } = ThemeMode.Dark;
    public bool Collapsed { get; set; }
    public int SideBarExpandWidth { get; set; } = 260;
    public string? MainThemeColor { get; set; }
    public string? MainBackgroundColor { get; set; }
    public void ApplySetting(IAppStore? app)
    {
        Mode = app?.Mode ?? options?.CurrentValue.LayoutMode ?? LayoutMode.Card;
        AppLanguage = app?.AppLanguage ?? options?.CurrentValue.AppLanguage ?? "zh-CN";
        Theme = app?.Theme ?? ThemeMode.Light;
        MenuTheme = app?.MenuTheme ?? ThemeMode.Dark;
        Collapsed = app?.Collapsed ?? false;
        MainThemeColor = app?.MainThemeColor;
        MainBackgroundColor = app?.MainBackgroundColor;
        SideBarExpandWidth = app?.SideBarExpandWidth ?? 260;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(AppLanguage);
        languageService?.SetLanguage(AppLanguage);
    }
}
