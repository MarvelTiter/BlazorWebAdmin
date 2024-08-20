using Microsoft.AspNetCore.Components.Server.Circuits;
using Project.Constraints.Store;

namespace Project.AppCore.Middlewares;

public class CircuitTracker : CircuitHandler
{
    private readonly ClientInfo circuitInfo = new("");

    public CircuitTracker(IUserStore store)
    {
        circuitInfo.UserStore = store;
    }

    public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        circuitInfo.CircuitId = circuit.Id;
        circuitInfo.CreateTime = DateTime.Now;
        CircuitTrackerGlobalInfo.CircuitClients.TryAdd(circuit.Id, circuitInfo);
        return Task.CompletedTask;
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        CircuitTrackerGlobalInfo.CircuitClients.TryRemove(circuit.Id, out _);
        return Task.CompletedTask;
    }
}