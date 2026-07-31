using Microsoft.AspNetCore.Components;
using BlazorTemplate.Constraints.Common;
using BlazorTemplate.Constraints.Models.Request;
using BlazorTemplate.Constraints.UI.Table;
using System.Data;

namespace BlazorTemplate.Constraints.UI.Extensions;

public static class BuildTableExtensions
{
    public static RenderFragment BuildDynamicTable(this IUIService service, DataTable dataSource)
    {
        var options = new TableOptions<DataRow, GenericRequest>();
        options.LoadDataOnLoaded = true;
        options.OnQueryAsync = request => Task.FromResult(dataSource.ToEnumerable().CollectionResult());
        return service.BuildDynamicTable(options);
    }
}