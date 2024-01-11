using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using Project.Constraints.Options;
using Project.Constraints.UI.Extensions;
using Project.Constraints.UI.Tree;
using Project.Models.Entities.Permissions;

namespace Project.AppCore.SystemPermission
{
    public partial class RolePermission : ModelPage<Role, GenericRequest<Role>>
    {

        bool powerLoading = false;
        Role? CurrentRole;
        IEnumerable<Power> allPower;
        string[]? selectedKeys;
        bool sideExpand;
        [Inject] public IPermissionService PermissionSrv { get; set; }
        [Inject] public IStringLocalizer<Power> Localizer { get; set; }
        [Inject] public IOptionsMonitor<CultureOptions> CultureSetting { get; set; }
        TreeOptions<Power> options;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            Options.LoadDataOnLoaded = true;
            await InitPowerTree();
        }
        protected override object SetRowKey(Role model) => model.RoleId;

        protected override Task<IQueryCollectionResult<Role>> OnQueryAsync(GenericRequest<Role> query)
        {
            return PermissionSrv.GetRoleListAsync(query);

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
            var result = await PermissionSrv.GetPowerListAsync();
            allPower = result.Payload;
        }
        /// <summary>
        /// 构建权限树
        /// </summary>
        /// <returns></returns>
        Task GeneratePowerTreeDataAsync()
        {
            List<TreeData<Power>> powerTreeNodes = new();
            var rootNodes = allPower.Where(p => p.PowerId == "ROOT");
            foreach (var item in rootNodes)
            {
                var n = new TreeData<Power>(item)
                {
                    Children = FindChildren(allPower, item)
                };
                powerTreeNodes.Add(n);
            }
            options = new TreeOptions<Power>(powerTreeNodes);
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

            List<TreeData<Power>> FindChildren(IEnumerable<Power> all, Power parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<TreeData<Power>> childNodes = new();
                foreach (var child in children)
                {
                    var n1 = new TreeData<Power>(child)
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
        public async Task<bool> EditRole(Role role)
        {
            //var newRole = await UI.ShowDialogAsync<RoleForm, Role>("编辑角色", role);
            var newRole = await this.ShowEditFormAsync("编辑角色", role);
            var result = await PermissionSrv.UpdateRoleAsync(newRole);
            return result.Success;
        }

        [DeleteButton]
        public async Task<bool> DeleteRole(Role role)
        {
            var result = await PermissionSrv.DeleteRoleAsync(role);
            return result.Success;
        }

        async Task SaveRolePower()
        {
            if (selectedKeys is null) return;
            var flag = await PermissionSrv.SaveRolePower(CurrentRole!.RoleId,  selectedKeys);
            if (flag.Success) UI.Success("保存成功");
            else UI.Error("保存数据异常！");
        }


        protected override async Task OnRowClickAsync(Role model)
        {
            powerLoading = true;
            await InitPowerTree();
            CurrentRole = model;
            sideExpand = true;
            StateHasChanged();
            var result = await PermissionSrv.GetPowerListByRoleIdAsync(CurrentRole.RoleId);
            var keys = result.Payload.Select(p => p.PowerId);
            selectedKeys = keys.ToArray();
            powerLoading = false;
            StateHasChanged();
        }
    }
}
