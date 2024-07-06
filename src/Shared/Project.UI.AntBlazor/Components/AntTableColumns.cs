using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Linq.Expressions;
using AntDesign.TableModels;
using Project.Constraints.UI.Table;
using System.Reflection;
using System.Collections.Concurrent;
using Project.Constraints.Models.Request;
using Project.Constraints.UI.Extensions;
using System.Diagnostics.CodeAnalysis;

namespace Project.UI.AntBlazor.Components
{
    public class AntTableColumns<TData, TRowData> : ComponentBase
    {
        [Parameter] public TData? Source { get; set; }
        [Parameter] public TRowData? RowData { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {

            var isDictionary = typeof(TRowData) == typeof(Dictionary<string, object>);
            var isDataTable = typeof(TRowData) == typeof(DataRow);
            if (!isDictionary && !isDataTable)
                throw new NotSupportedException($"data type: {typeof(TData)}");

            if (Source is DataTable table)
            {
                var columns = table.Columns;
                foreach (DataColumn item in columns)
                {
                    CellRenderHelperHelper.BuildPropertyColumnDataTable(builder, item);
                }
            }
            else if (RowData is Dictionary<string, object> dictionary)
            {
                var columns = dictionary.Keys;
                foreach (var item in columns)
                {
                    CellRenderHelperHelper.BuildPropertyColumnDictionary(builder, item);
                }
            }
        }
    }

    public static class CellRenderHelperHelper
    {
        public static void BuildPropertyColumnDataTable(RenderTreeBuilder builder, DataColumn item)
        {
            builder.Component<PropertyColumn<DataRow, object>>()
              .SetComponent(c => c.Title, item.ColumnName)
              .SetComponent(c => c.Property, row => row[item])
              .Build();

        }

        public static void BuildPropertyColumnDictionary(RenderTreeBuilder builder, string item)
        {
            builder.Component<PropertyColumn<Dictionary<string, object>, object>>()
                .SetComponent(c => c.Title, item)
                .SetComponent(c => c.Property, dic => dic[item])
                .Build();
        }
    }
}
