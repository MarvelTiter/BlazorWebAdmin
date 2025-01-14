using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Project.Constraints.Store.Models;
using Project.Constraints.UI;
using Project.Web.Shared.Components;

namespace Project.Web.Shared.Layouts.LayoutComponents
{
    public partial class NavTabs
    {
        private bool showContextmenu = false;
        private string contextmenuLeft = "";
        private string contextmenuTop = "";
        private TagRoute? current;
        private HorizontalScroll? horizontalScroll;
        private ElementReference? leftButton;
        private ElementReference? rightButton;
        [CascadingParameter, NotNull] public IAppDomEventHandler? RootLayout { get; set; }
        [Parameter] public string? Class { get; set; }
        private int navMenuWidth = 200;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Router.DataChangedEvent += StateHasChanged;
            // Router.RouterChangingEvent += RouterOnRouterChangingEvent;
        }

        // private async Task<bool> RouterOnRouterChangingEvent(TagRoute arg)
        // {
        //     await InvokeVoidAsync("checkActiveChanged", arg.RouteId);
        //     return true;
        // }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                navMenuWidth = await InvokeAsync<int>("getMenuWidth");
            }
        }

        protected override async ValueTask Init()
        {
            var scrollWrap = horizontalScroll?.GetWrapElementRef();
            await InvokeVoidAsync("init", new
            {
                tabsContainer = scrollWrap,
                leftButton,
                rightButton,
            });
        }

        private ClassHelper ContextmenuClass => ClassHelper.Default.AddClass("context").AddClass("open", () => showContextmenu);

        private void CloseTag(TagRoute state)
        {
            var index = Router.TopLinks.IndexOf(state);
            Router.Remove(state.RouteUrl);
            if (index < Router.TopLinks.Count)
            {
                Navigator.NavigateTo(Router.TopLinks[index].RouteUrl);
            }
            else
            {
                if (Router.TopLinks.Count > 1)
                    Navigator.NavigateTo(Router.TopLinks[index - 1].RouteUrl);
                else if (Router.TopLinks.Count == 1)
                    Navigator.NavigateTo(Router.TopLinks[0].RouteUrl);
            }
        }

        private void OpenContextMenu(MouseEventArgs e, TagRoute route)
        {
            current = route;
            contextmenuLeft = $"{e.ClientX + 10}px";
            contextmenuTop = $"{e.ClientY + 10}px";
            showContextmenu = true;
            RootLayout.BodyClickEvent += RootLayout_BodyClickEvent;
        }

        private Task RootLayout_BodyClickEvent(MouseEventArgs obj)
        {
            return CloseMenu();
        }

        //private async Task ReLoad()
        //{
        //    if (current == null) return;
        //    await Router.Reload();
        //    await CloseMenu();
        //}

        private async Task CloseOther()
        {
            if (current == null) return;
            await Router.RemoveOther(current.RouteUrl);
            await CloseMenu();
        }

        private async Task CloseAll()
        {
            await Router.Reset();
            Navigator.NavigateTo("/");
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

        protected override ValueTask OnDisposeAsync()
        {
            Router.DataChangedEvent -= StateHasChanged;
            RootLayout.BodyClickEvent -= RootLayout_BodyClickEvent;
            return base.OnDisposeAsync();
        }
    }
}