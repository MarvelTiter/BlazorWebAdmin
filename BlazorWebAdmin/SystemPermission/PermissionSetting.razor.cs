using AntDesign;
using BlazorWeb.Shared.Template.Tables;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using BlazorWebAdmin.SystemPermission.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;

namespace BlazorWebAdmin.SystemPermission
{
    record PowerTreeNode(Power Node)
    {
        public IList<PowerTreeNode> Children { get; set; }
    }
    public partial class PermissionSetting
    {
        [Inject]
        public ModalService ModalSrv { get; set; }
        [Inject]
        public DrawerService DrawerSrv { get; set; }
        [Inject]
        public IPermissionService PermissionSrv { get; set; }
        [Inject] IStringLocalizer<Power> Localizer { get; set; }

        //TableOptions<Power, GenericRequest<Power>> tableOptions = new();
        IEnumerable<PowerTreeNode> powerTree;
        IEnumerable<Power> allPower;
        protected override void OnInitialized()
        {
            base.OnInitialized();
            //tableOptions.LoadDataOnLoaded = true;
            //tableOptions.DataLoader = Search;
            //tableOptions.AddHandle = AddPower;
            //tableOptions.AddButton(ButtonDefinition<Power>.Edit(EditPower));
            //tableOptions.AddButton(ButtonDefinition<Power>.Delete(DeletePower));
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
            powerTree = powerTreeNodes;
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

        Task<IQueryCollectionResult<Power>> Search(GenericRequest<Power> req)
        {
            return PermissionSrv.GetPowerListAsync(req);
        }
        string[] defaultPageButtons = new[] { "Add", "Modify", "Delete" };
        async Task AddPower(PowerTreeNode parent)
        {
            var power = await ModalSrv.OpenDialog<PowerForm, Power>("新增权限");
            power.ParentId = parent.Node.PowerId;
            power.Level = parent.Node.Level + 1;
            power.Sort = parent.Children.Count + 1;
            var result = await PermissionSrv.InsertPowerAsync(power);
            if (result.Success)
            {
                if (power.PowerType == PowerType.Page)
                {
                    foreach (var item in defaultPageButtons)
                    {
                        var p = new Power
                        {
                            PowerId = $"{power.PowerId}:{item}",
                            ParentId = power.PowerId,
                            PowerName = $"{power.PowerName}:{item}",
                            PowerType = PowerType.Button,
                            Level = power.Level + 1,
                        };
                        await PermissionSrv.InsertPowerAsync(p);
                    }
                }
                await InitPowerTree();
            }
        }
        async Task<bool> EditPower(PowerTreeNode node)
        {
            var p = await ModalSrv.OpenDialog<PowerForm, Power>("编辑权限", node.Node);
            var result = await PermissionSrv.UpdatePowerAsync(p);
            return result.Success;
        }

        Task<bool> DeletePower(PowerTreeNode node)
        {
            //TODO delete power
            return Task.FromResult(false);
        }

    }
}
