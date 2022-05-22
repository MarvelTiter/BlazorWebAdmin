using AntDesign;
using BlazorWebAdmin.Shared.LayoutComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Project.ApplicationStore.Store;
using Project.Common;
using Project.Services;

namespace BlazorWebAdmin.Shared
{
    public partial class RootLayout : IDisposable
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
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
            }
        }

        const string LOCATION_MAP = "[http://|https://](.+)(?=/)(.+)";
        public event Action<LocationChangedEventArgs> OnNavigated;
        private async void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        {
            //if (NavigationManager.Uri.Contains("/login"))
            //    return;
            //if (string.IsNullOrEmpty(UserStore?.UserId))
            //{
            //    NavigationManager.NavigateTo("/login");
            //    await MsgSrv.Error("登录过期！请重新登录！");
            //}
            await RouterStore.SetActive(NavigationManager.ToBaseRelativePath(e.Location));
            OnNavigated?.Invoke(e);
            StateHasChanged();
        }

        public void HandleRootClick(MouseEventArgs e)
        {
            BodyClickEvent?.Invoke(e);
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
                }

                // 释放未托管的资源(未托管的对象)并重写终结器
                // 将大型字段设置为 null
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
