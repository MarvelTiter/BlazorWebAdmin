using AntDesign;
using BlazorWeb.Shared.Template.Tables;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using BlazorWebAdmin.SystemPermission.Forms;
using Microsoft.AspNetCore.Components;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;

namespace BlazorWebAdmin.SystemPermission
{
    public partial class PermissionSetting
    {
        [Inject]
        public ModalService ModalSrv { get; set; }
        [Inject]
        public DrawerService DrawerSrv { get; set; }
        [Inject]
        public IPermissionService PermissionSrv { get; set; }
        TableOptions<Power, GenericRequest<Power>> tableOptions = new();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions.LoadDataOnLoaded = true;
            tableOptions.DataLoader = Search;
            tableOptions.AddHandle = AddPower;
            tableOptions.AddButton(ButtonDefinition<Power>.Edit(EditPower));
            tableOptions.AddButton(ButtonDefinition<Power>.Delete(DeletePower));
        }
        Task<IQueryCollectionResult<Power>> Search(GenericRequest<Power> req)
        {
            return PermissionSrv.GetPowerListAsync(req);
        }
        async Task<bool> AddPower()
        {
            var power = await ModalSrv.OpenDialog<PowerForm, Power>("新增权限");
            var result = await PermissionSrv.InsertPowerAsync(power);
            return result.Success;
        }
        async Task<bool> EditPower(Power power)
        {
            var p = await ModalSrv.OpenDialog<PowerForm, Power>("编辑权限", power);
            var result = await PermissionSrv.UpdatePowerAsync(p);
            return result.Success;
        }

        Task<bool> DeletePower(Power power)
        {
            //TODO delete power
            return Task.FromResult(false);
        }

    }
}
