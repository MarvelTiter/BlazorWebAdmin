using Microsoft.AspNetCore.Components.Server.Circuits;
using Project.Constraints;
using Project.Constraints.Store;

namespace Project.AppCore.Middlewares
{
    public class CircuitTracker : CircuitHandler
    {
        private ClientInfo _circuitInfo = new();
        private string _visitorId = "";
        private readonly IUserStore store;
        private readonly IAuthenticationStateProvider auth;

        public CircuitTracker(IUserStore store, IAuthenticationStateProvider auth)
        {
            store.LoginSuccessEvent += Store_LoginSuccessEvent;
            this.store = store;
            this.auth = auth;
        }

        private Task Store_LoginSuccessEvent(UserInfo arg)
        {
            _circuitInfo.UserInfo = arg;
            _circuitInfo.Context = auth.HttpContext;
            return Task.CompletedTask;
        }

        public string CircuitId => _circuitInfo.CircuitId;
        public string VisitorId => _visitorId;

        public override Task OnConnectionUpAsync(Circuit circuit, CancellationToken cancellationToken)
        {
            _circuitInfo.CircuitId = circuit.Id;
            _circuitInfo.CreateTime = DateTime.Now;
            CircuitTrackerGlobalInfo.CircuitClients.TryAdd(circuit.Id, _circuitInfo);
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
