using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;
using BlazorTemplate.Constraints.UI;

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