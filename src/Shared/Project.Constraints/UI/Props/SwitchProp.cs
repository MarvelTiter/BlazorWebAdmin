using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Props
{
    public class SwitchProp : DefaultProp
    {
        public string? CheckedLabel { get; set; }
        public string? UnCheckedLabel { get; set; }
    }

    public class TabsProp
    {
        public class TabItem
        {
            public string? Title { get; set; }
            public RenderFragment? TitleTemplate { get; set; }
            public RenderFragment? Content { get; set; }
        }
        public string? ActiveKey { get; set; }
        public string? Type { get; set; }
        public EventCallback OnChange { get; set; }
        public List<TabItem> TabContents { get; set; } = [];
    }
}
