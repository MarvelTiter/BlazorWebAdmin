using BlazorTemplate.UI.Shared.Services;
using LightORM;

namespace BlazorTemplate.AppCore.Services;

[AutoInject(Group = "SERVER", ServiceType = typeof(ITemplateRunLogService))]
[AutoInject(Group = "SERVER", ServiceType = typeof(IRunLogService))]
public class TemplateRunLogService : DefaultRunLogService<TemplateRunLog>, ITemplateRunLogService
{
    public TemplateRunLogService(IExpressionContext context) : base(context)
    {
    }

}
