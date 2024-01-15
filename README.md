### . 创建新项目（启动程序），引用 Shared，AppCore，Constraints，Model 项目

1. 移动 App.razor, Routes.razor, \_Imports.razor 到根目录
2. 删除 Components 文件夹
3. 修改 Routes.razor
```CSharp
<Project.AppCore.Layouts.AppRoot AppAssembly="typeof(App).Assembly"></Project.AppCore.Layouts.AppRoot>
```
4. 拷贝appsettings.json


5. 修改 Program.cs

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

6. 修改 App.razor 添加样式和 js 引用

```html
<link  href="_content/AntDesign/css/ant-design-blazor.css"  rel="stylesheet"/>
<script src="_content/AntDesign/js/ant-design-blazor.js"></script>
<link  href="_content/Project.Web.Shared/css/app.css"  rel="stylesheet"/>
<link  href="_content/Project.Web.Shared/css/echarts.css"  rel="stylesheet"/>
```
```CSharp 
// 可选，配置重连样式
<Project.Web.Shared.Components.ReconnectorOutlet @rendermode="@InteractiveServer"></Project.Web.Shared.Components.ReconnectorOutlet>
```
