using AntDesign;
using AntDesign.TableModels;
using BlazorWeb.Shared.Template.Forms.EntityForms;
using BlazorWeb.Shared.Template.Tables;
using BlazorWeb.Shared.Template.Tables.Setting;
using BlazorWeb.Shared.Utils;
using Microsoft.AspNetCore.Components;
using Project.AppCore.Services;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;

namespace BlazorWeb.UI.SystemPermission
{
    public partial class UserIndex
    {
        TableOptions<User, GenericRequest<User>> tableOptions;
        [Inject]
        public ModalService ModalSrv { get; set; }
        [Inject]
        public DrawerService DrawerSrv { get; set; }
        [Inject]
        public IUserService UserSrv { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions = new TableOptions<User, GenericRequest<User>>();
            tableOptions.LoadDataOnLoaded = true;
            tableOptions.AddButton(ButtonDefinition<User>.Edit(EditUser));
            tableOptions.AddButton(ButtonDefinition<User>.Delete(DeleteUser));
            tableOptions.DataLoader = Search;
            tableOptions.AddHandle = AddUser;
            tableOptions.OnRowClick = AssignRole;
            tableOptions[nameof(User.Password)].OnCell = cell =>
            {
                cell.FormattedValue = "********";
                return null;
            };
        }

        async Task<IQueryCollectionResult<User>> Search(GenericRequest<User> req)
        {
            await Task.Delay(2000);
            return await UserSrv.GetUserListAsync(req);
        }
        protected User currentSelected;
        bool sideExpand;
        Task AssignRole(RowData<User> row)
        {
            sideExpand = true;
            currentSelected = row.Data;
            StateHasChanged();
            return Task.CompletedTask;
        }
        async Task<bool> AddUser()
        {
            var user = await ModalSrv.OpenDialog<UserForm, User>("创建用户");
            await UserSrv.InsertUserAsync(user);
            return true;
        }
        public async Task<bool> EditUser(User user)
        {
            var n = await DrawerSrv.OpenDrawer<UserForm, User>("用户信息", user);
            return false;
        }

        public Task<bool> DeleteUser(User user)
        {
            // delete
            return Task.FromResult(false);
        }
    }
}
