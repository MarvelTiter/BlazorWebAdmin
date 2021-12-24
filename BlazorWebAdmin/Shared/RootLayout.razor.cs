using AntDesign;
using BlazorWebAdmin.Shared.LayoutComponents;
using BlazorWebAdmin.Store;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Project.Common;

namespace BlazorWebAdmin.Shared
{
    public partial class RootLayout
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        //[Inject]
        //public ProtectedSessionStorage ProtectedSessionStore { get; set; }
        [Inject]
        public RouterStore RouterStore { get; set; }
        [Inject]
        public UserStore UserStore { get; set; }
        [Inject]
        public MessageService MsgSrv { get; set; }
        [Inject]
        public EventDispatcher Dispatcher { get; set; }

        public event Action<MouseEventArgs> BodyClickEvent;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            if (NavigationManager != null)
            {
                NavigationManager.LocationChanged += NavigationManager_LocationChanged;
                if (NavigationManager.Uri.Contains("/login"))
                    return;
                if (string.IsNullOrEmpty(UserStore?.UserId))
                {
                    NavigationManager.NavigateTo("/login");
                    await MsgSrv.Error("登录过期！请重新登录！");
                }
            }
        }
        const string LOCATION_MAP = "[http://|https://](.+)(?=/)(.+)";
        private async void NavigationManager_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            if (NavigationManager.Uri.Contains("/login"))
                return;
            if (string.IsNullOrEmpty(UserStore?.UserId))
            {
                NavigationManager.NavigateTo("/login");
                await MsgSrv.Error("登录过期！请重新登录！");
            }
            var url = e.Location.Replace("http://", "").Replace("https://", "").Split('/');
            if (url.Length == 2)
                await RouterStore.SetActive(url[1]);
        }

        public void HandleRootClick(MouseEventArgs e)
        {
            BodyClickEvent?.Invoke(e);
        }
    }
}
