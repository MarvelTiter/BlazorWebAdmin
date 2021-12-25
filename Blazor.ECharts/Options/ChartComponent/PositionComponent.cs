using Blazor.ECharts.Options.Constraints;

namespace Blazor.ECharts.Options.ChartComponent
{
    public class PositionComponent : EChartComponent, IPosition
    {
        public int Zlevel { get; set; } = 0;
        public int Z { get; set; } = 2;
        /// <summary>
        /// 组件离容器左侧的距离。
        /// left 的值可以是像 20 这样的具体像素值，可以是像 '20%' 这样相对于容器高宽的百分比，也可以是 'left', 'center', 'right'。
        /// 如果 left 的值为'left', 'center', 'right'，组件会根据相应的位置自动对齐。
        /// </summary>
        public object Left { get; set; }
        /// <summary>
        /// 组件离容器上侧的距离。
        /// top 的值可以是像 20 这样的具体像素值，可以是像 '20%' 这样相对于容器高宽的百分比，也可以是 'top', 'middle', 'bottom'。
        /// 如果 top 的值为'top', 'middle', 'bottom'，组件会根据相应的位置自动对齐。
        /// </summary>
        public object Top { get; set; }
        /// <summary>
        /// 组件离容器右侧的距离。
        /// right  的值可以是像 20 这样的具体像素值，可以是像 '20%' 这样相对于容器高宽的百分比，也可以是 'left', 'center', 'right'。
        /// 如果 right 的值为'left', 'center', 'right'，组件会根据相应的位置自动对齐。
        /// </summary>
        public object Right { get; set; }
        /// <summary>
        /// 组件离容器下侧的距离。
        /// bottom  的值可以是像 20 这样的具体像素值，可以是像 '20%' 这样相对于容器高宽的百分比，也可以是 'top', 'middle', 'bottom'。
        /// 如果 bottom 的值为'top', 'middle', 'bottom'，组件会根据相应的位置自动对齐。
        /// </summary>
        public object Bottom { get; set; }
        /// <summary>
        /// 组件的宽度。默认自适应。
        /// Width  的值可以是像 20 这样的具体像素值，可以是像 '20%' 这样相对于容器高宽的百分比
        /// </summary>
        public object Width { get; set; }
        /// <summary>
        /// 组件的高度。默认自适应。
        /// Height  的值可以是像 20 这样的具体像素值，可以是像 '20%' 这样相对于容器高宽的百分比
        /// </summary>
        public object Height { get; set; }
    }
}
