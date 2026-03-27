using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Form;

public class FormOptions<TData>(IUIService ui, TData? data, List<ColumnInfo> columns) where TData : class, new()
{
    public string? FormName { get; set; }
    public IUIService UI { get; } = ui;
    public TData Data { get; } = data ?? new TData();
    public ColumnInfo[] Columns { get; } = [.. columns];
    public int LabelSpan { get; set; } = 6;
    public int WrapperSpan { get; set; }
    public int? RowCapacity { get; set; }
    public Func<TData, Task<bool>>? OnPostAsync { get; set; }
    public Func<bool>? Validate { get; set; }
    public Action? Update { get; set; }
    public IReadOnlyList<ColumnInfo[]> GetRows()
    {
        var settedRow = Columns.Where(c => c.Row.HasValue && c.Row > 0).GroupBy(c => c.Row!.Value).OrderBy(c => c.Key).ToList();
        var defaultRows = Columns.Where(c => !c.Row.HasValue || c.Row == 0 && c.ShowOnForm);
        var rowEnumerator = defaultRows.GetEnumerator();
        var list = new List<ColumnInfo[]>();
        //while (true)
        //{
        //    if (settedRow.TryGetValue(rowIndex, out ColumnInfo[]? values))
        //    {
        //        yield return values;
        //    }
        //    else
        //    {
        //        if (rowEnumerator.MoveNext())
        //        {
        //            yield return [rowEnumerator.Current];
        //        }
        //        else
        //        {
        //            break;
        //        }
        //    }
        //    rowIndex++;
        //}
        return list;
    }
}