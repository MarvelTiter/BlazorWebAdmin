using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTemplate.ClientCore.Lookup;

public class DictionaryLookupProvider<TDictItem>(IDictionaryService<TDictItem> dictionaryService) : BaseLookupProvider
    where TDictItem : class, IDictItem, new()
{
    public override string TypeName => LookupTypes.Dictionary;

    protected override TimeSpan RefreshInterval => TimeSpan.FromMinutes(2);

    protected override async ValueTask<IList<LookupCategory>> LoadDataAsync(CancellationToken cancellationToken)
    {
        var result = await dictionaryService.GetAllItemAsync();
        var values = result.Payload;
        return [.. values.GroupBy(t => t.TypeCode).Select(g => new LookupCategory(g.Key, g.Select(gg => new LookupEntry(gg.ItemValue, gg.ItemName))))];
    }
}
