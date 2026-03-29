using Project.Constraints.UI.Table;
using System.Reflection;

namespace Project.Constraints.UI.Form;

public sealed class AutoFormBuilder
{
    internal AutoFormBuilder()
    {
    }

    public static AutoFormBuilder Create()
    {
        return new AutoFormBuilder();
    }

    public static AutoFormBuilder Create<TData>()
    {
        var builder = new AutoFormBuilder();
        builder.columns.AddRange(typeof(TData).GenerateColumns());

        // foreach (var item in props)
        // {
        //     var form = item.GetCustomAttribute<FormAttribute>();
        //     if (form?.Hide == true) continue;
        //     builder.AddField($"{typeof(TData).Name}.{item.Name}", item);
        // }

        return builder;
    }

    private readonly List<ColumnInfo> columns = [];

    public AutoFormBuilder AddField(string label, PropertyInfo property, Action<ColumnInfo>? configure = null)
    {
        var col = new ColumnInfo(property)
        {
            Label = label,
            ColumnIndex = columns.Count,
        };
        configure?.Invoke(col);
        columns.Add(col);
        return this;
    }

    public FormOptions<TData> Build<TData>(IUIService ui, TData data) where TData : class, new()
    {
        return new FormOptions<TData>(ui, data, columns);
    }
}