# 基于 AntDesignBlazor 的 Blazor 后台管理框架

### . 创建新项目（启动程序），引用 Shared，AppCore，Services 项目

1. 修改 Program

```CSharp
using BlazorWeb.Shared;

BlazorWeb.Shared.Program.Run("Demo", DefaultSetup.Setup, DefaultSetup.SetupCustomAppUsage, null, args);
```

2. 修改 App.Razor

```CSharp
@using BlazorWeb.Shared.Layouts
<AppRoot AppAssembly="@typeof(App).Assembly"></AppRoot>
```

3. 拷贝 Demo 项目中的 Pages、appsettings.json
