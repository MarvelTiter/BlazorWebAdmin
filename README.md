### . 创建新项目（启动程序），引用 Shared，AppCore，Constraints，Model 项目

1. 移动App.razor, Routes.razor, _Imports.razor到根目录
2. 删除Components文件夹
3. 修改Routes.razor
```CSharp
<Project.AppCore.Layouts.AppRoot AppAssembly="typeof(App).Assembly"></Project.AppCore.Layouts.AppRoot>
```
4. 修改Program.cs
```CSharp
using Project.AppCore;

builder.AddProject(setting =>
{
    // ICustomSettingProvider的实现
    setting.SettingProviderType = typeof(SettingImpl);
    setting.AutoInjectConfig = filter =>
    {

    };
});

app.UseProject();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([.. Config.Pages]);

```
