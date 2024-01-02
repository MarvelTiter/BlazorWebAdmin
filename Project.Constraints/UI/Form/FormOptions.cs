using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Form
{
    public class FormOptions<TData>(IUIService ui, TData data, List<ColumnInfo> columns)
    {
        public IUIService UI { get; } = ui;
        public TData Data { get; } = data;
        public List<ColumnInfo> Columns { get; } = columns;
        public int LabelSpan { get; set; }
        public int WrapperSpan { get; set; }

        public IEnumerable<ColumnInfo[]> GetRows()
        {
            var rowIndex = 0;
            var settedRow = Columns.Where(c => c.Row.HasValue).GroupBy(c => c.Row!.Value);
            var defaultRows = Columns.Where(c => !c.Row.HasValue);
            var rowEnumerator = defaultRows.GetEnumerator();
            while (true)
            {
                if (settedRow.Any(g => g.Key == rowIndex))
                {
                    yield return settedRow.First(g => g.Key == rowIndex).ToArray();
                }
                else
                {
                    if (rowEnumerator.MoveNext())
                    {
                        yield return [rowEnumerator.Current];
                    }
                    else
                    {
                        break;
                    }
                }
                rowIndex++;
            }
        }
    }
}
