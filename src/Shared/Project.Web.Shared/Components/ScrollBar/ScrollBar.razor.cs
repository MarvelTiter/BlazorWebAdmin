using Microsoft.AspNetCore.Components;
namespace Project.Web.Shared.Components
{
    public partial class ScrollBar : JsComponentBase
    {
        public struct ScrollArgs
        {
            public int Top { get; set; }
            public int Left { get; set; }
        }
        public ElementReference? WrapElementRef { get; set; }
        public ElementReference? ScrollbarRef { get; set; }
        public ElementReference? ResizeRef { get; set; }
        public ElementReference? HorizontalTracker { get; set; }
        public ElementReference? HorizontalThumb { get; set; }
        public ElementReference? VerticalTracker { get; set; }
        public ElementReference? VerticalThumb { get; set; }
        //public IJSObjectReference ScrollBarModule { get; set; }
        [Parameter] public bool Always { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }
        [Parameter] public bool Native { get; set; }
        [Parameter] public string WrapClass { get; set; }
        [Parameter] public string ViewClass { get; set; }
        [Parameter] public string ViewStyle { get; set; }
        [Parameter] public string WrapStyle { get; set; }
        [Parameter] public int? Height { get; set; }
        [Parameter] public int? MaxHeight { get; set; }
        //[Parameter] public EventCallback<ScrollArgs> OnScroll { get; set; }
        [Parameter] public int MinSize { get; set; }
        [Parameter] public int BarWidth { get; set; } = 6;
        [Parameter] public string BarColor { get; set; } = "#909399";

        protected override async ValueTask Init()
        {
            if (Native) return;
            //ScrollBarModule ??= await Js.InvokeAsync<IJSObjectReference>("import", "./js/scrollbar/scrollbar.js");
            await ModuleInvokeVoidAsync("initBarInstance", "vertical", VerticalTracker, VerticalThumb);
            await ModuleInvokeVoidAsync("initBarInstance", "horizontal", HorizontalTracker, HorizontalThumb);
            await ModuleInvokeVoidAsync("initScrollbar", ScrollbarRef, WrapElementRef, ResizeRef, MinSize, Always);
        }
    }
}
