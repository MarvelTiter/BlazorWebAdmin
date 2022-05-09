using AntDesign.TableModels;

namespace BlazorWebAdmin.Template.Tables.Setting
{
    public class ColumnDefinition
    {
        public string Label { get; set; }
        public int Index { get; set; }
        public string PropertyOrFieldName { get; set; }
        public Type DataType { get; set; } = typeof(string);
        public string? Fixed { get; set; }
        public string? Width { get; set; }
        public Func<CellData, Dictionary<string,object>> OnCell { get; set; }
        public Func<RowData, Dictionary<string,object>> OnRow { get; set; }
        public Dictionary<string, string> EnumValues { get; set; }
        public ColumnDefinition(string label, string name)
        {
            Label = label;
            PropertyOrFieldName = name;
        }
    }
}
