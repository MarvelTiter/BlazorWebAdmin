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

    public Task<QueryCollectionResult<ClientInfo>> GetClientsAsync()
    {
        RemoveExpired();
        return Task.FromResult(clients.Values.CollectionResult());
    }

    public Task<QueryResult> AddOrUpdateAsync(ClientInfo client)
    {
        clients.AddOrUpdate(client.CircuitId, client, (_, _) => UpdateClient(client));
        return Task.FromResult(Result.Success());
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

    // public Task<QueryResult<ClientInfo>> GetClientAsync(string key)
    // {
    //     if (!clients.TryGetValue(key, out var client))
    //     {
    //     }
    // }

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
}