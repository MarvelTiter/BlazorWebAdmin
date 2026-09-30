using Microsoft.Extensions.DependencyInjection;

namespace BlazorTemplate.ClientCore.Lookup;

public class LookupServiceBuilder(IServiceCollection services)
{
    public LookupServiceBuilder AddLookupProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Tp>()
        where Tp : class, ILookupProvider
    {
        services.AddScoped<ILookupProvider, Tp>();
        return this;
    }
    public LookupServiceBuilder AddStaticLookupProvider<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Tp>()
        where Tp : class, ILookupProvider
    {
        services.AddSingleton<ILookupProvider, Tp>();
        return this;
    }
    /// <summary>
    /// 登记一个枚举，让 <c>EnumDisplayProviderGenerator</c> 为它生成编译期元数据
    /// </summary>
    public LookupServiceBuilder AddEnumLookupProvider<TEnum>()
        where TEnum : Enum
    {
        // 有意留空：仅作为生成器的编译期识别点。
        return this;
    }
}
