using Microsoft.Extensions.DependencyInjection;
using BlazorTemplate.Constraints.Common;

namespace BlazorAdmin.Wpf;

[AutoInjectGenerator.AutoInjectContext]
public static partial class AutoInjectContext
{
    [AutoInjectGenerator.AutoInjectConfiguration(Include = AutoInjectGroups.WASM)]
    [AutoInjectGenerator.AutoInjectConfiguration(Include = AutoInjectGroups.Hybrid)]
    [AutoInjectGenerator.AutoInjectConfiguration(Include = AutoInjectGroups.SERVER)]
    public static partial void AutoInjectWpf(this IServiceCollection services);
}
