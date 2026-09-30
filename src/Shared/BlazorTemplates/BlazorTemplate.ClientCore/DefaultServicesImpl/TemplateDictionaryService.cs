using BlazorTemplate.ClientCore.Models.Dictionary;

namespace BlazorTemplate.ClientCore.DefaultServicesImpl;

[AutoInject(Group = "SERVER", ServiceType = typeof(ITemplateDictionaryService))]
[AutoInject(Group = "SERVER", ServiceType = typeof(IDictionaryService<TemplateDictItem>))]
public class TemplateDictionaryService
    : DefaultDictionaryService<TemplateDictItem>, ITemplateDictionaryService
{
    public TemplateDictionaryService(IExpressionContext context) : base(context)
    {
    }
}
