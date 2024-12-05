﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common.Attributes;
using Project.Constraints.PageHelper;
using System.Diagnostics.CodeAnalysis;

namespace Project.Web.Shared.Pages
{

    public abstract class SystemPageIndex : ComponentBase, IPageAction
    {
        [Inject, NotNull] IProjectSettingService? SettingProvider { get; set; }
        [Inject, NotNull] IPageLocatorService? Locator { get; set; }
        protected Type? PageType { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            PageType = GetPageType(Locator);
        }
        public abstract Type? GetPageType(IPageLocatorService customSetting);
        IPageAction? page;
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (PageType != null)
            {
                builder.OpenComponent(0, PageType);
                builder.AddComponentReferenceCapture(1, obj =>
                {
                    page = obj as IPageAction;
                });
                builder.CloseComponent();
            }
        }

        public async Task OnShowAsync()
        {
            if (page != null)
                await page.OnShowAsync();
        }

        public async Task OnHiddenAsync()
        {
            if (page != null)
                await page.OnHiddenAsync();
        }
    }

    [Route("/user/index")]
    public class UserIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetUserPageType();
        }
    }

    [Route("/operationlog")]
    public class RunLogIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetRunLogPageType();
        }
    }

    [Route("/permission")]
    public class PermissionIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetPermissionPageType();
        }
    }

    [Route("/rolepermission")]
    public class RolePermissionIndex : SystemPageIndex
    {
        public override Type? GetPageType(IPageLocatorService customSetting)
        {
            return customSetting.GetRolePermissionPageType();
        }
    }
}
