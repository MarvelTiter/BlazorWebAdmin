using AutoPageStateContainerGenerator;
using BlazorTemplate.ClientCore.Models.Dictionary;
using BlazorTemplate.ClientCore.UI.Extensions;

namespace BlazorTemplate.ClientCore.BuildInPages;

/// <summary>
/// 字典管理页（单表自关联，无限级树）。
/// <para>根节点即「字典类型」，其下为多级字典项。</para>
/// </summary>
public partial class DictPage<TDictItem, TDictionaryService> : ModelPage<TDictItem, GenericRequest<TDictItem>>
    where TDictItem : class, IDictItem, new()
    where TDictionaryService : IDictionaryService<TDictItem>
{
    [Inject, NotNull] public TDictionaryService? DictSrv { get; set; }
    [Inject, NotNull] public IStringLocalizer<TDictItem>? Localizer { get; set; }
    private string typeFilter = string.Empty;
    private IList<TDictItem> dictTypes = [];
    protected TDictItem? current;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        Options.Pager = false;
        Options.LoadDataOnLoaded = true;
        Options.TreeChildren = p => p.Children?.Cast<TDictItem>() ?? [];
    }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RefreshTypeListAsync();
    }

    protected override object SetRowKey(TDictItem model) => model.ItemId;

    protected override async Task<QueryCollectionResult<TDictItem>> OnQueryAsync(GenericRequest<TDictItem> query)
    {
        if (current is null)
        {
            return QueryResult.EmptyResult<TDictItem>();
        }
        return await DictSrv.GetDictItemListAsync(new(query, current));
    }

    protected override async Task<IQueryResult?> OnAddItemAsync()
    {
        if (current is null)
        {
            return QueryResult.Fail("先在左侧选择类型");
        }
        var newValue = new TDictItem()
        {
            ItemId = Guid.NewGuid().ToString("N"),
            TypeCode = current.TypeCode,
            ParentId = current.ItemId,
        };
        await this.ShowEditFormAsync(newValue, config: config =>
        {
            config.Title = "新增字典项";
            config.PostCheckAsync = async (value, validate) =>
            {
                if (value is null) return false;
                if (!validate()) return false;
                var result = await DictSrv.InsertDictItemAsync(value);
                UI.ShowError(result);
                return result.IsSuccess;
            };
        });
        await Options.RefreshAsync();
        return QueryResult.Null();
    }

    [EditButton]
    public async Task<IQueryResult> EditDictItem(TDictItem item)
    {
        var edit = await this.ShowEditFormAsync("编辑字典项", item);
        return await DictSrv.UpdateDictItemAsync(edit);
    }

    [DeleteButton]
    public async Task<IQueryResult> DeleteDictItem(TDictItem item)
    {
        return await DictSrv.DeleteDictItemAsync(item);
    }

    private async Task EditTypeAsync(TDictItem item)
    {
        await UI.ShowFormDialogAsync(item, Options.Columns.Where(c => c.PropertyOrFieldName == nameof(IDictItem.TypeCode) || c.PropertyOrFieldName == nameof(IDictItem.ItemName)), config: c =>
        {
            c.Title = "修改字典类型";
            c.PostCheckAsync = async (value, validate) =>
            {
                if (value is null) return false;
                if (!validate()) return false;
                var result = await DictSrv.InsertDictItemAsync(value);
                UI.ShowResult(result);
                return result.IsSuccess;
            };
        });
        await RefreshTypeListAsync();
    }

    private async Task DeleteTypeAsync(TDictItem item)
    {
        var result = await DictSrv.DeleteDictItemAsync(item);
        UI.ShowResult(result);
        await RefreshTypeListAsync();
    }

    public string AddChildLabel(TableButtonContext<TDictItem> _) => Localizer["DictionarySetting.AddChild"];

    private async Task AddDictTypeAsync()
    {
        await UI.ShowFormDialogAsync<TDictItem>(null, Options.Columns.Where(c => c.PropertyOrFieldName == nameof(IDictItem.TypeCode) || c.PropertyOrFieldName == nameof(IDictItem.ItemName)), config: c =>
        {
            c.Title = "添加字典类型";
            c.PostCheckAsync = async (value, validate) =>
            {
                if (value is null) return false;
                if (!validate()) return false;
                value.ParentId = "ROOT";
                value.ItemId = Guid.NewGuid().ToString("N");
                var result = await DictSrv.InsertDictItemAsync(value);
                UI.ShowResult(result);
                return result.IsSuccess;
            };
        });
        await RefreshTypeListAsync();
    }

    private async Task RefreshTypeListAsync()
    {
        var result = await DictSrv.GetDictTypesAsync();
        UI.ShowResult(result);
        dictTypes = [.. result.Payload];
    }
}

[StateContainer]
public partial class TemplateDictPage
    : DictPage<TemplateDictItem, ITemplateDictionaryService>
{
}
