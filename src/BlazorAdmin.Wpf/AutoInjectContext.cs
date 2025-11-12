using Microsoft.Extensions.DependencyInjection;
using Project.Constraints.Common;

namespace BlazorAdmin.Wpf;

[AutoInjectGenerator.AutoInjectContext]
public static partial class AutoInjectContext
{
    [AutoInjectGenerator.AutoInjectConfiguration(Include = AutoInjectGroups.WASM)]
    [AutoInjectGenerator.AutoInjectConfiguration(Include = AutoInjectGroups.Hybrid)]
    public static partial void AutoInjectWpf(this IServiceCollection services);
}
