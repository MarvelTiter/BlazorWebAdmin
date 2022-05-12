using AntDesign;
using BlazorWebAdmin.Template.Forms.EntityForms;
using BlazorWebAdmin.Template.Tables;
using BlazorWebAdmin.Template.Tables.Setting;
using BlazorWebAdmin.Utils;
using Microsoft.AspNetCore.Components;
using Project.Models;
using Project.Models.Permissions;
using Project.Models.Request;

namespace BlazorWebAdmin.Pages.SystemPermission
{
    public partial class PermissionSetting
    {
        [Inject]
        public ModalService ModalSrv { get; set; }
        [Inject]
        public DrawerService DrawerSrv { get; set; }
        TableOptions<Power, GeneralReq<Power>> tableOptions = new();
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions.DataLoader = Search;
            tableOptions.AddHandle = AddPower;
            tableOptions.AddButton(ButtonDefinition<Power>.Edit(EditPower));
            tableOptions.AddButton(ButtonDefinition<Power>.Delete(DeletePower));
        }
        Task<QueryResult<PagingResult<Power>>> Search(GeneralReq<Power> req)
        {
            Console.WriteLine(req.Expression);
            return Task.FromResult(QueryResult<Power>.PagingResult(Enumerable.Empty<Power>(), 0));
        }
        async Task<bool> AddPower()
        {
            var power = await DrawerSrv.OpenDrawer<PowerForm, Power>("新增权限");
            //TODO save power
            return true;
        }
        async Task EditPower(Power power)
        {
            var p = await ModalSrv.OpenDialog<PowerForm, Power>("编辑权限", power);
            //TODO save power
        }

        Task DeletePower(Power power)
        {
            //TODO delete power
            return Task.CompletedTask;
        }

    }
}
