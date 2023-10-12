using BlazorWeb.Shared.Interfaces;
using BlazorWeb.Shared.Layouts;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Project.AppCore.Store;
using Project.Common;

namespace BlazorWeb.Shared.Layouts.LayoutComponents
{
    public partial class NavTabs : IDisposable
    {
        private bool showContextmenu = false;
        private string contextmenuLeft = "";
        private string contextmenuTop = "";
        private RouterMeta current;
        [CascadingParameter] public IDomEventHandler RootLayout { get; set; }
        [Parameter] public string Class { get; set; }
        private int navMenuWidth = 200;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                navMenuWidth = await Js.ElementProperty<int>(".nav-menu", "offsetWidth");
            }
        }

        private ClassHelper ContextmenuClass => ClassHelper.Default.AddClass("context").AddClass("open", () => showContextmenu);

        private void CloseTag(RouterMeta state)
        {
            store.Remove(state.RouteLink);
            nav.NavigateTo(store.Current.RouteLink);
        }

        private void OpenContextMenu(MouseEventArgs e, RouterMeta current)
        {
            this.current = current;
            contextmenuLeft = $"{e.ClientX + 10}px";
            contextmenuTop = $"{e.ClientY + 10}px";
            showContextmenu = true;
            RootLayout.BodyClickEvent += RootLayout_BodyClickEvent;
        }

        private Task RootLayout_BodyClickEvent(MouseEventArgs obj)
        {
            return CloseMenu();
        }

        private async Task CloseOther()
        {
            if (current == null) return;
            //await store.Reset();
            //await store.TryAdd(current.RouteLink, current.RouteName);
            //nav.NavigateTo("/" + current.RouteLink);
            await store.RemoveOther(current.RouteLink);
            await CloseMenu();
        }

        private async Task CloseAll()
        {
            await store.Reset();
            nav.NavigateTo("/");
            await CloseMenu();
        }

        private async Task CloseSelf()
        {
            if (current == null) return;
            CloseTag(current);
            await CloseMenu();
        }

        private Task CloseMenu(MouseEventArgs e = null)
        {
            if (showContextmenu)
                showContextmenu = false;
            RootLayout.BodyClickEvent -= RootLayout_BodyClickEvent;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            store.DataChangedEvent -= StateHasChanged;
            RootLayout.BodyClickEvent -= RootLayout_BodyClickEvent;
        }
    }
}
