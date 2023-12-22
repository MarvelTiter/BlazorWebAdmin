using BlazorWeb.Shared.Interfaces;
using BlazorWeb.Shared.Layouts;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Project.AppCore.PageHelper;
using Project.AppCore.Routers;
using Project.AppCore.Store;
using Project.Common;
using System;

namespace BlazorWeb.Shared.Layouts.LayoutComponents
{
    public partial class NavTabs : IDisposable
    {
        private bool showContextmenu = false;
        private string contextmenuLeft = "";
        private string contextmenuTop = "";
        private TagRoute current;
        [CascadingParameter] public IDomEventHandler RootLayout { get; set; }
        [Parameter] public string Class { get; set; }
        [Inject] RouterStore Store { get; set; }
        [Inject] NavigationManager Nav { get; set; }
        [Inject] AppStore App { get; set; }
        private int navMenuWidth = 200;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Store.DataChangedEvent += StateHasChanged;
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                navMenuWidth = await Module!.InvokeAsync<int>("getMenuWidth", "");
            }
        }

        private ClassHelper ContextmenuClass => ClassHelper.Default.AddClass("context").AddClass("open", () => showContextmenu);

        private void CloseTag(TagRoute state)
        {
            var index = Store.TopLinks.IndexOf(state);
            Store.Remove(state.RouteUrl);
            if (index < Store.TopLinks.Count)
            {
                Nav.NavigateTo(Store.TopLinks[index].RouteUrl);
            }
            else
            {
                if (Store.TopLinks.Count > 1)
                    Nav.NavigateTo(Store.TopLinks[index - 1].RouteUrl);
                else if (Store.TopLinks.Count == 1)
                    Nav.NavigateTo(Store.TopLinks[0].RouteUrl);
            }
        }

        private void OpenContextMenu(MouseEventArgs e, TagRoute current)
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
            await Store.RemoveOther(current.RouteUrl);
            await CloseMenu();
        }

        private async Task CloseAll()
        {
            await Store.Reset();
            Nav.NavigateTo("/");
            await CloseMenu();
        }

        private async Task CloseSelf()
        {
            if (current == null) return;
            CloseTag(current);
            await CloseMenu();
        }

        private Task CloseMenu()
        {
            if (showContextmenu)
                showContextmenu = false;
            RootLayout.BodyClickEvent -= RootLayout_BodyClickEvent;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Store.DataChangedEvent -= StateHasChanged;
            RootLayout.BodyClickEvent -= RootLayout_BodyClickEvent;
        }
    }
}
