using AutoWasmApiGenerator;
using Project.Constraints.Models.Request;

namespace Project.Constraints.Services;

[WebController(Route = "hub")]
public interface IClientService
{
    Task<QueryResult<int>> GetCountAsync();
    Task<QueryCollectionResult<ClientInfo>> GetClientsAsync(GenericRequest<ClientInfo> query);
    //Task<QueryResult> CheckPermissionAsync(UserInfo? user);
    Task<QueryResult> AddOrUpdateAsync(ClientInfo client);
    // Task<QueryResult<ClientInfo>> GetClientAsync(string key);
}