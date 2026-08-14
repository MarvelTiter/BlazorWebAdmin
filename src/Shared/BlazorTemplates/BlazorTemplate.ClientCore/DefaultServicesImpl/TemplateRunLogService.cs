namespace BlazorTemplate.ClientCore.DefaultServicesImpl;

[AutoInject(Group = "SERVER", ServiceType = typeof(ITemplateRunLogService))]
[AutoInject(Group = "SERVER", ServiceType = typeof(IRunLogService))]
public class TemplateRunLogService : DefaultRunLogService<TemplateRunLog>, ITemplateRunLogService
{
    public TemplateRunLogService(IExpressionContext context) : base(context)
    {
    }

}
