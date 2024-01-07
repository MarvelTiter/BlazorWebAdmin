using AntDesign;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using System.Data;
using System.Linq.Expressions;
using AntDesign.TableModels;
using Project.Constraints.UI.Table;
using System.Reflection;
using Project.Web.Shared.Extensions;
using System.Collections.Concurrent;
using Project.Constraints.Models.Request;

namespace Project.UI.AntBlazor.Components
{
    public class AntTableColumns<TData, TQuery> : ComponentBase where TQuery : IRequest, new()
    {
        [Parameter] public TableOptions<TData, TQuery> Options { get; set; }
        [Parameter] public IStringLocalizer<TData> Localizer { get; set; }
        [Parameter] public TData RowData { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Options.Columns?.Count == 0)
                return;

            var isDictionary = typeof(TData) == typeof(Dictionary<string, object>);
            var isDataTable = typeof(TData) == typeof(DataRow);
            foreach (var item in Options.Columns!)
            {
                if (!(item.VisibleExpression?.Invoke(RowData) ?? item.Visible))
                    continue;

                if (isDictionary)
                    BuildPropertyColumnDictionary(builder, item);
                else if (isDataTable)
                    BuildPropertyColumnDataTable(builder, item);
                else
                    BuildColumn(builder, item);
            }
        }

        private void BuildPropertyColumnDictionary(RenderTreeBuilder builder, ColumnInfo item)
        {
            builder.OpenComponent<PropertyColumn<Dictionary<string, object>, object>>(0);
            builder.AddAttribute(1, "Property", (Expression<Func<Dictionary<string, object>, object>>)(c => c[item.PropertyOrFieldName]));
            AddColumnParameters(builder, item);
            builder.CloseComponent();
        }

        private void BuildPropertyColumnDataTable(RenderTreeBuilder builder, ColumnInfo item)
        {
            builder.OpenComponent<PropertyColumn<DataRow, object>>(0);
            builder.AddAttribute(1, "Property", (Expression<Func<DataRow, object>>)(c => c[item.PropertyOrFieldName]));
            AddColumnParameters(builder, item);
            builder.CloseComponent();
        }
        static readonly ConcurrentDictionary<Type, MethodInfo> caches = new();
        private void BuildColumn(RenderTreeBuilder builder, ColumnInfo col)
        {
            var propertyType = col.Property.PropertyType.UnderlyingSystemType;
            var columnType = typeof(Column<>).MakeGenericType(propertyType);
            builder.OpenComponent(0, columnType);
            builder.AddAttribute(1, "DataIndex", col.PropertyOrFieldName);
            AddColumnParameters(builder, col);
            builder.CloseComponent();

            //var m = caches.GetOrAdd(propertyType, p =>
            // {
            //     return columnBuilder.MakeGenericMethod(propertyType);
            // });

            //var content = (RenderFragment)m.Invoke(null, [col])!;
            //builder.AddContent(0, content);
        }

        private void AddColumnParameters(RenderTreeBuilder builder, ColumnInfo col)
        {
            builder.AddAttribute(2, "Ellipsis", col.Ellipsis);
            if (col.Width != null)
                builder.AddAttribute(3, "Width", col.Width);
            builder.AddAttribute(4, "Sortable", col.Sortable);
            builder.AddAttribute(5, "Title", RenderColumnTitle(col.Label));
            if (col.Fixed != null)
                builder.AddAttribute(6, "Fixed", col.Fixed);

            var fragment = cellrender.MakeGenericMethod(col.DataType).Invoke(null, [col]);

            builder.AddAttribute(7, "CellRender", fragment);
        }

        static readonly MethodInfo cellrender = typeof(CellRenderHelperHelper).GetMethod("ColumnRender")!;
        static readonly MethodInfo columnBuilder = typeof(CellRenderHelperHelper).GetMethod("BuildColumn")!;

        private string RenderColumnTitle(string key) => Localizer[key];
    }

    public static class CellRenderHelperHelper
    {
        public static RenderFragment BuildColumn<T>(ColumnInfo col)
        {
            return builder =>
            {
                builder.OpenComponent<Column<T>>(0);
                builder.AddAttribute(1, "DataIndex", col.PropertyOrFieldName);
                builder.AddAttribute(2, "Ellipsis", col.Ellipsis);
                if (col.Width != null)
                    builder.AddAttribute(3, "Width", col.Width);
                builder.AddAttribute(4, "Sortable", col.Sortable);
                builder.AddAttribute(5, "Title", (col.Label));
                if (col.Fixed != null)
                    builder.AddAttribute(6, "Fixed", col.Fixed);
                builder.AddAttribute(7, "CellRender", ColumnRender<T>(col));
            };
        }

        public static RenderFragment<CellData<T>> ColumnRender<T>(ColumnInfo col)
        {
            return context => builder =>
            {
                string? formattedValue = null;
                if (col.IsEnum || col.EnumValues != null)
                {
                    var v = context.FieldValue;
                    if (v != null)
                    {
                        if (col.EnumValues?.ContainsKey($"{v}") ?? false)
                        {
                            formattedValue = col.EnumValues?[$"{v}"];
                        }
                    }
                }

                if (col.ValueFormat != null && context.FieldValue != null)
                {
                    formattedValue = col.ValueFormat.Invoke(context.FieldValue);
                }

                if (formattedValue != null)
                {
                    context.FormattedValue = formattedValue;
                }
                if (col.IsEnum || col.UseTag)
                {
                    var color = col.GetTagColor(context.FormattedValue);
                    builder.OpenComponent<Tag>(0);
                    builder.AddAttribute(1, nameof(Tag.Color), color);
                    builder.AddAttribute(2, nameof(Tag.ChildContent), context.FormattedValue.AsContent());
                    builder.CloseComponent();
                }
                else if ((col.UnderlyingType ?? col.DataType) == typeof(bool))
                {
                    builder.OpenComponent<Switch>(0);
                    builder.AddAttribute(1, nameof(Switch.Disabled), true);
                    builder.AddAttribute(2, nameof(Switch.Checked), context.FieldValue);
                    builder.CloseComponent();
                }
                else
                {
                    builder.OpenElement(0, "span");
                    builder.AddContent(1, context.FormattedValue);
                    builder.CloseElement();
                }
            };
        }

    }
}
