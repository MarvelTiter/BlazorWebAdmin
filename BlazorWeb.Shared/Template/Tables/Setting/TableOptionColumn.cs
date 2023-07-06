using AntDesign.TableModels;

namespace BlazorWeb.Shared.Template.Tables.Setting
{
    public record TableOptionColumn(string Label, string PropertyOrFieldName)
    {
        public int Index { get; set; }
        public Type DataType { get; set; } = typeof(string);
        public bool IsEnum => DataType.IsEnum || (UnderlyingType?.IsEnum ?? false);
        public bool Nullable => (System.Nullable.GetUnderlyingType(DataType) ?? null) != null;
        public Type? UnderlyingType => System.Nullable.GetUnderlyingType(DataType);
        public string? Fixed { get; set; }
        public string? Width { get; set; }
        public bool EnableEdit { get; set; }
        public bool Ellipsis { get; set; }
        public bool Visible { get; set; } = true;
        public bool UseTag { get; set; }
        public Func<CellData, Dictionary<string, object>> OnCell { get; set; }
        public Dictionary<string, string>? EnumValues { get; set; }

        internal Dictionary<string, string> TagColors { get; set; }
        public string GetTagColor(object? val)
        {
            if (TagColors?.TryGetValue(val?.ToString() ?? "", out var color) ?? false)
            {
                return color;
            }
            return "Blue";
        }

    }
}
