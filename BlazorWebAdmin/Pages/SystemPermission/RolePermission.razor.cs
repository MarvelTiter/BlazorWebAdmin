using AntDesign;
using AntDesign.TableModels;
using BlazorWebAdmin.Template.Forms.EntityForms;
using BlazorWebAdmin.Template.Tables;
using BlazorWebAdmin.Template.Tables.Setting;
using BlazorWebAdmin.Utils;
using Microsoft.AspNetCore.Components;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;
using Project.Services.interfaces;

namespace BlazorWebAdmin.Pages.SystemPermission
{
    public partial class RolePermission
    {
        record PowerTreeNode(Power Node)
        {
            public IList<PowerTreeNode> Children { get; set; }
        }
        TableOptions<Role, GeneralReq<Role>> roleOptions = new();
        bool powerLoading = false;
        Role? CurrentRole;
        Tree<PowerTreeNode> treeInstance;
        IEnumerable<PowerTreeNode> powerTreeData;
        IEnumerable<Power> allPower;
        string[]? selectedKeys = Array.Empty<string>();

        [Inject]
        public ModalService ModalSrv { get; set; }
        [Inject]
        public IPemissionService PermissionSrv { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            roleOptions.DataLoader = GetRolesAsync;
            roleOptions.AddHandle = AddRoleAsync;
            roleOptions.AddButton(ButtonDefinition<Role>.Edit(EditRole));
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
            allPower = await PermissionSrv.GetPowerListAsync();
        }
        /// <summary>
        /// 构建权限树
        /// </summary>
        /// <returns></returns>
        Task GeneratePowerTreeDataAsync()
        {
            List<PowerTreeNode> powerTreeNodes = new();
            var rootNodes = allPower.Where(p => p.ParentId == "ROOT");
            foreach (var item in rootNodes)
            {
                var n = new PowerTreeNode(item);
                n.Children = FindChildren(allPower, item);
            }
            powerTreeData = powerTreeNodes;
            return Task.CompletedTask;

            List<PowerTreeNode> FindChildren(IEnumerable<Power> all, Power parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<PowerTreeNode> powerTreeNodes = new();
                foreach (var child in children)
                {
                    var n = new PowerTreeNode(child);
                    n.Children = FindChildren(all, child);
                }
                return powerTreeNodes;
            }
        }
        #endregion

        Task<QueryResult<PagingResult<Role>>> GetRolesAsync(GeneralReq<Role> req)
        {
            return PermissionSrv.GetRoleListAsync(req);
        }
        async Task<bool> AddRoleAsync()
        {
            var role = await ModalSrv.OpenDialog<RoleForm, Role>("新增角色");
            return true;
        }
        async Task EditRole(Role role)
        {
            var newRole = await ModalSrv.OpenDialog<RoleForm, Role>("编辑角色", role);
            //TODO save change
            Console.WriteLine(newRole.RoleName);
        }
        async Task SaveRolePower()
        {
            if (selectedKeys is null) return;
            var totalKeys = new List<string>(selectedKeys);
            foreach (var item in selectedKeys)
            {
                var parent = allPower.FirstOrDefault(p => p.PowerId == item)?.ParentId;
                if (parent != null && !totalKeys.Contains(parent))
                {
                    totalKeys.Add(parent);
                }
            }
            await PermissionSrv.SaveRolePower(CurrentRole!.RoleId, totalKeys.ToArray());
        }

        async Task HandleRowClick(RowData<Role> rowData)
        {
            powerLoading = true;
            CurrentRole = rowData.Data;
            StateHasChanged();

            var result = await PermissionSrv.GetPowerListByRoleIdAsync(CurrentRole.RoleId);
            var keys = result.Payload.Select(p => p.PowerId);
            selectedKeys = keys.ToArray();
            powerLoading = false;
            StateHasChanged();
        }
        void CloseSide()
        {
            powerLoading = false;
            CurrentRole = null;
        }
    }
}
