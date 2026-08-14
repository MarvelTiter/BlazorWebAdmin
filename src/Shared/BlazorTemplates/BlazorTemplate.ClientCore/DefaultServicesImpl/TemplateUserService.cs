using AutoAopProxyGenerator;

namespace BlazorTemplate.ClientCore.DefaultServicesImpl;

[AutoInject(Group = "SERVER", ServiceType = typeof(ITemplateUserService))]
[GenAspectProxy]
public class TemplateUserService(IExpressionContext context) : DefaultUserService<TemplateUser, TemplateUserRole>(context), ITemplateUserService
{

    public async Task<QueryResult> SavePropertyAsync(TemplateUser user, string property)
    {
        var e = await context.Update(user).UpdateByName(property).ExecuteAsync();
        return e > 0;
    }
    public async Task<QueryResult> SavePropertiesAsync(TemplateUser user, string[] property)
    {
        var e = await context.Update(user).UpdateByNames(property).ExecuteAsync();
        return e > 0;
    }
}
