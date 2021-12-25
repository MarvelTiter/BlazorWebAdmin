namespace Blazor.ECharts.Options.ChartComponent
{
    public class AxisComponent : EChartComponent
    {
        public int? GridIndex { get; set; }
        public int? Offset { get; set; }
        /// <summary>
        /// 坐标轴类型。
        /// 可选 value | category | time | log
        /// </summary>
        public string Type { get; set; }
        public string Name { get; set; }
        public string NameLocation { get; set; }
        public int? NameGap { get; set; }
        public int? NameRotate { get; set; }
        public List<object> Data { get; set; }
    }

}
