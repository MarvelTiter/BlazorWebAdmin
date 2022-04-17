namespace BlazorWebAdmin.Template.Tables.Setting
{
    public class ButtonDefinition<TData>
    {
        public string Label { get; set; }
        public string Icon { get; set; }
        public bool NeedConfirm { get; set; } = false;
        public string ButtonType { get; set; } = AntDesign.ButtonType.Text;
        public Func<TData, Task> Callback { get; set; }

        public static ButtonDefinition<TData> Edit(Func<TData, Task> action)
        {
            return new ButtonDefinition<TData>
            {
                Label = "编辑",
                Icon = "edit",
                Callback = action
            };
        }

        public static ButtonDefinition<TData> Delete(Func<TData, Task> action)
        {
            return new ButtonDefinition<TData>
            {
                Label = "删除",
                Icon = "delete",
                NeedConfirm = true,
                Callback = action
            };
        }
    }
}
