﻿using Microsoft.AspNetCore.Components.Rendering;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Components;
using Project.Web.Shared.Pages.Component;

namespace Project.Web.Shared.Pages;

[Route("/userdashboard")]
[PageInfo(Icon = "svg-setting", Title = "在线用户", Sort = 999)]
public class OnlineUsers : ModelPage<ClientInfo, GenericRequest<ClientInfo>>
{
    [Inject, NotNull] IClientService? ClientService { get; set; }
    bool? hasPermission;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Options.LoadDataOnLoaded = true;
    }

    protected override async Task OnInitializedAsync()
    {
        var result = await ClientService.CheckPermissionAsync(User.UserInfo);
        hasPermission = result.IsSuccess;
        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await Options.NotifyChanged();
        }
    }

    protected override Task<QueryCollectionResult<ClientInfo>> OnQueryAsync(GenericRequest<ClientInfo> query)
    {
        return ClientService.GetClientsAsync(query);
    }

    [TableButton(Label = "用户信息")]
    public Task<IQueryResult?> ShowUserInfo(ClientInfo info)
    {
        _ = UI.ShowDialogAsync<JsonDisplay, ClientInfo>("用户信息", info, width: "50%");
        return QueryResult.Null().AsTask();
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (!hasPermission.HasValue)
        {
            return;
        }
        if (hasPermission.Value)
        {
            base.BuildRenderTree(builder);
        }
        else
        {
            builder.Component<ForbiddenPage>().Build();
        }
    }
}