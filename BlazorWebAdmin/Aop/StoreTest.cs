using BlazorWebAdmin.Store;

namespace BlazorWebAdmin.Aop
{
    public class StoreTest
    {
        private readonly IServiceProvider serviceProvider;

        public StoreTest(IServiceProvider serviceProvider)
        {
            Console.WriteLine($"provider {serviceProvider.GetType().FullName} {serviceProvider.GetHashCode()}");
            var store = serviceProvider.GetRequiredService<UserStore>();
            this.serviceProvider = serviceProvider;
            Console.WriteLine($"StoreTest {store.GetHashCode()}");
        }
    }
}
