using Microsoft.AspNetCore.Components;
using BlazorTemplate.Constraints.Common.Attributes;
using BlazorTemplate.Constraints.Page;
using BlazorTemplate.Constraints.Services;

namespace BlazorAdmin.Client.TestPages;
#if DEBUG
[Route("/testlocator")]
[PageInfo(Title = "Locator测试", Icon = "fa fa-question-circle-o", GroupId = "test")]
#endif
public class LocatorTest : SystemPageIndex<LocatorTest>
{
    protected override Type? GetPageType(IPageLocatorService pageLocator)
    {
        return pageLocator.GetPage("LocatorTest");
    }
}
