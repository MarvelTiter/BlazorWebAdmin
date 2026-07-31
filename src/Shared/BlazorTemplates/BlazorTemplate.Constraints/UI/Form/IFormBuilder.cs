using Microsoft.AspNetCore.Components;
using BlazorTemplate.Constraints.Common;
using System.Linq.Expressions;

namespace BlazorTemplate.Constraints.UI.Form;

public interface IFormBuilder<TData>
    where TData : class, new()
{
    //IFormBuilder<TData> Text<TField>(string label, Expression<Func<TData, TField>> fieldSelector);
    //IFormBuilder<TData> Number<TField>(string label, Expression<Func<TData, TField>> fieldSelector);
    //IFormBuilder<TData> Select<TField>(string label, Expression<Func<TData, TField>> fieldSelector, IEnumerable<TField> sources);
    //IFormBuilder<TData> DateTime<TField>(string label, Expression<Func<TData, TField>> fieldSelector);
    IFormBuilder<TData> AddField<TField>(string label
        , Expression<Func<TData, TField>> fieldSelector
        , int? row = null
        , Dictionary<string, string>? selectSource = null);
    IFormBuilder<TData> Options(Action<FormOptions<TData>> action);
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
    private Action<FormOptions<TData>>? action;
    public IFormBuilder<TData> Options(Action<FormOptions<TData>> action)
    {
        this.action = action;
        return this;
    }

    public RenderFragment Render()
    {
        options ??= builder.Build(ui, data);
        options.FormName ??= formName;
        action?.Invoke(options);
        return ui.BuildForm(options);
    }
}
