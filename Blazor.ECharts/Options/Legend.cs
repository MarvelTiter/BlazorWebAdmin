using Blazor.ECharts.Options.ChartComponent;

namespace Blazor.ECharts.Options
{
    /// <summary>
    /// 图例组件。
    /// 图例组件展现了不同系列的标记(symbol)，颜色和名字。可以通过点击图例控制哪些系列不显示。
    /// ECharts 3 中单个 echarts 实例中可以存在多个图例组件，会方便多个图例的布局。
    /// </summary>
    public class Legend : PositionComponent
    {
        /// <summary>
        /// 图例的类型。可选值：
        /// 'plain'：普通图例。缺省就是普通图例。
        /// 'scroll'：可滚动翻页的图例。当图例数量较多时可以使用。
        /// </summary>
        public string Type { get; set; }      
        /// <summary>
        /// 图例列表的布局朝向。
        /// 可选 horizontal | vertical
        /// </summary>
        public string Orient { get; set; }
        /// <summary>
        /// 图例标记和文本的对齐。默认自动，根据组件的位置和 orient 决定，当组件的 left 值为 'right' 以及纵向布局（orient 为 'vertical'）的时候为右对齐，即为 'right'。
        /// 可选 auto | left | right
        /// </summary>
        public string Align { get; set; }
        /// <summary>
        /// 图例内边距，单位px，默认各方向内边距为5，接受数组分别设定上右下左边距。
        /// </summary>
        public object Padding { get; set; } = 5;

        public TextStyle TextStyle { get; set; }
        /// <summary>
        /// 图例的数据数组
        /// </summary>
        public object Data { set; get; }
    }
}
