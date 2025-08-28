using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Page;
using Project.Constraints.Services;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;

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