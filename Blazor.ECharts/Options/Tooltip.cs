using Blazor.ECharts.Options.ChartComponent;
using Blazor.ECharts.Options.Enums;

namespace Blazor.ECharts.Options
{
    /// <summary>
    /// 提示框组件。
    /// </summary>
    public class Tooltip : EChartComponent
    {
        /// <summary>
        /// 触发类型。
        /// </summary>
        public TooltipTrigger? Trigger { set; get; }

        /// <summary>
        /// 提示框浮层的位置，默认不设置时位置会跟随鼠标的位置。
        /// </summary>
        public object Position { set; get; }

        /// <summary>
        /// 提示框浮层内容格式器，支持字符串模板和回调函数两种形式。
        /// </summary>
        public object Formatter { set; get; }

        /// <summary>
        /// 提示框浮层的背景颜色。
        /// </summary>
        public object BackgroundColor { set; get; }

        /// <summary>
        /// 提示框浮层的边框宽。
        /// </summary>
        public int? BorderWidth { set; get; }

        /// <summary>
        /// 提示框浮层的边框颜色。
        /// </summary>
        public object BorderColor { set; get; }

        /// <summary>
        /// 提示框浮层内边距，单位px，默认各方向内边距为5，接受数组分别设定上右下左边距。
        /// </summary>
        public object Padding { set; get; }

        public TextStyle TextStyle { set; get; }
    }
}
