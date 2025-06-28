using AutoGenMapperGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Localization;
using MT.Toolkit.Mapper;
using Project.Constraints.Page;
using Project.Constraints.UI.Extensions;

namespace Project.Constraints.UI.Flyout;

public class FormParam<TEntity>(TEntity? entity, bool? edit)
{
    public TEntity? Value { get; set; } = entity;
    public bool Edit { get; set; } = edit ?? entity != null;
}
public class DialogTemplate<TInput, TReturn> : JsComponentBase, IFeedback<TReturn>
{
    [Parameter, NotNull] public FormParam<TInput>? DialogModel { get; set; }
    [Parameter, NotNull] public RenderFragment<TInput?>? ChildContent { get; set; }
    [Parameter, NotNull] public FlyoutOptions<TReturn>? Options { get; set; }
    [Inject, NotNull] protected IStringLocalizer<TInput>? Localizer { get; set; }
    protected string GetLocalizeString(string prop) => Localizer[$"{typeof(TInput).Name}.{prop}"];

    protected TInput? Value
    {
        get
        {
            return DialogModel.Value;
        }
        set
        {
            DialogModel.Value = value;
        }
    }

    protected virtual TReturn? ReturnValue { get; set; }

    protected bool Edit => DialogModel.Edit;
    protected override void OnInitialized()
    {
        base.OnInitialized();
        LoadJs = false;
        var valueType = typeof(TInput);
        if (DialogModel == null) return;
        if (DialogModel.Value == null && valueType.IsClass && valueType != typeof(string))
        {
            DialogModel.Value = (TInput)Activator.CreateInstance(valueType)!;
        }
        else
        {
            if (DialogModel.Value is IAutoMap map)
            {
                DialogModel.Value = map.MapTo<TInput>();
            }
            //DialogModel.Value = Mapper.Map<TInput, TInput>(DialogModel.Value);
        }
    }

    public virtual async Task<bool> OnPostAsync()
    {
        if (Options.PostCheckAsync == null) return true;
        var flag = await Options.PostCheckAsync.Invoke(ReturnValue, static () => true);
        return flag;
    }

    protected Task CloseAsync()
    {
        return Options.OnClose?.Invoke() ?? Task.CompletedTask;
    }

    protected Task OkAsync()
    {
        return Options.OnOk?.Invoke() ?? Task.CompletedTask;
    }

    public Task OnCancelAsync()
    {
        return Task.CompletedTask;
    }

    public async Task<FeedBackValue<TReturn>> OnOkAsync()
    {
        var flag = await OnPostAsync();
        return new FeedBackValue<TReturn> { Value = ReturnValue, Success = flag };
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (ChildContent != null)
        {
            builder.Component<CascadingValue<bool>>()
                .SetComponent(c => c.Value, Edit)
                .SetComponent(c => c.ChildContent, b =>
                {
                    ChildContent.Invoke(Value).Invoke(b);
                }).Build();
        }
    }
}

public class DialogTemplate<TValue> : DialogTemplate<TValue, TValue>
{
    protected override TValue? ReturnValue { get => Value; set => Value = value; }
}