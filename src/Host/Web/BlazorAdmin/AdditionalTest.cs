using Project.Constraints.Services;
using Project.Constraints.Store.Models;

namespace BlazorAdmin
{
    public class AdditionalTest: IAddtionalInterceptor
    {
        public Task<bool> RouterChangingAsync(TagRoute route)
        {
            Console.WriteLine(route.RouteUrl);
            return Task.FromResult(true);
        }
    }
}
