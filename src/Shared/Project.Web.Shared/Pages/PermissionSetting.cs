using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Project.Constraints.Models.Permissions;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Pages.Component;

namespace Project.Web.Shared.Pages
{
    public class PermissionSetting<TPower, TRole,TPermissionService> : ModelPage<TPower, GenericRequest<TPower>>
        where TPower : class, IPower, new()
        where TRole : class, IRole, new()
        where TPermissionService : IPermissionService<TPower, TRole>
    {
        [Inject, NotNull] public TPermissionService? PermissionSrv { get; set; }
        [Inject, NotNull] IStringLocalizer<TPower>? Localizer { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            //await InitPowerTree();
            HideDefaultTableHeader = true;
            Options.Pager = false;
            Options.LoadDataOnLoaded = true;
            Options.TreeChildren = p => p.Children?.Cast<TPower>() ?? [];
            Options.GetColumn(p => p.Icon).FormTemplate = ctx => b =>
                b.Component<PowerIconSelector>()
                .SetComponent(c => c.Context, ctx)
                .Build();

        }
        protected override object SetRowKey(TPower model) => model.PowerId;
        protected override async Task<IQueryCollectionResult<TPower>> OnQueryAsync(GenericRequest<TPower> query)
        {
            var result = await PermissionSrv.GetAllPowerAsync();
            var powers = result.Payload.Cast<TPower>();
            result.Payload = GeneratePowerTreeDataAsync(powers);
            return result;
        }

        #region 初始化权限树
        /// <summary>
        /// 构建权限树
        /// </summary>
        /// <returns></returns>
        static List<TPower> GeneratePowerTreeDataAsync(IEnumerable<TPower> all)
        {
            List<TPower> powers = new();
            var topLevel = all.Min(p => p.PowerLevel);
            var rootNodes = all.Where(p => p.PowerLevel == topLevel);
            foreach (var item in rootNodes)
            {
                item.Children = FindChildren(all, item);
                powers.Add(item);
            }

            return powers;

            List<TPower> FindChildren(IEnumerable<TPower> all, TPower parent)
            {
                var children = all.Where(p => p.ParentId == parent.PowerId);
                List<TPower> childNodes = new();
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
        public bool CanShow(TableButtonContext<TPower> context) => context.Data.PowerType == PowerType.Page;
        public string AddPowerLabel(TableButtonContext<TPower> _) => Localizer["PermissionSetting.AddChild"];

        [TableButton(LabelExpression = nameof(AddPowerLabel), VisibleExpression = nameof(CanShow))]
        public async Task<bool> AddPower(TPower parent)
        {
            var edit = new TPower();
            edit.ParentId = parent.PowerId;
            edit.PowerLevel = parent.PowerLevel + 1;
            edit.Sort = parent.Children?.Count() ?? 0 + 1;
            var power = await this.ShowEditFormAsync("新增权限", edit, false);
            var result = await PermissionSrv.InsertPowerAsync(power);
            if (result.Success)
            {
                if (power.PowerType == PowerType.Page && power.GenerateCRUDButton)
                {
                    foreach (var item in defaultPageButtons)
                    {
                        var p = new TPower
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
        public async Task<bool> EditPower(TPower node)
        {
            var p = await this.ShowEditFormAsync("编辑权限", node);
            var result = await PermissionSrv.UpdatePowerAsync(p);
            return result.Success;
        }

        [DeleteButton]
        public async Task<bool> DeletePower(TPower node)
        {
            var result = await PermissionSrv.DeletePowerAsync(node);
            return result.Success;
        }

    }
}
