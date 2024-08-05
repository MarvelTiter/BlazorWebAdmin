
using BlazorAdmin.Client;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

//[assembly: GenerateApiInvoker]
var builder = WebAssemblyHostBuilder.CreateDefault(args);
//Config.IsClient = true;
//Config.HostUrl = builder.HostEnvironment.BaseAddress;
//builder.Services.AddSample();
//builder.Services.AddSampleClient();
builder.Services.AutoInjectWasm();
await builder.Build().RunAsync();

