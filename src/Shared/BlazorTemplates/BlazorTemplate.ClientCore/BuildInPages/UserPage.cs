using BlazorTemplate.ClientCore.UI.Extensions;
using BlazorTemplate.ClientCore.BuildInPages.Component;
using AutoPageStateContainerGenerator;

namespace BlazorTemplate.ClientCore.BuildInPages;

public partial class UserPage<TUser, TPower, TRole, TUserService, TPermissionService> : ModelPage<TUser, GenericRequest<TUser>>
    where TUser : class, IUser, new()
    where TPower : class, IPermission, new()
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
        Options.EnableRowEdit = true;
        Options.GetColumn(u => u.Password).ValueFormat = val => "******";
        Options.GetColumn(u => u.Roles).FormTemplate = ctx => b =>
            b.Component<AssignUserRoles<TRole>>()
                .SetComponent(c => c.Roles, allRoles)
                .SetComponent(c => c.Ctx, ctx).Build();
        var btn = new TableButton<TUser>();
        btn.Callback = EditUser;
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

    protected override async Task<IQueryResult?> OnAddItemAsync()
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
                return UI.ShowError(result);
            };
        });
        user.OnUserSave(SaveActionType.Insert);
        return QueryResult.Success();
    }

    protected override async Task<QueryResult> HandleImportedDataAsync(TUser data)
    {
        await Task.CompletedTask;
        return true;
    }

    [EditButton]
    public async Task<IQueryResult?> EditUser(TUser user)
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
                u.OnUserSave(SaveActionType.Update);
                var saveUserResult = await UserSrv.SaveUserWithRolesAsync(u);
                return UI.ShowError(saveUserResult);
            });
        });
        return QueryResult.Success();
    }

    [DeleteButton]
    public async Task<IQueryResult> DeleteUser(TUser user)
    {
        return await UserSrv.DeleteUserAsync(user);
    }

    private async Task UpdateRoles()
    {
        var result = await PermissionSrv.GetAllRoleAsync();
        allRoles = result.Payload;
    }
}

[StateContainer]
public partial class TemplateUserPage : UserPage<TemplateUser, TemplatePermission, TemplateRole, ITemplateUserService, ITemplatePermissionService>
{
    protected override async Task<IQueryResult?> OnCellUpdateAsync(TemplateUser model, ColumnInfo col)
    {
        return await UserSrv.SavePropertyAsync(model, col.PropertyOrFieldName);
    }

    protected override async Task<IQueryResult?> OnRowUpdateAsync(TemplateUser model, IReadOnlyList<ColumnInfo> col)
    {
        return await UserSrv.SavePropertiesAsync(model, [.. col.Select(c => c.PropertyOrFieldName)]);
    }
}
