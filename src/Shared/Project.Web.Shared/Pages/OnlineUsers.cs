using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Options;
using Project.Constraints.Common.Attributes;
using Project.Constraints.Options;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Components;
using Project.Web.Shared.Pages.Component;
using System.Text.Json;

namespace Project.AppCore.Clients
{
    [Route("/userdashboard")]
    [PageInfo(Icon = "setting", Title = "在线用户")]
    public class OnlineUsers : ModelPage<ClientInfo, GenericRequest>
    {
        [Inject, NotNull] IClientService? ClientService { get; set; }
        [Inject, NotNull] IOptions<AppSetting>? AppOptions { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.LoadDataOnLoaded = true;
        }

        protected override Task<QueryCollectionResult<ClientInfo>> OnQueryAsync(GenericRequest query)
        {
            return ClientService.GetClientsAsync();
        }

        [TableButton(Label = "用户信息")]
        public async Task<bool> ShowUserInfo(ClientInfo info)
        {
            _ = await UI.ShowDialogAsync<JsonDisplay, ClientInfo>("用户信息", info, width: "50%");
            return true;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var userAllow = AppOptions.Value.ClientHubOptions.AllowUsers.Contains(User.UserId);
            var roleAllow = Inset(AppOptions.Value.ClientHubOptions.AllowRoles, User.Roles);
            if (userAllow || roleAllow)
            {
                base.BuildRenderTree(builder);
            }
            else
            {
                builder.Component<ForbiddenPage>().Build();
            }
        }

        private static bool Inset<T>(IEnumerable<T> values1, IEnumerable<T> values2)
        {
            foreach (var v1 in values1)
            {
                return values2.Any(v2 => Equals(v1, v2));
            }
            return false;
        }
    }
}
