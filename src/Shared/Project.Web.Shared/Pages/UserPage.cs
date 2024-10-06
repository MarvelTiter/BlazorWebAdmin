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
        where TPermissionService : IPermissionService<TPower, TRole>
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
            var user = await this.ShowAddFormAsync(option =>
            {
                option.Title = Localizer["User.DialogTitle.Add"];
                option.PostCheckAsync = async (u, validate) =>
                {
                    if (!validate.Invoke() || u is null)
                    {
                        return false;
                    }
                    var result = await UserSrv.InsertUserAsync(u);
                    return UI.ShowResult(result);
                };
            });
            user.OnUserSave(SaveActionType.Insert);
            return true;
        }

        protected override async Task<QueryResult> HandleImportedDataAsync(TUser data)
        {
            await Task.CompletedTask;
            return true;
        }

        [EditButton]
        public async Task<bool> EditUser(TUser user)
        {
            var userRoles = await PermissionSrv.GetUserRolesAsync(user.UserId);
            user.Roles = userRoles.Payload?.Select(r => r.RoleId).ToList() ?? [];
            //var n = await this.ShowEditFormAsync(Localizer["User.DialogTitle.Modify"], user);
            var u = await this.ShowEditFormAsync(user, true, option =>
            {
                option.Title = Localizer["User.DialogTitle.Modify"];
                option.PostCheckAsync = (async (u, validate) =>
                {
                    if (!validate.Invoke() || u is null)
                    {
                        return false;
                    }
                    var saveUserResult = await UserSrv.UpdateUserAsync(u);
                    //var success = UI.ShowResult(saveUserResult);
                    //if (success)
                    //{
                    //    var saveUserRoleResult = await PermissionSrv.SaveUserRoleAsync((u.UserId, u.Roles));
                    //    //success = UI.ShowResult()
                    //}
                    //if (saveUserResult.Success && saveUserRoleResult.Success)
                    //{
                    //    UI.Success("用户信息保存成功！");
                    //    return true;
                    //}
                    //else
                    //{
                    //    UI.Error($"更新用户: {saveUserResult.Message} ; 更新用户角色: {saveUserRoleResult.Message}");
                    //    return false;
                    //}
                    return UI.ShowResult(saveUserResult);
                });
            });
            u.OnUserSave(SaveActionType.Update);
            return true;
        }

        [DeleteButton]
        public async Task<bool> DeleteUser(TUser user)
        {
            var ret = await UserSrv.DeleteUserAsync(user);
            return UI.ShowResult(ret);
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
