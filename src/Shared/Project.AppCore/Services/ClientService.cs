using AutoInjectGenerator;
using Microsoft.Extensions.Options;
using Project.Constraints.Options;
using System.Collections.Concurrent;

namespace Project.AppCore.Services;

[AutoInject(Group = "SERVER", LifeTime = InjectLifeTime.Singleton)]
public sealed class ClientService : IClientService, IDisposable
{
    private readonly ConcurrentDictionary<string, ClientInfo> clients = [];
    private readonly CancellationTokenSource tokenSource = new();
    private readonly Task clearTimeoutClientTask;
    private readonly IOptions<AppSetting> options;
    private bool disposedValue;

    public ClientService(IOptions<AppSetting> options)
    {
        clearTimeoutClientTask = new Task(ClearTimeoutClient, tokenSource.Token, TaskCreationOptions.LongRunning);
        clearTimeoutClientTask.Start();
        this.options = options;
    }

    public Task<QueryResult<int>> GetCountAsync()
    {
        return Task.FromResult(clients.Count.Result());
    }

    public Task<QueryCollectionResult<ClientInfo>> GetClientsAsync(GenericRequest<ClientInfo> query)
    {
        RemoveExpired();
        var filted = clients.Values
            .Where(query.Expression().Compile())
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize);
        return Task.FromResult(filted.CollectionResult());
    }

    public Task<QueryResult> AddOrUpdateAsync(ClientInfo client)
    {
        clients.AddOrUpdate(client.CircuitId, client, (_, _) => UpdateClient(client));
        return Task.FromResult(Result.Success());
    }

    public Task<QueryResult> CheckPermissionAsync(UserInfo? user)
    {
        if (user == null)
        {
            return Task.FromResult(Result.Fail());
        }
        var userAllow = options.Value.ClientHubOptions.AllowUsers.Contains(user.UserId);
        var roleAllow = Inset(options.Value.ClientHubOptions.AllowRoles, user.Roles);
        return Task.FromResult(Result.Return(userAllow || roleAllow));
    }

    private async void ClearTimeoutClient()
    {
        while (!tokenSource.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(options.Value.ClientHubOptions.ServerScanFrequency, tokenSource.Token);
                RemoveExpired();
            }
            catch
            {

            }
        }
    }

    private void RemoveExpired()
    {
        // 距离上一次的心跳超过超时限制时间
        var expired = clients.Values.Where(c => DateTime.Now - c.BeatTime > options.Value.ClientHubOptions.ClearTimeoutLimit).Select(c => c.CircuitId).ToList();
        expired.ForEach(id => clients.TryRemove(id, out _));
    }


    private static ClientInfo UpdateClient(ClientInfo client)
    {
        client.BeatTime = DateTime.Now;
        return client;
    }

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (!tokenSource.IsCancellationRequested)
                {
                    tokenSource.Cancel();
                }
                tokenSource.Dispose();
                clearTimeoutClientTask.Dispose();
            }
            disposedValue = true;
        }
    }


    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
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