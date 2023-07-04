using AntDesign;
using BlazorWeb.Shared.Template.Tables;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using BlazorWebAdmin.SystemPermission.Forms;
using DocumentFormat.OpenXml.InkML;
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
        [Inject] public ModalService ModalSrv { get; set; }
        [Inject] public DrawerService DrawerSrv { get; set; }
        [Inject] ConfirmService ConfirmSrv { get; set; }
        [Inject] IStringLocalizer<TableTemplate> TableLocalizer { get; set; }
        [Inject] public IPermissionService PermissionSrv { get; set; }
        [Inject] IStringLocalizer<Power> Localizer { get; set; }

        //TableOptions<Power, GenericRequest<Power>> tableOptions = new();
        IEnumerable<PowerTreeNode> powerTree;
        IEnumerable<Power> allPower;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await InitPowerTree();
        }

        #region 初始化权限树
        bool loading = false;
        async Task InitPowerTree()
        {
            loading = true;
            await GetAllPowersAsync();
            await GeneratePowerTreeDataAsync();
            loading = false;
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
            power.PowerLevel = parent.Node.PowerLevel + 1;
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
                            PowerLevel = power.PowerLevel + 1,
                        };
                        await PermissionSrv.InsertPowerAsync(p);
                    }
                }
                await InitPowerTree();
            }
        }
        async Task EditPower(PowerTreeNode node)
        {
            var p = await ModalSrv.OpenDialog<PowerForm, Power>("编辑权限", node.Node);
            await PermissionSrv.UpdatePowerAsync(p);
            await InitPowerTree();
        }

        async Task DeletePower(PowerTreeNode node)
        {
            var confirmResult = await ConfirmSrv.Show(
                   TableLocalizer["TableTips.DangerActionConfirmContent"].Value
               , TableLocalizer["TableTips.DangerActionConfirmTitle"].Value);
            if (confirmResult == ConfirmResult.OK)
            {
                await PermissionSrv.DeletePowerAsync(node.Node);
                await InitPowerTree();
            }
        }

    }
}
