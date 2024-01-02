using AntDesign;
using BlazorWeb.Shared.SystemPermission.Forms;
using BlazorWeb.Shared.Template.Tables;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Project.AppCore.PageHelper;
using Project.AppCore.Services;
using Project.Constraints.Services;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;

namespace BlazorWeb.Shared.SystemPermission
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

        TableOptions<Power, GenericRequest<Power>> tableOptions = new();
        IEnumerable<PowerTreeNode> powerTree;
        IEnumerable<Power> allPower;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //await InitPowerTree();
            tableOptions.Page = false;
            tableOptions.LoadDataOnLoaded = true;
            tableOptions.TreeChildren = p => p.Children;
            tableOptions.DefaultExpandAllRows = true;
            tableOptions.DataLoader = Search;
            var addPowerBtn = new ButtonDefinition<Power>
            {
                Label = Localizer["PermissionSetting.AddChild"],
                Callback = AddPower,
                Visible = p => p.PowerType == PowerType.Page

            };
            tableOptions.AddButton(ButtonDefinition<Power>.Edit(EditPower));
            tableOptions.AddButton(ButtonDefinition<Power>.Delete(DeletePower));
            tableOptions.AddButton(addPowerBtn);
        }

        #region 初始化权限树
        //bool loading = false;
        //async Task InitPowerTree()
        //{
        //    loading = true;
        //    await GetAllPowersAsync();
        //    await GeneratePowerTreeDataAsync();
        //    loading = false;
        //}
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
        IEnumerable<Power> GeneratePowerTreeDataAsync(IEnumerable<Power> all)
        {
            List<Power> powers = new();
            var topLevel = all.Min(p => p.PowerLevel);
            var rootNodes = all.Where(p => p.PowerLevel == topLevel);
            foreach (var item in rootNodes)
            {
                //var n = (Power)item.Clone();
                item.Children = FindChildren(all, item);
                powers.Add(item);
            }

            return powers;

            List<Power> FindChildren(IEnumerable<Power> all, Power parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<Power> childNodes = new();
                foreach (var child in children)
                {
                    //var n1 = (Power)child.Clone();
                    child.Children = FindChildren(all, child);
                    childNodes.Add(child);
                }
                return childNodes;
            }
        }
        #endregion

        async Task<IQueryCollectionResult<Power>> Search(GenericRequest<Power> req)
        {
            var result = await PermissionSrv.GetPowerListAsync();
            var powers = result.Payload;
            result.Payload = GeneratePowerTreeDataAsync(powers);
            return result;
        }
        string[] defaultPageButtons = new[] { "Add", "Modify", "Delete" };
        async Task<bool> AddPower(Power parent)
        {
            var power = await ModalSrv.OpenDialog<PowerForm, Power>("新增权限");
            power.ParentId = parent.PowerId;
            power.PowerLevel = parent.PowerLevel + 1;
            power.Sort = parent.Children.Count() + 1;
            var result = await PermissionSrv.InsertPowerAsync(power);
            if (result.Success)
            {
                if (power.PowerType == PowerType.Page && power.GenerateCRUDButton)
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
                return true;
                //await InitPowerTree();
            }
            return false;
        }
        async Task<bool> EditPower(Power node)
        {
            var p = await ModalSrv.OpenDialog<PowerForm, Power>("编辑权限", node);
            var result = await PermissionSrv.UpdatePowerAsync(p);
            //await InitPowerTree();
            return result.Success;
        }

        async Task<bool> DeletePower(Power node)
        {
            var result = await PermissionSrv.DeletePowerAsync(node);
            return result.Success;
        }
    }
}
