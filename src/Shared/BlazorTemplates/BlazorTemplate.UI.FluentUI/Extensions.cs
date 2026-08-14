using BlazorTemplate.ClientCore.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorTemplate.UI.FluentUI;

public static class Extensions
{
    public static void AddFluentUI(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddFluentUIComponents();
        services.AddScoped<IUIService, UIService>();
    }

        
}