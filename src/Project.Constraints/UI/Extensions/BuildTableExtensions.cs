using Microsoft.AspNetCore.Components;
using Project.Constraints.Common;
using Project.Constraints.Models.Request;
using Project.Constraints.UI.Table;
using System.Data;

namespace Project.Constraints.UI.Extensions
{
    public static class BuildTableExtensions
    {
        public static RenderFragment BuildDynamicTable(this IUIService service, DataTable dataSource)
        {
            var options = new TableOptions<DataRow, GenericRequest>();
            options.LoadDataOnLoaded = true;
            options.OnQueryAsync = request => Task.FromResult(dataSource.ToEnumerable().CollectionResult());
            return service.BuildDynamicTable(options, dataSource);
        }
    }
}
