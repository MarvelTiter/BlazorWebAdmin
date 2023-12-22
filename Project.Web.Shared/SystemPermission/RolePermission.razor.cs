using AntDesign;
using AntDesign.TableModels;
using BlazorWeb.Shared.SystemPermission.Forms;
using BlazorWeb.Shared.Template.Tables;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;

namespace BlazorWeb.Shared.SystemPermission
{
    public partial class RolePermission
    {
        record PowerTreeNode(Power Node)
        {
            public IList<PowerTreeNode> Children { get; set; }
        }
        TableOptions<Role, GenericRequest<Role>> roleOptions = new();
        bool powerLoading = false;
        Role? CurrentRole;
        IEnumerable<PowerTreeNode> powerTreeData;
        IEnumerable<Power> allPower;
        string[]? selectedKeys;
        bool sideExpand;
        [Inject] public ModalService ModalSrv { get; set; }
        [Inject] public IPermissionService PermissionSrv { get; set; }
        [Inject] public MessageService MessageSrv { get; set; }
        [Inject] public ConfirmService ConfirmSrv { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            roleOptions.LoadDataOnLoaded = true;
            roleOptions.DataLoader = GetRolesAsync;
            roleOptions.AddHandle = AddRoleAsync;
            roleOptions.AddButton(ButtonDefinition<Role>.Edit(EditRole));
            roleOptions.AddButton(ButtonDefinition<Role>.Delete(DeleteRole));
            roleOptions.OnRowClick = HandleRowClick;
            _ = InitPowerTree();
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

        Task<IQueryCollectionResult<Role>> GetRolesAsync(GenericRequest<Role> req)
        {
            return PermissionSrv.GetRoleListAsync(req);
        }
        async Task<bool> AddRoleAsync()
        {
            var role = await ModalSrv.OpenDialog<RoleForm, Role>("新增角色");
            await PermissionSrv.InsertRoleAsync(role);
            return true;
        }
        async Task<bool> EditRole(Role role)
        {
            var newRole = await ModalSrv.OpenDialog<RoleForm, Role>("编辑角色", role);
            var result = await PermissionSrv.UpdateRoleAsync(newRole);
            return result.Success;
        }
        async Task<bool> DeleteRole(Role role)
        {
            var confirm = await ConfirmSrv.Show("该操作无法恢复！", "是否确认删除角色！");
            if (confirm == ConfirmResult.OK)
            {
                var result = await PermissionSrv.DeleteRoleAsync(role);
                return result.Success;
            }
            return false;
        }
        async Task SaveRolePower()
        {
            if (selectedKeys is null) return;
            var flag = await PermissionSrv.SaveRolePower(CurrentRole!.RoleId, selectedKeys.ToArray());
            if (flag.Success) _ = MessageSrv.Success("保存成功");
            else _ = MessageSrv.Error("保存数据异常！");
        }

        async Task HandleRowClick(RowData<Role> rowData)
        {
            powerLoading = true;
            await InitPowerTree();
            CurrentRole = rowData.Data;
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
