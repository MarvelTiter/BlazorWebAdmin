using Microsoft.Extensions.DependencyInjection;

namespace BlazorTemplate.ClientCore.Lockup;

public class LookupServiceBuilder(IServiceCollection services)
{
    public LookupServiceBuilder AddLookupProvider<Tp>()
        where Tp : class, ILookupProvider
    {
        services.AddScoped<ILookupProvider, Tp>();
        return this;
    }
    public LookupServiceBuilder AddStaticLookupProvider<Tp>()
        where Tp : class, ILookupProvider
    {
        services.AddSingleton<ILookupProvider, Tp>();
        return this;
    }
    public LookupServiceBuilder AddEnumLookupProvider<TEnum>()
        where TEnum : Enum
    {
        //services.AddSingleton<ILookupProvider, EnumLookupProvider>();
        return this;
    }
}
