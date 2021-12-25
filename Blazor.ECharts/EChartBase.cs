using Blazor.ECharts.Options;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.ECharts
{
    public class EChartBase<T> : ComponentBase where T : SeriesBase
    {
        protected string Id = $"echart_{Guid.NewGuid():N}";
        [Inject]
        public EChartsJsInterop JsInterop { get; set; }

        private EChartOption<T> option;
        [Parameter]
        public EChartOption<T> Option
        {
            get
            {
                return option;
            }
            set
            {
                option = value;
                if (option != null && afterRender)
                    _ = JsInterop.renderChart(Id, option);
            }
        }
        [Parameter]
        public string Style { get; set; }
        [Parameter]
        public string Class { get; set; }

        private bool afterRender;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (Option != null)
                    await JsInterop.renderChart(Id, Option.ToString());
            }
            afterRender = true;
        }
    }
}
