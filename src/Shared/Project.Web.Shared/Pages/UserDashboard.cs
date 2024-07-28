using Microsoft.AspNetCore.Components;
using Project.Constraints.Common.Attributes;
using Project.Constraints.UI.Extensions;
using Project.Web.Shared.Pages.Component;
using System.Text.Json;

namespace Project.AppCore.Clients
{
    [Route("/userdashboard")]
    [PageInfo(Icon = "setting", Title = "在线用户")]
    public class UserDashboard : ModelPage<ClientInfo, GenericRequest>
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            Options.LoadDataOnLoaded = true;
        }

        protected override async Task<QueryCollectionResult<ClientInfo>> OnQueryAsync(GenericRequest query)
        {
            await Task.Delay(1);
            return CircuitTrackerGlobalInfo.CircuitClients.Values.Where(c => c.UserAgent != null).CollectionResult();
        }

        [TableButton(Label = "用户信息")]
        public async Task<bool> ShowUserInfo(ClientInfo info)
        {
            _ = await UI.ShowDialogAsync<JsonDisplay, ClientInfo>("用户信息", info, width: "50%");
            return true;
        }
    }
}
