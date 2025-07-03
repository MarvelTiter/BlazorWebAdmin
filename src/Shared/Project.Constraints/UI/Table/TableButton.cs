using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Project.Constraints.UI.Table;

/// <summary>
/// 委托签名<code><![CDATA[public Task<IQueryResult?> MethodImpl(TData data)]]></code>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class TableButtonAttribute : Attribute
{
    public string? Label { get; set; }
    /// <summary>
    /// 委托签名
    /// <code>
    /// public string LabelExpressionImpl(TData data)
    /// </code>
    /// </summary>
    public string? LabelExpression { get; set; }
    public string? Icon { get; set; }
    /// <summary>
    /// 委托签名
    /// <code>
    /// public bool VisibleExpressionImpl(TableButtonContext&lt;TData&gt; ctx)
    /// </code>
    /// </summary>
    public string? VisibleExpression { get; set; }
    /// <summary>
    /// 为<see cref="LabelExpression"/>或者<see cref="VisibleExpression"/>的委托提供的额外的参数
    /// </summary>
    public string? AdditionalParameter { get; set; }
    public string Type { get; set; } = "primary";
    public bool Danger { get; set; }
    public string? ConfirmContent { get; set; }
    public string? ConfirmTitle { get; set; }
    public string? Group { get; set; }
}

/// <summary>
/// 委托签名<code><![CDATA[public Task<IQueryResult?> MethodImpl(TData data)]]></code>
/// </summary>
public class EditButton : TableButtonAttribute
{
    public EditButton()
    {
        Label = "TableButtons.Edit";
        Icon = "edit";
    }
}
/// <summary>
/// 委托签名<code><![CDATA[public Task<IQueryResult?> MethodImpl(TData data)]]></code>
/// </summary>
public class DeleteButton : TableButtonAttribute
{
    public DeleteButton()
    {
        Label = "TableButtons.Delete";
        Icon = "delete";
        Danger = true;
    }
}

public class TableButtonContext<T>(T data)
{
    public T Data { get; set; } = data;
    //public TableButton<T> ButtonDefinition { get; set; }
    public string? AdditionalParameter { get; set; }
}

/// <summary>
/// 委托签名<code><![CDATA[public Task<IQueryResult?> MethodImpl(TData data)]]></code>
/// </summary>
public class TableButton<TData>
{
    public TableButton()
    {

    }
    public TableButton(TableButtonAttribute options)
    {
        Label = options.Label ?? "Button";
        Icon = options.Icon;
        ButtonType = options.Type;
        Danger = options.Danger;
        ConfirmContent = options.ConfirmContent;
        ConfirmTitle = options.ConfirmTitle;
        AdditionalParameter = options.AdditionalParameter;
        Group = options.Group ?? "TableTips.ActionColumn";
    }
    public string Label { get; set; } = "Button";
    public bool Danger { get; set; }
    public string? Icon { get; set; }
    public string? ButtonType { get; set; }
    public string? ConfirmContent { get; set; }
    public string? ConfirmTitle { get; set; }
    public string? AdditionalParameter { get; set; }
    public string Group { get; set; } = "TableTips.ActionColumn";
    [NotNull] public Func<TData, Task<IQueryResult?>>? Callback { get; set; }

    private Func<TableButtonContext<TData>, bool>? visible;
    private Func<TableButtonContext<TData>, string>? label;
    public Func<TableButtonContext<TData>, string>? LabelExpression { set => label = value; }

    public Func<TableButtonContext<TData>, bool>? VisibleExpression { set => visible = value; }
    public bool CheckVisible(TData data)
    {
        var context = new TableButtonContext<TData>(data) { AdditionalParameter = AdditionalParameter };
        return visible?.Invoke(context) ?? true;
    }
    public string? GetLabel(TData data)
    {
        var context = new TableButtonContext<TData>(data) { AdditionalParameter = AdditionalParameter };
        return label?.Invoke(context);
    }

    public static TableButton<TData> Edit(Func<TData, Task<IQueryResult?>> action)
    {
        return new TableButton<TData>
        {
            Label = "TableButtons.Edit",
            Icon = "edit",
            Callback = action
        };
    }

    public static TableButton<TData> Delete(Func<TData, Task<IQueryResult?>> action)
    {
        return new TableButton<TData>
        {
            Label = "TableButtons.Delete",
            Icon = "delete",
            Danger = true,
            Callback = action
        };
    }
}