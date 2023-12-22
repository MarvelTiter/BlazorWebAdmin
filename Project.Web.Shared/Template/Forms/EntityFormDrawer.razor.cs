using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorWeb.Shared.Template.Forms
{
    public partial class EntityFormDrawer<TEntity> : EntityFormTemplate<TEntity> where TEntity : class, new()
    {
        async void OnDrawerClose()
        {
            DrawerRef<TEntity>? drawerRef = FeedbackRef as DrawerRef<TEntity>;
            await drawerRef!.CloseAsync(Value);
        }
    }
}
