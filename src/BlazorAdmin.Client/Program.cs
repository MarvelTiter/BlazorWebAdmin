
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

//[assembly: GenerateApiInvoker]
var builder = WebAssemblyHostBuilder.CreateDefault(args);
//Config.IsClient = true;
//Config.HostUrl = builder.HostEnvironment.BaseAddress;
//builder.Services.AddSample();
//builder.Services.AddSampleClient();
await builder.Build().RunAsync();

