using AntDesign;
using BlazorWebAdmin.Template.Forms.EntityForms;
using BlazorWebAdmin.Template.Tables;
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
		public IUserService UserSrv { get; set; }
		protected override void OnInitialized()
		{
			base.OnInitialized();
			tableOptions = new TableOptions<User, GeneralReq<User>>();
			tableOptions.DataLoader = Load;
			tableOptions.AddHandle = AddUser;
		}

		Task<QueryResult<PagingResult<User>>> Load(GeneralReq<User> req)
		{
			return UserSrv.GetUserListAsync(req);
		}

		async Task<bool> AddUser()
        {
			var user = await ModalSrv.OpenDialog<UserForm, User>("创建用户");
			await UserSrv.InsertUserAsync(user);
			return true;
        }
	}
}
