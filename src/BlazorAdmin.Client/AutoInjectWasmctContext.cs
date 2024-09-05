namespace BlazorAdmin.Client;


[AutoInjectGenerator.AutoInjectContext]
public static partial class AutoInjectWasmctContext
{
    [AutoInjectGenerator.AutoInjectConfiguration(Include = "WASM")]
    public static partial void AutoInjectWasm(this IServiceCollection services);
}
