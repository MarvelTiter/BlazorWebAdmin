namespace BlazorAdmin;

[AutoInjectGenerator.AutoInjectContext]
public static partial class AutoInjectContext
{
    [AutoInjectGenerator.AutoInjectConfiguration(Include = "SERVER")]
    public static partial void AutoInject(this IServiceCollection services);
}