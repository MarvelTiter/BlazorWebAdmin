using Microsoft.AspNetCore.Components;
using Project.Constraints.UI.Extensions;

namespace Project.AppCore.SystemPermission
{
    public partial class CustomUserPage : ModelPage<User, GenericRequest<User>>
    {
        [Inject] public IUserService UserSrv { get; set; }
        [Inject] public IStringLocalizer<CustomUserPage> Localizer { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.Pager = false;
            Options.LoadDataOnLoaded = true;
            Options[u => u.Password]!.ValueFormat = val =>
            {
                return "******";
            };
        }

        protected override object SetRowKey(User model) => model.UserId;

        protected override async Task<IQueryCollectionResult<User>> OnQueryAsync(GenericRequest<User> query)
        {
            return await UserSrv.GetUserListAsync(query);
        }

        protected override Task OnRowClickAsync(User model)
        {
            sideExpand = true;
            currentSelected = model;
            StateHasChanged();
            return Task.CompletedTask;
        }

        protected User currentSelected;
        bool sideExpand;



        protected override async Task<bool> OnAddItemAsync()
        {
            var user = await this.ShowAddFormAsync(Localizer["User.DialogTitle.Add"], width:"60%");
            await UserSrv.InsertUserAsync(user);
            return true;
        }

        [EditButton]
        public async Task<bool> EditUser(User user)
        {
            //var content = UI.BuildForm(new Constraints.UI.Form.FormOptions<User>(UI, user, Options.Columns));
            var n = await this.ShowEditFormAsync(Localizer["User.DialogTitle.Modify"], user, width: "60%");
            await UserSrv.UpdateUserAsync(n);
            return true;
        }

        [DeleteButton]
        public async Task<bool> DeleteUser(User user)
        {
            var ret = await UserSrv.DeleteUserAsync(user);
            return ret.Success;
        }
    }
}
