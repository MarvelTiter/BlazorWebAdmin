using AutoInjectGenerator;
using BlazorTemplate.ClientCore.Services;
using BlazorTemplate.ClientCore.Store.Models;

namespace BlazorAdmin.Client.TestPages;

[AutoInject(ServiceType = typeof(IAddtionalInterceptor))]
public class AdditionalTest(IPageLocatorService pageLocator) : IAddtionalInterceptor
{
    public Task<bool> RouterChangingAsync(RouteTag route)
    {
        Console.WriteLine($"AdditionalTest: RouterChangingAsync: {route.RouteUrl}");
        return Task.FromResult(true);
    }

    public Task AfterWebApplicationAccessedAsync()
    {
        pageLocator.SetPage<TestSplitAndDataTable>("LocatorTest");
        return Task.CompletedTask;
    }
}