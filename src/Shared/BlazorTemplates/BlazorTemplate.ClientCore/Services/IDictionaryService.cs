using AutoAopProxyGenerator;
using AutoWasmApiGenerator;
using BlazorTemplate.ClientCore.Aop;
using BlazorTemplate.ClientCore.Models.Dictionary;

namespace BlazorTemplate.ClientCore.Services;

public class DictRequestContext<TDictItem>(GenericRequest<TDictItem> request, TDictItem? parent)
{
    public GenericRequest<TDictItem> Request { get; } = request;
    public TDictItem? Parent { get; } = parent;
}

/// <summary>
/// 数据字典服务（单表自关联，无限级树）。根节点即「字典类型」。
/// </summary>
public interface IDictionaryService<TDictItem>
    where TDictItem : class, IDictItem, new()
{
    [IgnoreAspect]
    Task<QueryCollectionResult<TDictItem>> GetDictTypesAsync();

    [IgnoreAspect]
    Task<QueryCollectionResult<TDictItem>> GetAllItemAsync();

    [IgnoreAspect]
    Task<QueryCollectionResult<TDictItem>> GetDictItemListAsync(DictRequestContext<TDictItem> query);

    [LogInfo(Action = "新增字典节点", Module = "字典管理")]
    Task<QueryResult> InsertDictItemAsync(TDictItem item);

    [LogInfo(Action = "修改字典节点", Module = "字典管理")]
    Task<QueryResult> UpdateDictItemAsync(TDictItem item);

    [LogInfo(Action = "删除字典节点", Module = "字典管理")]
    Task<QueryResult> DeleteDictItemAsync(TDictItem item);
}

/// <summary>
/// 模板内置字典服务，注册为 Web API 供客户端调用。
/// </summary>
[WebController(Route = "template/dict", Authorize = true)]
[AddAspectHandler(AspectType = typeof(AopLogger))]
[AddAspectHandler(AspectType = typeof(AopPermissionCheck))]
public interface ITemplateDictionaryService : IDictionaryService<TemplateDictItem>
{
}
