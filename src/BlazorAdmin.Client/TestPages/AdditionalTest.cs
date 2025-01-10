using AutoInjectGenerator;
using Project.Constraints.Services;
using Project.Constraints.Store.Models;

namespace BlazorAdmin.Client.TestPages
{
    [AutoInject(ServiceType = typeof(IAddtionalInterceptor))]
    public class AdditionalTest(IPageLocatorService pageLocator) : IAddtionalInterceptor
    {
        public Task<bool> RouterChangingAsync(TagRoute route)
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
}
