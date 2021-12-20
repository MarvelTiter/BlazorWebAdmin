using BlazorWebAdmin.StoreData;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace BlazorWebAdmin.Shared
{
    public partial class RootLayout
    {
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        //[Inject]
        //public ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
		public RouterStore RouterStore { get; set; }
		protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (NavigationManager != null)
                NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        }
        const string LOCATION_MAP = "[http://|https://](.+)(?=/)(.+)";
        private async void NavigationManager_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
   //         var store = await ProtectedSessionStore.GetAsync<RouterStore>("store");
   //         if (!store.Success)
   //         {
   //             RouterStore = new RouterStore();
   //             await ProtectedSessionStore.SetAsync("store", RouterStore);
   //         }
			//else
			//{
   //             RouterStore = store.Value;
			//}
            var url = e.Location.Replace("http://", "").Replace("https://", "").Split('/');
            if (url.Length > 1)
            {
                for (int i = 1; i < url.Length; i++)
                {
                    await RouterStore.TryAdd(url[1]);
                }
            }
        }
    }
}
