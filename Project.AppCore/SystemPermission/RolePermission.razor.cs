using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Project.AppCore.SystemPermission.Forms;
using Project.Constraints.Services;
using Project.Constraints.UI;
using Project.Constraints.UI.Table;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;
using Project.Web.Shared.Basic;
using System.Linq.Expressions;

namespace Project.AppCore.SystemPermission
{
    public partial class RolePermission : ModelPage<Role, GenericRequest<Role>>
    {
        record PowerTreeNode(Power Node)
        {
            public IList<PowerTreeNode> Children { get; set; }
        }
        bool powerLoading = false;
        Role? CurrentRole;
        IEnumerable<PowerTreeNode> powerTreeData;
        IEnumerable<Power> allPower;
        string[]? selectedKeys;
        bool sideExpand;
        [Inject] public IPermissionService PermissionSrv { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.LoadDataOnLoaded = true;
            //roleOptions.DataLoader = GetRolesAsync;
            //roleOptions.AddHandle = AddRoleAsync;
            //roleOptions.OnRowClick = HandleRowClick;
            _ = InitPowerTree();
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
            List<PowerTreeNode> powerTreeNodes = new();
            var rootNodes = allPower.Where(p => p.PowerId == "ROOT");
            foreach (var item in rootNodes)
            {
                var n = new PowerTreeNode(item);
                n.Children = FindChildren(allPower, item);
                powerTreeNodes.Add(n);
            }
            powerTreeData = powerTreeNodes;
            return Task.CompletedTask;

            List<PowerTreeNode> FindChildren(IEnumerable<Power> all, Power parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<PowerTreeNode> childNodes = new();
                foreach (var child in children)
                {
                    var n1 = new PowerTreeNode(child);
                    n1.Children = FindChildren(all, child);
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
            var flag = await PermissionSrv.SaveRolePower(CurrentRole!.RoleId, selectedKeys.ToArray());
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
