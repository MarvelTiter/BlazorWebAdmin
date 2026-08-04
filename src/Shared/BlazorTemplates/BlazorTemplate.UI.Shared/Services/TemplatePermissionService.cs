using LightORM;
using AutoAopProxyGenerator;
namespace BlazorTemplate.UI.Shared.Services;

[AutoInject(ServiceType = typeof(ITemplatePermissionService), Group = "SERVER")]
[AutoInject(ServiceType = typeof(IPermissionService), Group = "SERVER")]
[GenAspectProxy]
public class TemplatePermissionService : DefaultPermissionService<TemplatePermission, TemplateRole, TemplateRolePermission, TemplateUserRole>, ITemplatePermissionService
{
    public TemplatePermissionService(IExpressionContext context) : base(context)
    {

    }
}
