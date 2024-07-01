using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Http;
using Project.Constraints;
using Project.Constraints.Store;

namespace Project.AppCore.Middlewares
{
    public class CircuitTracker : CircuitHandler
    {
        private ClientInfo circuitInfo = new();
        private readonly IUserStore store;
        private readonly IHttpContextAccessor contextAccessor;

        public CircuitTracker(IUserStore store, IHttpContextAccessor contextAccessor)
        {
            store.LoginSuccessEvent += Store_LoginSuccessEvent;
            this.store = store;
            this.contextAccessor = contextAccessor;
        }

        private Task Store_LoginSuccessEvent(UserInfo arg)
        {
            circuitInfo.UserInfo = arg;
            return Task.CompletedTask;
        }

        public string CircuitId => circuitInfo.CircuitId;

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            circuitInfo.CircuitId = circuit.Id;
            circuitInfo.CreateTime = DateTime.Now;
            circuitInfo.IpAddress = contextAccessor.HttpContext?.Connection.RemoteIpAddress.ToIpString();
            circuitInfo.UserAgent = contextAccessor.HttpContext?.Request.Headers.UserAgent;
            CircuitTrackerGlobalInfo.CircuitClients.TryAdd(circuit.Id, circuitInfo);
            return Task.CompletedTask;
        }

        public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            CircuitTrackerGlobalInfo.CircuitClients.TryRemove(circuit.Id, out _);
            store.LoginSuccessEvent -= Store_LoginSuccessEvent;
            return Task.CompletedTask;
        }
    }
}
