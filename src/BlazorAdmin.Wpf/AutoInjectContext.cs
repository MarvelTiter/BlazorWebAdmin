using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.Common;

namespace BlazorAdmin.Wpf;

[AutoInjectGenerator.AutoInjectContext]
public static partial class AutoInjectContext
{
    [AutoInjectGenerator.AutoInjectConfiguration(Include = "WASM")]
    public static partial void AutoInject(this IServiceCollection services);
}

[AutoInjectGenerator.AutoInjectContext]
public static partial class AutoInjectHybridContext
{

    [AutoInjectGenerator.AutoInjectConfiguration(Include = AutoInjectGroups.Hybrid)]
    public static partial void AutoInjectHybrid(this IServiceCollection services);
}