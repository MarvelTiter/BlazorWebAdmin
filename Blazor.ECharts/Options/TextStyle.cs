namespace Blazor.ECharts.Options
{
    public class TextStyle
    {       
        /// <summary>
        /// 主标题文字的颜色。
        /// </summary>
        public string Color { get; set; }
        /// <summary>
        /// 主标题文字字体的风格。
        /// 可选：
        /// 'normal'
        /// 'italic'
        /// 'oblique'
        /// </summary>
        public string FontStyle { get; set; }
        /// <summary>
        /// 主标题文字字体的粗细。
        /// 'normal'
        /// 'bold'
        /// 'bolder'
        /// 'lighter'
        /// 100 | 200 | 300 | 400...
        /// </summary>
        public object FontWeight { get; set; }
        /// <summary>
        /// 主标题文字的字体系列。还可以是 'serif' , 'monospace', 'Arial', 'Courier New', 'Microsoft YaHei'
        /// </summary>
        public string FontFamily { get; set; }
        /// <summary>
        /// 主标题文字的字体大小。
        /// </summary>
        public int? FontSize { get; set; }
        /// <summary>
        /// 行高
        /// </summary>
        public int? LineHeight { get; set; }
        /// <summary>
        /// 文本显示宽度
        /// </summary>
        public int? Width { get; set; }
        /// <summary>
        /// 文本显示高度
        /// </summary>
        public int? Height { get; set; }

    }
}