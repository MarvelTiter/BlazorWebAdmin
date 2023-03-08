using BlazorWeb.Shared.Layouts.LayoutComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Project.AppCore.Store;

namespace BlazorWeb.Shared.Layouts.Layouts
{
    public class ContainerBase : ComponentBase
    {
        [Parameter] public string Class { get; set; }
        [Parameter] public string Style { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
    }
    public class LayoutBase : ComponentBase
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Inject] public AppStore App { get; set; }
        [Inject] public ProtectedLocalStorage Storage { get; set; }
        public Banner? Banner { get; set; }
        public SideBar? SideBar { get; set; }
        public void HandleToggleCollapse(bool newState)
        {
            SideBar?.ToggleCollapse(newState);
            App.Collapsed = newState;
            _ = Storage.SetAsync(AppStore.KEY, App);
        }

        public void UpdateCollapse(bool state)
        {
            _ = Banner?.UpdateToggleMenu(state);
        }

        public int MainWidthOffset
        {
            get
            {
                return App.Mode switch
                {
                    LayoutMode.Card => App.Collapsed ? 80 : App.SideBarExpandWidth + 16,
                    LayoutMode.Classic => App.Collapsed ? 80 : App.SideBarExpandWidth,
                    _ => 0
                };
            }
        }
    }
}
