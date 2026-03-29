using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Form;

public enum RowCapacity
{
    One = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Six = 6,
    Eight = 8,
}
public class FormOptions<TData>(IUIService ui, TData? data, List<ColumnInfo> columns) where TData : class, new()
{
    public string? FormName { get; set; }
    public IUIService UI { get; } = ui;
    public TData Data { get; } = data ?? new TData();
    public ColumnInfo[] Columns { get; } = [.. columns];
    public int LabelSpan { get; set; } = 6;
    public int WrapperSpan { get; set; }
    public Func<TData, Task<bool>>? OnPostAsync { get; set; }
    public Func<bool>? Validate { get; set; }
    public Action? Update { get; set; }
    public RowCapacity? RowCapacity { get; set; }

    //private List<ColumnInfo[]>? rows;
    public IReadOnlyList<ColumnInfo[]> GetRows()
    {
        //if (rows is not null)
        //{
        //    return rows;
        //}
        List<ColumnInfo[]> rows = [];
        var settedRow = Columns.Where(c => c.Row.HasValue && c.Row > 0).GroupBy(c => c.Row!.Value).OrderBy(c => c.Key);
        foreach (var row in settedRow)
        {
            rows.Add([.. row.OrderBy(c => c.Index)]);
        }
        var defaultRows = Columns.Where(c => !c.Row.HasValue || c.Row == 0 && c.ShowOnForm).OrderBy(c => c.Index).ToList();
        //var defaultRows = new Stack<ColumnInfo>(Columns.Where(c => !c.Row.HasValue || c.Row == 0 && c.ShowOnForm));

        if (RowCapacity.HasValue)
        {
            // 有容量限制：按容量分配到现有行之后或创建新行
            var capacity = (int)RowCapacity.Value;
            for (int i = 0; i < defaultRows.Count; i += capacity)
            {
                var rowItems = defaultRows.Skip(i).Take(capacity).ToArray();
                if (rowItems.Length > 0)
                {
                    rows.Add(rowItems);
                }
            }
        }
        else
        {
            foreach (var column in defaultRows)
            {
                rows.Add([column]);
            }
        }
        return rows;
    }
}