using System.Collections.Concurrent;

namespace Project.Web.Shared.Services;

[AutoInject(LifeTime = InjectLifeTime.Singleton)]
public sealed class ClientService : IClientService, IDisposable
{
    private readonly ConcurrentDictionary<string, ClientInfo> clients = [];
    private readonly CancellationTokenSource tokenSource = new();
    private readonly Task clearTimeoutClientTask;
    private bool disposedValue;

    public ClientService()
    {
        clearTimeoutClientTask = new Task(ClearTimeoutClient, tokenSource.Token, TaskCreationOptions.LongRunning);
        clearTimeoutClientTask.Start();
    }

    public Task<QueryResult<int>> GetCountAsync()
    {
        return Task.FromResult(clients.Count.Result());
    }

    public Task<QueryCollectionResult<ClientInfo>> GetClientsAsync()
    {
        return Task.FromResult(clients.Values.CollectionResult());
    }

    public Task<QueryResult> AddOrUpdateAsync(ClientInfo client)
    {
        clients.AddOrUpdate(client.CircuitId, client, UpdateClient);
        return Task.FromResult(Result.Success());
    }

    private void ClearTimeoutClient()
    {
        while (!tokenSource.IsCancellationRequested)
        {
            try
            {
                await Task.Delay()
            }
            catch
            {

            }
        }
    }

    // public Task<QueryResult<ClientInfo>> GetClientAsync(string key)
    // {
    //     if (!clients.TryGetValue(key, out var client))
    //     {
    //     }
    // }

    private static ClientInfo UpdateClient(string circuitId, ClientInfo client)
    {
        client.BeatTime = DateTime.Now;
        Console.WriteLine("UpdateClient");
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