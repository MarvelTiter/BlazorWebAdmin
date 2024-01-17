﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.Models.Permissions;

namespace Project.AppCore.SystemPermission
{
    public partial class PermissionSetting : ModelPage<Power, GenericRequest<Power>>
    {
        [Inject] public IPermissionService PermissionSrv { get; set; }
        [Inject] IStringLocalizer<Power> Localizer { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //await InitPowerTree();
            HideDefaultTableHeader = true;
            Options.Pager = false;
            Options.LoadDataOnLoaded = true;
            Options.TreeChildren = p => p.Children;
            Options.GetColumn(p => p.Icon).FormTemplate = IconSelect();

        }
        protected override object SetRowKey(Power model) => model.PowerId;
        protected override async Task<IQueryCollectionResult<Power>> OnQueryAsync(GenericRequest<Power> query)
        {
            var result = await PermissionSrv.GetPowerListAsync();
            var powers = result.Payload;
            result.Payload = GeneratePowerTreeDataAsync(powers);
            return result;
        }

        #region 初始化权限树
        /// <summary>
        /// 构建权限树
        /// </summary>
        /// <returns></returns>
        static List<Power> GeneratePowerTreeDataAsync(IEnumerable<Power> all)
        {
            List<Power> powers = new();
            var topLevel = all.Min(p => p.PowerLevel);
            var rootNodes = all.Where(p => p.PowerLevel == topLevel);
            foreach (var item in rootNodes)
            {
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
                    child.Children = FindChildren(all, child);
                    childNodes.Add(child);
                }
                return childNodes;
            }
        }
        #endregion

        string[] defaultPageButtons = new[] { "Add", "Modify", "Delete" };
        bool test;
        public bool CanShow(TableButtonContext<Power> context) => context.Data.PowerType == PowerType.Page;
        public string AddPowerLabel(TableButtonContext<Power> _) => Localizer["PermissionSetting.AddChild"];

        [TableButton(LabelExpression =nameof(AddPowerLabel), VisibleExpression = nameof(CanShow))]
        public async Task<bool> AddPower(Power parent)
        {
            var power = await this.ShowAddFormAsync("新增权限");
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
            }
            return false;
        }

        [EditButton]
        public async Task<bool> EditPower(Power node)
        {
            var p = await this.ShowEditFormAsync("编辑权限", node);
            if (p.Icon != selectedIcon && !string.IsNullOrEmpty(selectedIcon))
            {
                p.Icon = selectedIcon;
            }
            var result = await PermissionSrv.UpdatePowerAsync(p);
            return result.Success;
        }

        [DeleteButton]
        public async Task<bool> DeletePower(Power node)
        {
            var result = await PermissionSrv.DeletePowerAsync(node);
            return result.Success;
        }

    }
}
