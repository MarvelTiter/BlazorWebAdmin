using Blazor.ECharts.Options.ChartComponent;

namespace Blazor.ECharts.Options
{
    /// <summary>
    /// 直角坐标系内绘图网格，单个 grid 内最多可以放置上下两个 X 轴，左右两个 Y 轴。可以在网格上绘制折线图，柱状图，散点图（气泡图）。
    /// </summary>
    public class Grid : PositionComponent
    {
        public bool ContainLabel { get; set; } = false;
        public string BackgroundColor { get; set; } = "transparnet";

    }
}
