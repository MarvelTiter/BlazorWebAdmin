using AntDesign;
using BlazorWeb.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Project.AppCore.Store;
using Project.Common;

namespace BlazorWeb.Shared.Layouts
{
    public partial class RootLayout : IDisposable
    {
        [Inject] public NavigationManager Navigator { get; set; }
        [Inject] public RouterStore RouterStore { get; set; }
        [Inject] public UserStore UserStore { get; set; }
        [Inject] public MessageService MsgSrv { get; set; }

        public event Func<MouseEventArgs, Task> BodyClickEvent;
        public event Func<KeyboardEventArgs, Task> OnKeyDown;
        public event Func<KeyboardEventArgs, Task> OnKeyUp;
        protected ElementReference? RootWrapper { get; set; }
        //protected override async Task OnInitializedAsync()
        //{
        //    await base.OnInitializedAsync();
        //    if (NavigationManager != null)
        //    {
        //        NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        //    }
        //}

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                _ = RootWrapper?.FocusAsync();
            }
        }

        const string LOCATION_MAP = "[http://|https://](.+)(?=/)(.+)";
        //public event Action<LocationChangedEventArgs> OnNavigated;
        //private void NavigationManager_LocationChanged(object? sender, LocationChangedEventArgs e)
        //{
        //    //if (NavigationManager.Uri.Contains("/login"))
        //    //    return;
        //    //if (string.IsNullOrEmpty(UserStore?.UserId))
        //    //{
        //    //    NavigationManager.NavigateTo("/login");
        //    //    await MsgSrv.Error("登录过期！请重新登录！");
        //    //}
        //    //RouterStore.SetActive(NavigationManager.ToBaseRelativePath(e.Location));
        //    OnNavigated?.Invoke(e);
        //    //StateHasChanged();
        //}

        protected Task HandleRootClick(MouseEventArgs e)
        {
            return BodyClickEvent?.Invoke(e);
        }

        protected Task HandleKeyAction(KeyboardEventArgs e)
        {
            return OnKeyDown?.Invoke(e);
        }

        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)
                    //Navigator.LocationChanged -= NavigationManager_LocationChanged;
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
