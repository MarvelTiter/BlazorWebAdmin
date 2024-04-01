using Microsoft.AspNetCore.Components;

namespace Project.Constraints.UI.Props
{
    public class ModalProp : DefaultProp
    {
        public string Title { get; set; }
        public bool Visible { get; set; }
        public string? Width { get; set; }
        public EventCallback<bool> VisibleChanged { get; set; }
        public RenderFragment ChildContent { get; set; }
        public bool HideDefaultFooter { get; set; }
    }
}
