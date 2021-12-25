using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blazor.ECharts.Options
{
    public class EChartOption<T> where T : SeriesBase
    {
        public Title Title { get; set; }
        public Legend Legend { get; set; }
        public Grid Grid { get; set; }
        public XAxis XAxis { get; set; }
        public YAxis YAxis { get; set; }
        public Tooltip Tooltip { get; set; }
        public List<T> Series { get; set; }
        public override string ToString()
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
            return JsonSerializer.Serialize(this, jsonSerializerOptions);
        }
    }
}
