using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Project.Constraints.Common;
using Project.Constraints.UI.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project.Constraints.UI.Form;

public interface IFormBuilder<TData>
{
    //IFormBuilder<TData> Text<TField>(string label, Expression<Func<TData, TField>> fieldSelector);
    //IFormBuilder<TData> Number<TField>(string label, Expression<Func<TData, TField>> fieldSelector);
    //IFormBuilder<TData> Select<TField>(string label, Expression<Func<TData, TField>> fieldSelector, IEnumerable<TField> sources);
    //IFormBuilder<TData> DateTime<TField>(string label, Expression<Func<TData, TField>> fieldSelector);
    IFormBuilder<TData> AddField<TField>(string label
        , Expression<Func<TData, TField>> fieldSelector
        , int? row = null
        , Dictionary<string, string>? selectSource = null);
    RenderFragment Render();
}

public sealed class FluentFormBuilder<TData>(IUIService ui, TData data, string? formName)
    : IFormBuilder<TData>
    where TData : class, new()
{
    //public IFormBuilder<TData> DateTime<TField>(string label, Expression<Func<TData, TField>> fieldSelector)
    //{
    //    //fieldSelector.ExtractProperty
    //    return this;
    //}

    //public IFormBuilder<TData> Number<TField>(string label, Expression<Func<TData, TField>> fieldSelector)
    //{
    //    return this;
    //}

    //public IFormBuilder<TData> Select<TField>(string label, Expression<Func<TData, TField>> fieldSelector, IEnumerable<TField> sources)
    //{
    //    return this;
    //}

    //public IFormBuilder<TData> Text<TField>(string label, Expression<Func<TData, TField>> fieldSelector)
    //{
    //    return this;
    //}
    private readonly AutoFormBuilder builder = AutoFormBuilder.Create();
    private FormOptions<TData>? options;
    public IFormBuilder<TData> AddField<TField>(string label
        , Expression<Func<TData, TField>> fieldSelector
        , int? row = null
        , Dictionary<string, string>? selectSource = null)
    {
        var prop = fieldSelector.ExtractProperty();
        builder.AddField(label, prop, col =>
        {
            if (row.HasValue)
            {
                col.Row = row.Value;
            }
            if (selectSource is not null)
            {
                col.EnumValues = selectSource;
            }
        });
        return this;
    }

    public RenderFragment Render()
    {
        options ??= builder.Build(ui, data);
        options.FormName ??= formName;
        return ui.BuildForm(options);
    }
}
