using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Project.Constraints.Options;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Tree;
using Project.Web.Shared.Pages.Component;

namespace Project.Web.Shared.Pages
{
    public class RolePermission<TPower, TRole, TPermissionService> : ModelPage<TRole, GenericRequest<TRole>>
        where TPower : class, IPower, new()
        where TRole : class, IRole, new()
        where TPermissionService : IPermissionService<TPower, TRole>
    {
        IEnumerable<TPower> allPower = [];
        [Inject, NotNull] public TPermissionService? PermissionSrv { get; set; }
        [Inject, NotNull] public IStringLocalizer<TPower>? Localizer { get; set; }
        [Inject, NotNull] public IOptionsMonitor<CultureOptions>? CultureSetting { get; set; }

        TreeOptions<TPower>? options;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Options.LoadDataOnLoaded = true;
            Options.GetColumn(p => p.Powers).FormTemplate = ctx => b =>
                b.Component<AssignRolePowers<TPower>>()
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
            var result = await PermissionSrv.GetAllPowerAsync();
            allPower = result.Payload;
        }
        /// <summary>
        /// 构建权限树
        /// </summary>
        /// <returns></returns>
        Task GeneratePowerTreeDataAsync()
        {
            List<TreeData<TPower>> powerTreeNodes = new();
            var rootNodes = allPower.Where(p => p.PowerId == "ROOT");
            foreach (var item in rootNodes)
            {
                var n = new TreeData<TPower>(item)
                {
                    Children = FindChildren(allPower, item)
                };
                powerTreeNodes.Add(n);
            }
            options = new TreeOptions<TPower>(powerTreeNodes);
            options.KeyExpression = p => p.PowerId;
            //if (CultureSetting.CurrentValue.Enabled)
            //{
            //    options.TitleExpression = p => Localizer[p.PowerId].Value;
            //}
            //else
            //{
            //}
            options.TitleExpression = p => p.PowerName;
            return Task.CompletedTask;

            List<TreeData<TPower>> FindChildren(IEnumerable<TPower> all, TPower parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<TreeData<TPower>> childNodes = new();
                foreach (var child in children)
                {
                    var n1 = new TreeData<TPower>(child)
                    {
                        Children = FindChildren(all, child)
                    };
                    childNodes.Add(n1);
                }
                return childNodes;
            }
        }
        #endregion

        protected override async Task<bool> OnAddItemAsync()
        {
            var role = await this.ShowAddFormAsync("新增角色");
            await PermissionSrv.InsertRoleAsync(role);
            return true;
        }

        [EditButton]
        public async Task<bool> EditRole(TRole role)
        {
            var powers = await PermissionSrv.GetPowerListByRoleIdAsync(role.RoleId);
            role.Powers = powers.Payload.Select(p => p.PowerId).ToList();
            var newRole = await this.ShowEditFormAsync("编辑角色", role);
            var result = await PermissionSrv.UpdateRoleAsync(newRole);
            await PermissionSrv.SaveRolePowerAsync((newRole.RoleId, newRole.Powers));
            return result.Success;
        }

        [DeleteButton]
        public async Task<bool> DeleteRole(TRole role)
        {
            var result = await PermissionSrv.DeleteRoleAsync(role);
            return result.Success;
        }
    }
}
