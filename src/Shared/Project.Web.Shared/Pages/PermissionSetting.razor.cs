using AutoPageStateContainerGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Project.Constraints.Models.Permissions;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Pages.Component;

namespace Project.Web.Shared.Pages;

public partial class PermissionSetting<TPermission, TRole, TPermissionService> : ModelPage<TPermission, GenericRequest<TPermission>>
    where TPermission : class, IPermission, new()
    where TRole : class, IRole, new()
    where TPermissionService : IPermissionService<TPermission, TRole>
{
    [Inject, NotNull] public TPermissionService? PermissionSrv { get; set; }
    [Inject, NotNull] IStringLocalizer<TPermission>? Localizer { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        //await InitPowerTree();
        HideDefaultTableHeader = true;
        Options.Pager = false;
        Options.LoadDataOnLoaded = true;
        Options.TreeChildren = p => p.Children?.Cast<TPermission>() ?? [];
        Options.GetColumn(p => p.Icon).FormTemplate = ctx => b =>
            b.Component<PowerIconSelector>()
                .SetComponent(c => c.Context, ctx)
                .Build();
    }

    protected override object SetRowKey(TPermission model) => model.PermissionId;

    protected override async Task<QueryCollectionResult<TPermission>> OnQueryAsync(GenericRequest<TPermission> query)
    {
        var result = await PermissionSrv.GetPermissionListAsync(query);
        var powers = result.Payload.Cast<TPermission>();
        result.Payload = GeneratePowerTreeDataAsync(powers);
        return result;
    }

    #region 初始化权限树

    /// <summary>
    /// 构建权限树
    /// </summary>
    /// <returns></returns>
    static List<TPermission> GeneratePowerTreeDataAsync(IEnumerable<TPermission> all)
    {
        List<TPermission> powers = new();
        if (!all.Any())
        {
            return powers;
        }

        var topLevel = all.Min(p => p.PermissionLevel);
        var rootNodes = all.Where(p => p.PermissionLevel == topLevel);
        foreach (var item in rootNodes)
        {
            item.Children = FindChildren(all, item);
            powers.Add(item);
        }

        return powers;

        List<TPermission> FindChildren(IEnumerable<TPermission> all, TPermission parent)
        {
            var children = all.Where(p => p.ParentId == parent.PermissionId);
            List<TPermission> childNodes = new();
            foreach (var child in children)
            {
                child.Children = FindChildren(all, child);
                childNodes.Add(child);
            }

            return childNodes;
        }
    }

    #endregion


    public async Task AddRootPowerAsync()
    {
        var root = new TPermission()
        {
            PermissionId = "ROOT",
            PermissionName = "/",
            Sort = 1,
            PermissionLevel = 0,
            PermissionType = PermissionType.Page
        };
        try
        {
            _ = await PermissionSrv.InsertPermissionAsync(root);
            await Options.RefreshAsync();
        }
        catch (Exception ex)
        {
            UI.Error(ex.Message);
        }
    }

    string[] defaultPageButtons = new[] { "Add", "Modify", "Delete" };
    public bool CanShow(TableButtonContext<TPermission> context) => context.Data.PermissionType == PermissionType.Page;
    public string AddPowerLabel(TableButtonContext<TPermission> _) => Localizer["PermissionSetting.AddChild"];

    [TableButton(LabelExpression = nameof(AddPowerLabel), VisibleExpression = nameof(CanShow))]
    public async Task<IQueryResult> AddPower(TPermission parent)
    {
        var edit = new TPermission();
        edit.ParentId = parent.PermissionId;
        edit.PermissionLevel = parent.PermissionLevel + 1;
        edit.Sort = parent.Children?.Count() ?? 0;
        var power = await this.ShowEditFormAsync(Localizer["PermissionSetting.EditPermission"], edit, false);
        return await PermissionSrv.InsertPermissionAsync(power);
    }

    [EditButton]
    public async Task<IQueryResult> EditPower(TPermission node)
    {
        var p = await this.ShowEditFormAsync("编辑权限", node);
        return await PermissionSrv.UpdatePermissionAsync(p);
    }

    [DeleteButton]
    public async Task<IQueryResult> DeletePower(TPermission node)
    {
        return await PermissionSrv.DeletePermissionAsync(node);
    }
}
#if (ExcludeDefaultService)
#else
[StateContainer]
public partial class DefaultPermissionSetting : PermissionSetting<Permission, Role, IStandardPermissionService>
{
}
#endif