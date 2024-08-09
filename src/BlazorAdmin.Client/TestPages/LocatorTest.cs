using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Page;
using Project.Constraints.Services;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.Client.TestPages
{
#if DEBUG
    [Route("/test5")]
    [PageInfo(Id = "TestPage5", Title = "Locator测试5", GroupId = "test")]
#endif
    public class LocatorTest : PageIndex
    {
        [Inject, NotNull] IUIService? UI { get; set; }

        protected override Type? GetPageType(IPageLocatorService pageLocator)
        {
            return pageLocator.GetPage("LocatorTest");
        }
    }
}
