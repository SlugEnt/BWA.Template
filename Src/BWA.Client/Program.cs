
using BWA.Client.Classes;
using BWA.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Extensions;
using MudBlazor.Services;
using MudExtensions.Services;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
using SlugEnt.BWA.Client.Classes;
using SlugEnt.BWA.Client.Weather;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.Models;
using SlugEnt.HR.NextGen.Entities.Models;


WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);

AppInfo appInfo = new(builder.HostEnvironment.BaseAddress);
builder.Services.AddSingleton<IAppInfo>(appInfo);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();

builder.Services.AddMudServices();
builder.Services.AddMudExtensions();

// use this to add MudServices and the MudBlazor.Extensions
builder.Services.AddMudServicesWithExtensions();

// or this to add only the MudBlazor.Extensions but please ensure that this is added after mud servicdes are added. That means after `AddMudServices`
builder.Services.AddMudExtensions();

builder.Services.AddMemoryCache();

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddSingleton<ErrorManager>();

// API Connector Services
builder.Services.AddScoped<IEntityRepositoryE2Int<User>, EntityDataServiceInt<User>>();
builder.Services.AddScoped<IEntityRepositoryE2Int<SampleInt>, EntityDataServiceInt<SampleInt>>();
builder.Services.AddScoped<IEntityRepositoryE2Long<SampleLong>, EntityDataServiceLong<SampleLong>>();
builder.Services.AddScoped<IEntityRepositoryE2Guid<SampleGuid>, EntityDataServiceGuid<SampleGuid>>();


builder.Services.AddHttpClient<IWeatherForecaster, ClientWeatherForecaster>(httpClient =>
{
    httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
WebAssemblyHost app = builder.Build();


await app.RunAsync();
