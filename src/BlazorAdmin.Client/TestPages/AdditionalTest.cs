using AutoInjectGenerator;
using Project.Constraints.Services;
using Project.Constraints.Store.Models;

namespace BlazorAdmin.Client.TestPages
{
    [AutoInject(ServiceType = typeof(IAddtionalInterceptor))]
    public class AdditionalTest : IAddtionalInterceptor
    {
        private readonly IPageLocatorService pageLocator;

        public AdditionalTest(IPageLocatorService pageLocator)
        {
            this.pageLocator = pageLocator;
        }
        public Task<bool> RouterChangingAsync(TagRoute route)
        {
            Console.WriteLine($"AdditionalTest: RouterChangingAsync: {route.RouteUrl}");
            return Task.FromResult(true);
        }

        public Task AfterWebApplicationAccessedAsync()
        {
            pageLocator.SetPage<TestPage2>("LocatorTest");
            return Task.CompletedTask;
        }
    }
}
