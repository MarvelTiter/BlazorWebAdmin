using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Project.AppCore.SystemPermission
{
    public abstract class PageIndex : ComponentBase
    {
        [Inject] ICustomSettingService SettingProvider { get; set; }

        protected Type? PageType { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            PageType = GetPageType(SettingProvider);
        }
        public abstract Type? GetPageType(ICustomSettingService customSetting);

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (PageType != null)
            {
                builder.OpenComponent(0, PageType);
                builder.CloseComponent();
            }
        }
    }

    [Route("/user/index")]
    public class UserIndex : PageIndex
    {
        public override Type? GetPageType(ICustomSettingService customSetting)
        {
            return customSetting.GetUserPageType();
        }
    }

    [Route("/operationlog")]
    public class RunLogIndex : PageIndex
    {
        public override Type? GetPageType(ICustomSettingService customSetting)
        {
            return customSetting.GetRunLogPageType();
        }
    }

    [Route("/permission")]
    public class PermissionIndex : PageIndex
    {
        public override Type? GetPageType(ICustomSettingService customSetting)
        {
            return customSetting.GetPermissionPageType();
        }
    }

    [Route("/rolepermission")]
    public class RolePermissionIndex : PageIndex
    {
        public override Type? GetPageType(ICustomSettingService customSetting)
        {
            return customSetting.GetRolePermissionPageType();
        }
    }
}
