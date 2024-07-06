﻿using Project.AppCore.Layouts.LayoutComponents;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Project.AppCore.Store;
using Project.Constraints.Store.Models;
using Project.Constraints.Store;
using Project.Constraints.Page;
using System.Diagnostics.CodeAnalysis;

namespace Project.AppCore.Layouts.Layouts;

public class ContainerBase : BasicComponent
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
}
public class LayoutBase : BasicComponent
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Inject, NotNull] public IProtectedLocalStorage? Storage { get; set; }
    public Banner? Banner { get; set; }
    public SideBar? SideBar { get; set; }
    public WebSetting? WebSetting { get; set; }
    public async Task HandleToggleCollapse(bool newState)
    {
        App.Collapsed = newState;
        SideBar?.ToggleCollapse();
        await Storage.SetAsync(AppStore.KEY, App);
    }

    //public void UpdateCollapse(bool state)
    //{
    //    _ = Banner?.UpdateToggleMenu(state);
    //}

    public int MainWidthOffset
    {
        get
        {
            return App.Mode switch
            {
                LayoutMode.Card => App.Collapsed ? 80 : App.SideBarExpandWidth + 16,
                LayoutMode.Classic => App.Collapsed ? 80 : App.SideBarExpandWidth,
                _ => 0
            };
        }
    }
}
