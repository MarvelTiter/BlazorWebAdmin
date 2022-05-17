using AntDesign;
using AntDesign.TableModels;
using BlazorWebAdmin.Template.Forms.EntityForms;
using BlazorWebAdmin.Template.Tables;
using BlazorWebAdmin.Template.Tables.Setting;
using BlazorWebAdmin.Utils;
using Microsoft.AspNetCore.Components;
using Project.Models;
using Project.Models.Entities;
using Project.Models.Request;
using Project.Services.interfaces;

namespace BlazorWebAdmin.Pages.SystemPermission
{
    public partial class UserIndex
    {
        TableOptions<User, GeneralReq<User>> tableOptions;
        [Inject]
        public ModalService ModalSrv { get; set; }
        [Inject]
        public DrawerService DrawerSrv { get; set; }
        [Inject]
        public IUserService UserSrv { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            tableOptions = new TableOptions<User, GeneralReq<User>>();
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

        Task<QueryResult<PagingResult<User>>> Search(GeneralReq<User> req)
        {
            return UserSrv.GetUserListAsync(req);
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
        public async Task EditUser(User user)
        {
            var options = new ModalOptions();
            options.Title = "用户信息";
            var n = await ModalSrv.OpenDialog<UserForm, User>(options, user);
        }

        public Task DeleteUser(User user)
        {
            // delete
            return Task.CompletedTask;
        }
    }
}
