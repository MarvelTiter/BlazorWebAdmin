using AntDesign.TableModels;

namespace BlazorWeb.Shared.Template.Tables.Setting
{
    public record ColumnDefinition(string Label, string PropertyOrFieldName)
    {
        public int Index { get; set; }
        public Type DataType { get; set; } = typeof(string);
        public bool IsEnum => DataType.IsEnum;
        public string? Fixed { get; set; }
        public string? Width { get; set; }
        public bool EnableEdit { get; set; }
        public bool Visible { get; set; } = true;
        public Func<CellData, Dictionary<string, object>> OnCell { get; set; }
        public Dictionary<string, string>? EnumValues { get; set; }

    }
}
