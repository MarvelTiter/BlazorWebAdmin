namespace BlazorWebAdmin.Template.Tables.Setting
{
    public class ColumnDefinition
    {
        public string Label { get; set; }
        public int Index { get; set; }
        public string PropertyOrFieldName { get; set; }
        public string DataType { get; set; } = "string";
        public ColumnDefinition(string label, string name)
        {
            Label = label;
            PropertyOrFieldName = name;
        }
    }
}
