using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Project.Constraints.UI.Table
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TableButtonAttribute : Attribute
    {
        public string Label { get; set; }
        /// <summary>
        /// 委托签名
        /// <code>
        /// public string LabelExpressionImpl(TData data)
        /// </code>
        /// </summary>
        public string? LabelExpression { get; set; }
        public string Icon { get; set; }
        /// <summary>
        /// 委托签名
        /// <code>
        /// public bool VisibleExpressionImpl(TData data)
        /// </code>
        /// </summary>
        public string? VisibleExpression { get; set; }
        public string Type { get; set; } = "primary";
        public bool Danger { get; set; }
        public string? ConfirmContent { get; set; }
        public string? ConfirmTitle { get; set; }
    }

    /// <summary>
    /// 委托签名
    /// <code>
    /// public Task&lt;bool&gt; EditMethodImpl(TData data)
    /// </code>
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
    /// 委托签名
    /// <code>
    /// public Task&lt;bool&gt; DeleteMethodImpl(TData data)
    /// </code>
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

    /// <summary>
    /// 委托签名
    /// <code>
    /// Task&lt;bool&gt; AddMethodImpl()
    /// </code>
    /// </summary>
    public class AddButton : TableButtonAttribute
    {
        public AddButton()
        {
            Label = "TableButtons.Add";
            Icon = "plus";
        }
    }

    /// <summary>
    /// 委托签名
    /// <code>
    /// public Task&lt;bool&gt; MethodImpl(TData data)
    /// </code>
    /// </summary>
    public class TableButton<TData>
    {
        public TableButton()
        {

        }
        public TableButton(TableButtonAttribute options)
        {
            Label = options.Label;
            Icon = options.Icon;
            ButtonType = options.Type;
            Danger = options.Danger;
            ConfirmContent = options.ConfirmContent;
            ConfirmTitle = options.ConfirmTitle;
        }
        public string Label { get; set; }
        public Func<TData, string>? LabelExpression { get; set; }
        public bool Danger { get; set; }
        public string Icon { get; set; }
        public string ButtonType { get; set; }
        public string? ConfirmContent { get; set; }
        public string? ConfirmTitle { get; set; }
        public Func<TData, Task<bool>> Callback { get; set; }
        public Func<TData, bool> Visible { get; set; } = t => true;

        public static TableButton<TData> Edit(Func<TData, Task<bool>> action)
        {
            return new TableButton<TData>
            {
                Label = "TableButtons.Edit",
                Icon = "edit",
                Callback = action
            };
        }

        public static TableButton<TData> Delete(Func<TData, Task<bool>> action)
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
}
