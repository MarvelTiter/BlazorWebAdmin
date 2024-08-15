using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Project.Constraints.Models.Request;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Table;

namespace Project.UI.AntBlazor.Components;

public partial class AntTable<TData, TQuery> where TQuery : IRequest, new()
{
    private static RenderFragment CellContentRender<TProp>(TData row, ColumnInfo col, CellData<TProp> context)
    {
        if (col.CellTemplate != null)
        {
            return col.CellTemplate.Invoke(new ColumnItemContext(row!, col));
        }

        string? formattedValue = null;
        if (col.IsEnum || col.EnumValues != null)
        {
            var v = context.FieldValue;
            if (v is not null)
            {
                if (col.EnumValues?.ContainsKey($"{v}") ?? false)
                {
                    formattedValue = col.EnumValues?[$"{v}"];
                }
            }
        }

        if (col.ValueFormat != null && context.FieldValue is not null)
        {
            formattedValue = col.ValueFormat.Invoke(context.FieldValue);
        }

        if (formattedValue != null)
        {
            context.FormattedValue = formattedValue;
        }

        if (col.IsEnum || col.UseTag)
        {
            var color = col.GetTagColor(context.FieldValue);
            return builder => builder.Component<Tag>().SetComponent(c => c.Color, color)
                .SetComponent(c => c.ChildContent, context.FormattedValue.AsContent()).Build();
            //@<Tag Color="@color">@context.FormattedValue</Tag>;
        }
        else if ((col.UnderlyingType ?? col.DataType) == typeof(bool))
        {
            return builder => builder.Component<Switch>().SetComponent(c => c.Disabled, true)
                .SetComponent(c => c.Checked, CastToBool(context.FieldValue)).Build();
            //@<Switch Disabled Checked="@CastToBool(context.FieldValue)"></Switch>;
        }

        return builder => builder.Span().AddContent(context.FormattedValue.AsContent()).Build();
        //@<span>@context.FormattedValue</span>;
    }

    private static bool CastToBool(object? v)
    {
        if (v is bool b) return b;
        return false;
    }

    private static RenderFragment<CellData<T>> ColumnRender<T>(TData row, ColumnInfo col) => context => CellContentRender(row, col, context);


    private static Func<CellData, Dictionary<string, object>> GetOnCell(ColumnInfo col)
    {
        if (col.AddCellOptions != null)
        {
            return c => col.AddCellOptions.Invoke(c.FormattedValue);
        }
        else
        {
            return c => null!;
        }
    }
}