namespace Project.Constraints.UI.Table;

public class ColumnItemContext(object instance, IColumnInfo col)
{
    public object Instance { get; set; } = instance;
    public IColumnInfo Column { get; set; } = col;

    public object? GetValue() => Column.GetValue(Instance);
    public T? GetValue<T>()
    {
        if (Column.GetValue(Instance) is T t)
        {
            return t;
        }
        return default;
    }
    public void SetValue(object val) => Column.SetValue(Instance, val);
}
