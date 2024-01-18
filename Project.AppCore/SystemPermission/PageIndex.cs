using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Project.AppCore.SystemPermission
{
    public abstract class PageIndex : ComponentBase
    {
        [Inject] ICustomSettingProvider SettingProvider { get; set; }

        protected Type? PageType { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            PageType = GetPageType(SettingProvider);
        }
        public abstract Type? GetPageType(ICustomSettingProvider customSetting);

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
        public override Type? GetPageType(ICustomSettingProvider customSetting)
        {
            return customSetting.GetUserPageType();
        }
    }

    [Route("/operationlog")]
    public class RunLogIndex : PageIndex
    {
        public override Type? GetPageType(ICustomSettingProvider customSetting)
        {
            return customSetting.GetRunLogPageType();
        }
    }

    [Route("/permission")]
    public class PermissionIndex : PageIndex
    {
        public override Type? GetPageType(ICustomSettingProvider customSetting)
        {
            return customSetting.GetPermissionPageType();
        }
    }

    [Route("/rolepermission")]
    public class RolePermissionIndex : PageIndex
    {
        public override Type? GetPageType(ICustomSettingProvider customSetting)
        {
            return customSetting.GetRolePermissionPageType();
        }
    }
}
