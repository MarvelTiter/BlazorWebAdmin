using AutoInjectGenerator;
using BlazorTemplate.ClientCore.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace BlazorTemplate.UI.FluentUI;

public static class Extensions
{
    [CustomModuleServiceConfiguration]
    public static void AddFluentUI(this IServiceCollection services)
    {
        services.AddFluentUIComponents();
        services.AddScoped<IUIService, UIService>();
    }
        
}