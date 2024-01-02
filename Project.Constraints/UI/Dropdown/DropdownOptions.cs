using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Dropdown
{
    public class ActionInfo
    {
        public string Label { get; set; }
        public RenderFragment Content { get; set; }
        public Delegate OnClick { get; set; }
    }

    public class DropdownOptions
    {
        public bool HiddenMode { get; set; }
        public RenderFragment Content { get; set; }

        public List<ActionInfo> Actions { get; set; }
    }
}
