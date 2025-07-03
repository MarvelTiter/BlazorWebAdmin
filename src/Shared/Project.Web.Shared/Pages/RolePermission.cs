using AutoPageStateContainerGenerator;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Project.Constraints.Options;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Tree;
using Project.Web.Shared.Pages.Component;

namespace Project.Web.Shared.Pages;

public class RolePermission<TPermission, TRole, TPermissionService> : ModelPage<TRole, GenericRequest<TRole>>
    where TPermission : class, IPermission, new()
    where TRole : class, IRole, new()
    where TPermissionService : IPermissionService<TPermission, TRole>
{
    IEnumerable<TPermission> allPower = [];
    [Inject, NotNull] public TPermissionService? PermissionSrv { get; set; }
    [Inject, NotNull] public IStringLocalizer<TPermission>? Localizer { get; set; }
    [Inject, NotNull] public IOptionsMonitor<CultureOptions>? CultureSetting { get; set; }

    TreeOptions<TPermission>? options;
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        Options.LoadDataOnLoaded = true;
        Options.GetColumn(p => p.Permissions).FormTemplate = ctx => b =>
            b.Component<AssignRolePowers<TPermission>>()
                .SetComponent(c => c.Context, ctx)
                .SetComponent(c => c.Options, options)
                .Build();
        await InitPowerTree();
    }
    protected override object SetRowKey(TRole model) => model.RoleId;

    protected override async Task<QueryCollectionResult<TRole>> OnQueryAsync(GenericRequest<TRole> query)
    {
        var result = await PermissionSrv.GetRoleListAsync(query);
        return result;

    }

    #region 初始化权限树
    async Task InitPowerTree()
    {
        await GetAllPowersAsync();
        await GeneratePowerTreeDataAsync();
    }
    /// <summary>
    /// 获取所有权限
    /// </summary>
    /// <returns></returns>
    async Task GetAllPowersAsync()
    {
        var result = await PermissionSrv.GetAllPermissionAsync();
        allPower = result.Payload;
    }
    /// <summary>
    /// 构建权限树
    /// </summary>
    /// <returns></returns>
    Task GeneratePowerTreeDataAsync()
    {
        List<TreeData<TPermission>> powerTreeNodes = new();
        var rootNodes = allPower.Where(p => p.PermissionId == "ROOT");
        foreach (var item in rootNodes)
        {
            var n = new TreeData<TPermission>(item)
            {
                Children = FindChildren(allPower, item)
            };
            powerTreeNodes.Add(n);
        }
        options = new TreeOptions<TPermission>(powerTreeNodes);
        options.KeyExpression = p => p.PermissionId;
        //if (CultureSetting.CurrentValue.Enabled)
        //{
        //    options.TitleExpression = p => Localizer[p.PowerId].Value;
        //}
        //else
        //{
        //}
        options.TitleExpression = p => p.PermissionName;
        return Task.CompletedTask;

        List<TreeData<TPermission>> FindChildren(IEnumerable<TPermission> all, TPermission parent)
        {
            var children = all.Where(p => p.ParentId == parent.PermissionId);
            List<TreeData<TPermission>> childNodes = new();
            foreach (var child in children)
            {
                var n1 = new TreeData<TPermission>(child)
                {
                    Children = FindChildren(all, child)
                };
                childNodes.Add(n1);
            }
            return childNodes;
        }
    }
    #endregion

    protected override async Task<IQueryResult?> OnAddItemAsync()
    {
        var role = await this.ShowAddFormAsync("新增角色");
        var result = await PermissionSrv.InsertRoleAsync(role);
        return result;
    }

    [EditButton]
    public async Task<IQueryResult> EditRole(TRole role)
    {
        var powers = await PermissionSrv.GetPermissionListByRoleIdAsync(role.RoleId);
        role.Permissions = powers.Payload.Select(p => p.PermissionId).ToList();
        var newRole = await this.ShowEditFormAsync("编辑角色", role);
        var result1 = await PermissionSrv.SaveRoleWithPermissionsAsync(newRole);
        //var result2 = await PermissionSrv.SaveRolePowerAsync((newRole.RoleId, newRole.Powers));
        return result1;
    }

    [DeleteButton]
    public async Task<IQueryResult> DeleteRole(TRole role)
    {
        return await PermissionSrv.DeleteRoleAsync(role);
    }
}
#if (ExcludeDefaultService)
#else
[StateContainer]
public partial class DefaultRolePermission : RolePermission<Permission, Role, IStandardPermissionService>
{
}
#endif