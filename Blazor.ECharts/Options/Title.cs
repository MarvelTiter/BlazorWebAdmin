using Blazor.ECharts.Options.ChartComponent;

namespace Blazor.ECharts.Options
{
    /// <summary>
    /// 标题组件，包含主标题和副标题。
    /// </summary>
    public class Title : EChartComponent
    {
        /// <summary>
        /// 主标题文本，支持使用 \n 换行。
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 主标题文本超链接。
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// 指定窗口打开主标题超链接。
        /// 'self' 当前窗口打开
        /// 'blank' 新窗口打开
        /// </summary>
        public string Target { get; set; }
        /// <summary>
        /// 副标题文本，支持使用 \n 换行。
        /// </summary>
        public string Subtext { get; set; }
        /// <summary>
        /// 副标题文本超链接。
        /// </summary>
        public string Sublink { get; set; }
        /// <summary>
        /// 指定窗口打开副标题超链接。
        /// 'self' 当前窗口打开
        /// 'blank' 新窗口打开
        /// </summary>
        public string Subtarget { get; set; }
        public TextStyle TextStyle { get; set; }
        public TextStyle SubtextStyle { get; set; }

    }
}