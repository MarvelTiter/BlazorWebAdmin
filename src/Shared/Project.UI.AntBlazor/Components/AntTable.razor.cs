using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Project.Constraints.Models.Request;
using Project.Constraints.UI;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Table;
using Project.Web.Shared.Components;
using System.Globalization;

namespace Project.UI.AntBlazor.Components;

public partial class AntTable<TData, TQuery> where TQuery : IRequest, new()
{
    private RenderFragment<GroupResult<TData>>? GroupTitle()
    {
        if (Options.GroupTitleTemplate is null)
        {
            return null;
        }

        return ctx => Options.GroupTitleTemplate.Invoke(new Grouping<object, TData>(ctx.Key, [.. ctx.Items]));
    }

    private RenderFragment<GroupResult<TData>> GroupFooter()
    {
        if (Options.GroupFooterTemplate is null)
        {
            return null!;
        }
        return ctx => Options.GroupFooterTemplate.Invoke(new Grouping<object, TData>(ctx.Key, [.. ctx.Items]));
    }

    private (int, int) editCell = (-1, -1);
    private int editRow = -1;
    private RenderFragment CellContentRender<TProp>(TData row, ColumnInfo col, CellData<TProp> context)
    {
        var enableEdit = !col.Readonly && (col.Editable ?? Options.EnableRowEdit);

        if (enableEdit && (Options.OnCellUpdateAsync is not null || Options.OnRowUpdateAsync is not null))
        {
            if (Options.OnCellUpdateAsync is not null && editCell == (col.ColumnIndex, context.RowData.RowIndex))
            {
                return EditModeFragment(true);
            }
            else if (Options.OnRowUpdateAsync is not null && editRow == context.RowData.RowIndex)
            {
                return EditModeFragment(false);
            }
            else
            {
                return DisplayFragmentWithWrap();
            }
        }
        else
        {
            return DisplayFragment();
        }

        RenderFragment EditModeFragment(bool cellEdit)
        {
            return builder => builder.Component<AntTableCellEdit<TData>>()
            .SetComponent(c => c.Column, col)
            .SetComponent(c => c.Data, row)
            .SetComponent(c => c.Reciver, this)
            .SetComponent(c => c.UI, UI)
            .SetComponent(c => c.CellEdit, cellEdit)
            .SetComponent(c => c.OnSave, EventCallback.Factory.Create(this, SaveCellEdit))
            .SetComponent(c => c.OnCancel, EventCallback.Factory.Create(this, ResetEditCell))
            .Build();
        }

        RenderFragment DisplayFragment()
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
            else if (col.Format is not null && context.FieldValue is IFormattable ft)
            {
                formattedValue = ft.ToString(col.Format, CultureInfo.CurrentUICulture);
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
            }
            else if ((col.UnderlyingType ?? col.DataType) == typeof(bool))
            {
                return builder => builder.Component<Switch>().SetComponent(c => c.Disabled, true)
                    .SetComponent(c => c.Checked, CastToBool(context.FieldValue)).Build();
            }

            return builder => builder.Span().AddContent(context.FormattedValue.AsContent()).Build();
        }

        RenderFragment DisplayFragmentWithWrap()
        {
            return builder => builder.Div()
            .Set("class", "editable-cell")
            .Set("onclick", EventCallback.Factory.Create(this, () =>
            {
                editCell = (col.ColumnIndex, context.RowData.RowIndex);
            })).AddContent(DisplayFragment()).Build();
        }

        void ResetEditCell() => editCell = (-1, -1);

        async Task SaveCellEdit()
        {
            if (Options.OnCellUpdateAsync is not null)
            {
                var r = await Options.OnCellUpdateAsync(row, col);
                if (r is null) return;
                UI.ShowResult(r);
                ResetEditStatus();
            }
        }
    }
    private async Task SaveRowEdit(TData row)
    {
        if (Options.OnRowUpdateAsync is not null)
        {
            ColumnInfo[] cols = [.. Options.Columns.Where(col => !col.Readonly && (col.Editable ?? Options.EnableRowEdit))];
            var r = await Options.OnRowUpdateAsync(row, cols);
            if (r is null) return;
            UI.ShowResult(r);
            ResetEditStatus();
        }
    }
    private static bool CastToBool(object? v)
    {
        if (v is bool b) return b;
        return false;
    }

    private RenderFragment<CellData<T>> ColumnRender<T>(TData row, ColumnInfo col) => context => CellContentRender(row, col, context);

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