﻿using Microsoft.AspNetCore.Components;
using Project.Constraints.Models.Permissions;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Components;
using System.Diagnostics;

namespace Project.AppCore.SystemPermission
{
    public partial class UserPage<TUser, TPower, TRole> : ModelPage<TUser, GenericRequest<TUser>>
        where TUser : class, IUser, new()
        where TPower : class, IPower, new()
        where TRole : class, IRole, new()
    {
        [Inject] public IUserService<TUser> UserSrv { get; set; }
        [Inject] public IStringLocalizer Localizer { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.Pager = false;
            Options.LoadDataOnLoaded = true;
            Options.GetColumn(u => u.Password).ValueFormat = val =>
            {
                return "******";
            };
        }

        protected override object SetRowKey(TUser model) => model.UserId;

        protected override async Task<IQueryCollectionResult<TUser>> OnQueryAsync(GenericRequest<TUser> query)
        {
            return await UserSrv.GetUserListAsync(query);
        }

        protected override Task OnRowClickAsync(TUser model)
        {
            sideExpand = true;
            currentSelected = model;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected TUser currentSelected;
        bool sideExpand;

        protected override async Task<bool> OnAddItemAsync()
        {
            var user = await this.ShowAddFormAsync(Localizer["User.DialogTitle.Add"]);
            user.OnUserSave(SaveActionType.Insert);
            await UserSrv.InsertUserAsync(user);
            return true;
        }

        [EditButton]
        public async Task<bool> EditUser(TUser user)
        {
            var n = await this.ShowEditFormAsync(Localizer["User.DialogTitle.Modify"], user);
            n.OnUserSave(SaveActionType.Update);
            await UserSrv.UpdateUserAsync(n);
            return true;
        }

        [DeleteButton]
        public async Task<bool> DeleteUser(TUser user)
        {
            var ret = await UserSrv.DeleteUserAsync(user);
            return ret.Success;
        }

    }
}
