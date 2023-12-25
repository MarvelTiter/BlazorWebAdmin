﻿namespace Project.AppCore.UI.Table
{
    public class TableButton<TData>
    {
        public string Label { get; set; }
        public Func<TData, string>? LabelFunc { get; set; }
        public string Icon { get; set; }
        public bool NeedConfirm { get; set; } = false;
        public string? ConfirmTitle { get; set; }
        public string? ConfirmContent { get; set; }
        public Func<TData, Task<bool>> Callback { get; set; }
        public Func<TData, bool> Visible { get; set; } = t => true;

        public static TableButton<TData> Edit(Func<TData, Task<bool>> action)
        {
            return new TableButton<TData>
            {
                Label = "TableButtons.Edit",
                Icon = "edit",
                Callback = action
            };
        }

        public static TableButton<TData> Delete(Func<TData, Task<bool>> action)
        {
            return new TableButton<TData>
            {
                Label = "TableButtons.Delete",
                Icon = "delete",
                NeedConfirm = true,
                Callback = action
            };
        }
    }
}
