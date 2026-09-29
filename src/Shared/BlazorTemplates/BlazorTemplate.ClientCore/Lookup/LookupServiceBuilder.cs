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
    ///     登记一个枚举，让 <c>EnumDisplayProviderGenerator</c> 为它生成编译期元数据
    ///     （<c>{Name}EnumMeta : IEnumMeta</c>）；元数据一旦存在，
    ///     <c>EnumHelper&lt;TEnum&gt;</c> 与下拉用的 <c>EnumLookupProvider&lt;TEnum&gt;</c> 都会由它派生。
    ///     <para>
    ///         这是<strong>编译期标记</strong>，方法体本身不做任何注册：生成器以语法方式识别本次调用，
    ///         取泛型实参所指的枚举来产出代码。适用于定义在<strong>引用程序集</strong>里的枚举
    ///         （生成器只扫描当前编译单元的枚举声明，看不到其他程序集里的）。
    ///     </para>
    ///     <para>
    ///         本程序集内定义的枚举无需调用此方法——生成器会无条件覆盖。
    ///     </para>
    /// </summary>
    public LookupServiceBuilder AddEnumLookupProvider<TEnum>()
        where TEnum : Enum
    {
        // 有意留空：仅作为生成器的编译期识别点。
        return this;
    }
}
