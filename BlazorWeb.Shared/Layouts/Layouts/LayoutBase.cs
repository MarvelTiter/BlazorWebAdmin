using BlazorWeb.Shared.Layouts.LayoutComponents;
using Microsoft.AspNetCore.Components;

namespace BlazorWeb.Shared.Layouts.Layouts
{
    public class LayoutBase : ComponentBase
    {
        [CascadingParameter] public MainLayout TopLayout { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        public SideBar? SideBar { get; set; }
        public void HandleToggleCollapse(bool newState)
        {
            SideBar?.ToggleCollapse(newState);
        }
    }
}
