﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI;
using System.Diagnostics.CodeAnalysis;

namespace BlazorAdmin.TestPages
{
#if DEBUG
    [Route("/test2")]
    [PageInfo(Id = "TestPage2", Title = "测试2", GroupId = "test")]
#endif
    public partial class TestPage2
    {
        [Inject, NotNull] IUIService? UI { get; set; }
    }
}
