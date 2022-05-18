using AntDesign;
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
    public partial class PermissionSetting
    {
        [Inject]
        public ModalService ModalSrv { get; set; }
        [Inject]
        public DrawerService DrawerSrv { get; set; }
        [Inject]
        public IPermissionService PermissionSrv { get; set; }
        TableOptions<Power, GeneralReq<Power>> tableOptions = new();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions.LoadDataOnLoaded = true;
            tableOptions.DataLoader = Search;
            tableOptions.AddHandle = AddPower;
            tableOptions.AddButton(ButtonDefinition<Power>.Edit(EditPower));
            tableOptions.AddButton(ButtonDefinition<Power>.Delete(DeletePower));
        }
        Task<IQueryCollectionResult<Power>> Search(GeneralReq<Power> req)
        {
            return PermissionSrv.GetPowerListAsync(req);
        }
        async Task<bool> AddPower()
        {
            var power = await ModalSrv.OpenDialog<PowerForm, Power>("新增权限");
            await PermissionSrv.InsertPowerAsync(power);
            return true;
        }
        async Task EditPower(Power power)
        {
            var p = await ModalSrv.OpenDialog<PowerForm, Power>("编辑权限", power);
            await PermissionSrv.UpdatePowerAsync(p); 
        }

        Task DeletePower(Power power)
        {
            //TODO delete power
            return Task.CompletedTask;
        }

    }
}
