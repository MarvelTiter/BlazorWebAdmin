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
        public Func<CellData, Dictionary<string, object>> OnCell { get; set; }
        public Dictionary<string, string>? EnumValues { get; set; }

    }
}
