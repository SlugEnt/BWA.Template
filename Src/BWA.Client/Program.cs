using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using MudExtensions.Services;

using SlugEnt.BWA.Client.Weather;


var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();


builder.Services.AddMudServices();
builder.Services.AddMudExtensions();

builder.Services.AddHttpClient<IWeatherForecaster, ClientWeatherForecaster>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

await builder.Build().RunAsync();
