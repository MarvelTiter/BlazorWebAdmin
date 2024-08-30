using AutoWasmApiGenerator;

namespace Project.Constraints.Services;

[WebController(Route = "hub")]
[ApiInvokerGenerate]
public interface IClientService
{
    Task<QueryResult<int>> GetCountAsync();
    Task<QueryCollectionResult<ClientInfo>> GetClientsAsync();

    Task<QueryResult> AddOrUpdateAsync(ClientInfo client);
    // Task<QueryResult<ClientInfo>> GetClientAsync(string key);
}