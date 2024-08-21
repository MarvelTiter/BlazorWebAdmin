using Microsoft.AspNetCore.Components;
using Project.Constraints.PageHelper;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Pages.Component;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Project.Web.Shared.Pages
{
    public class UserPage<TUser, TPower, TRole, TUserService, TPermissionService> : ModelPage<TUser, GenericRequest<TUser>>, IPageAction
        where TUser : class, IUser, new()
        where TPower : class, IPower, new()
        where TRole : class, IRole, new()
        where TUserService : IUserService<TUser>
        where TPermissionService: IPermissionService<TPower, TRole>
    {
        [Inject, NotNull] public TUserService? UserSrv { get; set; }
        [Inject, NotNull] public TPermissionService? PermissionSrv { get; set; }
        [Inject, NotNull] public IStringLocalizer? Localizer { get; set; }

        IEnumerable<TRole> allRoles = [];
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.Pager = false;
            Options.LoadDataOnLoaded = true;
            Options.Exportable = true;
            Options.GetColumn(u => u.Password).ValueFormat = val =>
            {
                return "******";
            };
            Options.GetColumn(u => u.Roles).FormTemplate = ctx => b =>
                b.Component<AssignUserRoles<TRole>>()
                 .SetComponent(c => c.Roles, allRoles)
                 .SetComponent(c => c.Ctx, ctx).Build();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await UpdateRoles();
        }

        protected override async Task<QueryCollectionResult<TUser>> OnQueryAsync(GenericRequest<TUser> query)
        {
            return await UserSrv.GetUserListAsync(query);
        }

        protected override async Task<bool> OnAddItemAsync()
        {
            var user = await this.ShowAddFormAsync(Localizer["User.DialogTitle.Add"]);
            user.OnUserSave(SaveActionType.Insert);
            await UserSrv.InsertUserAsync(user);
            return true;
        }

        protected override Task OnDataImportsAsync()
        {
            return base.OnDataImportsAsync();
        }

        [EditButton]
        public async Task<bool> EditUser(TUser user)
        {
            var userRoles = await PermissionSrv.GetUserRolesAsync(user.UserId);
            user.Roles = userRoles.Payload?.Select(r => r.RoleId).ToList() ?? [];
            var n = await this.ShowEditFormAsync(Localizer["User.DialogTitle.Modify"], user);
            n.OnUserSave(SaveActionType.Update);
            await UserSrv.UpdateUserAsync(n);
            await PermissionSrv.SaveUserRoleAsync((n.UserId, n.Roles));
            return true;
        }

        [DeleteButton]
        public async Task<bool> DeleteUser(TUser user)
        {
            var ret = await UserSrv.DeleteUserAsync(user);
            return ret.Success;
        }

        private async Task UpdateRoles()
        {
            var result = await PermissionSrv.GetAllRoleAsync();
            allRoles = result.Payload;
        }

        public Task OnShowAsync()
        {
            return UpdateRoles();
        }

        public Task OnHiddenAsync()
        {
            return Task.CompletedTask;
        }
    }
}
