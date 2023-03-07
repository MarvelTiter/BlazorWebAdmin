using BlazorWeb.Shared.Layouts;
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
        [CascadingParameter] public RootLayout RootLayout { get; set; }
        [Parameter] public string Class { get; set; }

        private ClassHelper ContextmenuClass => ClassHelper.Default.AddClass("context").AddClass("open", () => showContextmenu);

        private async Task CloseTag(RouterMeta state)
        {
            await store.Remove(state.RouteLink);
            nav.NavigateTo(store.Current.RouteLink);
        }

        private void OpenContextMenu(MouseEventArgs e, RouterMeta current)
        {
            this.current = current;
            contextmenuLeft = $"{e.ClientX + 10}px";
            contextmenuTop = $"{e.ClientY + 10}px";
            showContextmenu = true;
            RootLayout.BodyClickEvent += RootLayout_BodyClickEvent; ;
        }

        private void RootLayout_BodyClickEvent(MouseEventArgs obj)
        {
            CloseMenu();
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
            await CloseTag(current);
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
        }
    }
}
