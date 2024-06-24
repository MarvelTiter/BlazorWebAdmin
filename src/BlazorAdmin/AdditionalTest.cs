using Project.Constraints.Services;
using Project.Constraints.Store.Models;

namespace BlazorAdmin
{
    public class AdditionalTest: IAddtionalInterceptor
    {
        private readonly IPageLocatorService pageLocator;

        public AdditionalTest(IPageLocatorService pageLocator)
        {
            this.pageLocator = pageLocator;
        }
        public Task<bool> RouterChangingAsync(TagRoute route)
        {
            Console.WriteLine(route.RouteUrl);
            return Task.FromResult(true);
        }

        public Task AfterWebApplicationAccessedAsync()
        {
            pageLocator.SetPage<TestPages.TestPage2>("LocatorTest");
            return Task.CompletedTask;
        }
    }
}
