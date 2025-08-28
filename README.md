# 使用方式

## 创建Template包

- 项目根目录下运行`dotnet pack`
- 安装`./bin/Release`下生成的nuget包
- 运行`dotnet new mtblazor -n projectName`

### 可选参数

|参数|说明|默认值|
|-|-|-|
|ExcludeDefaultService|默认页面是否去除默认的服务实现(需要自己实现服务)|false|
|ExcludeDefaultPages|是否去除默认的页面|false|
|UseClientProject|是否使用.Client项目(依然保留.Client项目, 只是不再引用, 有需要可以引用)|true|

### 定制说明

通过`builder.AddServerProject(Action<ProjectSetting> setting)`方法配置

- `setting.App`设置Id、Name、Company等等, 目前除了显示页脚外没什么用处
- `setting.ConfigureSettingProviderType`, 设置`IProjectSettingService`的实现, 提供了一些钩子, 例如登录成功、首先渲染后、路由跳转守卫等等
- `setting.AddInterceotor`跟上面类似, 设置额外的钩子
- `setting.ConfigureAuthService`设置`IAuthService`的实现, 用于登录登出等等
- `setting.ConfigurePage`用于配置`IPageLocatorService`[参考](#客制化UI)

### Attribute菜单

使用`PageGroupAttribute`和`PageInfoAttribute`配置菜单

目前只支持配置2级菜单

### 客制化UI

场景: 路由地址固定, 组件UI不确定。建议搭配`Project.Constraints.Page.SystemPageIndex<TPage>`使用

#### 例子1: 需要自定义登录页UI

默认的登录页UI是`DefaultLogin`

```csharp
setting.ConfigurePage(locator =>
{
    locator.SetLoginPageType<DefaultLogin>();
});
```

重写`CascadingSelf`属性，将自身通过级联传递，重写`GetPageType`方法获取UI组件类型

```csharp
[Route("/account/login")]
[Layout(typeof(BlankLayout))]
[ExcludeFromInteractiveRouting]
public class Login : SystemPageIndex<Login>, ILoginPage
{
    protected override bool CascadingSelf => true;

    public Task HandleLogin(LoginFormModel model)
    {
        // ....
        // 登录逻辑
    }

    protected override Type? GetPageType(IPageLocatorService customSetting)
    {
        return customSetting.GetLoginPageType();
    }

}
```

登录页UI实现，通过级联参数获取ILoginPage实例，并调用上面的登录方法

```csharp
@inject IOptionsMonitor<AppSetting> AppOptions
<PageTitle>登录</PageTitle>
<div class="login-container" style="background-image: url('/_content/Project.Web.Shared/assets/login_bg.png');background-size:cover;">
    <div class="title-container">
        <div class="title-position">
            @*style="background-image: url('assets/logo.png')"*@
            <h3 class="title">@AppOptions.CurrentValue.AppTitle</h3>
        </div>
    </div>
    <div class="login-form">
        @LoginPage.UI.BuildLoginForm(LoginPage.HandleLogin)
    </div>
</div>
@code {
    [CascadingParameter(Name = "Login"), NotNull] public ILoginPage? LoginPage { get; set; }
}
```

#### 例子2: 需要自定义主页UI

```csharp
setting.ConfigurePage(locator =>
{
    locator.SetDashboardType<BlazorAdmin.Client.TestPages.TestDashboard>();
});
```

## master分支 web host

## master.wpf分支 wpf host

