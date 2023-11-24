using BlazorWeb.Shared.Pages;
using Microsoft.AspNetCore.Components;

namespace BlazorWeb.Shared
{
    public class OverrideDashboard : ComponentBase
    {
        [Inject] IDashboardContentProvider ContentProvider { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            ContentProvider.SetComponentType(GetType());
        }
    }
}
