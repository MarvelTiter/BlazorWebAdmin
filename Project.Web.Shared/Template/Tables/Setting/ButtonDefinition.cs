namespace BlazorWeb.Shared.Template.Tables.Setting
{
    public class ButtonDefinition<TData>
    {
        public string Label { get; set; }
        public Func<TData, string>? LabelFunc { get; set; }
        public string Icon { get; set; }
        public bool NeedConfirm { get; set; } = false;
        public string? ConfirmTitle { get; set; }
        public string? ConfirmContent { get; set; }
        public string ButtonType { get; set; } = AntDesign.ButtonType.Primary;
        public Func<TData, Task<bool>> Callback { get; set; }
        public Func<TData, bool> Visible { get; set; } = t => true;

        public static ButtonDefinition<TData> Edit(Func<TData, Task<bool>> action)
        {
            return new ButtonDefinition<TData>
            {
                Label = "TableButtons.Edit",
                Icon = "edit",
                Callback = action
            };
        }

        public static ButtonDefinition<TData> Delete(Func<TData, Task<bool>> action)
        {
            return new ButtonDefinition<TData>
            {
                Label = "TableButtons.Delete",
                Icon = "delete",
                NeedConfirm = true,
                Callback = action
            };
        }
    }
}
