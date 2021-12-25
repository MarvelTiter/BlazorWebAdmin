using Blazor.ECharts.Options.ChartComponent;

namespace Blazor.ECharts.Options
{
    public class SeriesBase : EChartComponent
    {
        public SeriesBase(string type, string id = null, string name = null)
        {
            Type = type;
            Id = id;
            Name = name;
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public string Type { set; get; } = "line";

        /// <summary>
        /// 系列名称，用于tooltip的显示，legend 的图例筛选，在 setOption 更新数据和配置项时用于指定对应的系列。
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// 初始动画的延迟，支持回调函数，可以通过每个数据返回不同的 delay 时间实现更戏剧的初始动画效果。
        /// </summary>
        public object AnimationDelay { set; get; }

        /// <summary>
        /// 系列中的数据内容数组。数组项通常为具体的数据项。
        /// </summary>
        public object Data { set; get; }
    }
}
