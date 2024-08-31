﻿using Project.Constraints.UI.Table;

namespace Project.Constraints.UI.Form
{
    public class FormOptions<TData>(IUIService ui, TData? data, List<ColumnInfo> columns) where TData : class, new()
    {
        public IUIService UI { get; } = ui;
        public TData Data { get; } = data ?? new TData();
        public ColumnInfo[] Columns { get; } = [.. columns];
        public int LabelSpan { get; set; } = 6;
        public int WrapperSpan { get; set; }
        public Func<TData, Task<bool>>? OnPostAsync {  get; set; }
        public Func<bool>? Validate { get; set; }
        public Action? Update { get; set; }
        public IEnumerable<ColumnInfo[]> GetRows()
        {
            var rowIndex = 1;
            var settedRow = Columns.Where(c => c.Row.HasValue && c.Row > 0).GroupBy(c => c.Row!.Value);
            var defaultRows = Columns.Where(c => !c.Row.HasValue || c.Row == 0 && c.ShowOnForm);
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
