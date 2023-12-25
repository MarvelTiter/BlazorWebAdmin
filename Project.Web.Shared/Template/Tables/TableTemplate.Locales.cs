using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace Project.Web.Shared.Template.Tables
{
    public class TableTemplate { }
    public partial class TableTemplate<TData, TQuery>
    {
        [Inject] IStringLocalizer<TData> Localizer { get; set; }
        [Inject] IStringLocalizer<TableTemplate> TableLocalizer { get; set; }
        public string RenderColumnTitle(string key)
        {
            return Localizer[key];
        }
    }
}
