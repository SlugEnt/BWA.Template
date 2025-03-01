using BWA.Weather;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using SlugEnt.BWA.Client.Weather;
using SlugEnt.BWA.Database;
using SlugEnt.IS.AppRun;
using MudBlazor.Services;

namespace SlugEnt.BWA.Server;

/// <summary>
/// This is the class you put all your applications unique requirements for starting the app.
/// </summary>
public class ProgramCustom : ProgramCustomBase
{
    /// <summary>
    /// This class is where you place all your custom code that the BWA Server / API needs to start up.  Normally this is in Program.cs, but Program.cs is meant to be abstract / the same so it
    /// can easily be updated across projects if needed.
    /// </summary>
    /// <param name="appRuntime"></param>
    public ProgramCustom(AppRuntime appRuntime) : base(appRuntime) { }


    /// <summary>
    /// Configures the database for the application
    /// </summary>
    /// <param name="builder"></param>
    public override void ConfigureDB(WebApplicationBuilder builder)
    {
        base.ConfigureDB(builder);

        string connStr = builder.Configuration.GetConnectionString(AppDbContext.DatabaseReferenceName());
        _appRuntime.Logger.Warning("Database Connection:  Using value found in Configuration: " + connStr);
        builder.Services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseSqlServer(connStr)
                   .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))

                   // IF Debug then log all SQL to Console
                   .EnableDetailedErrors();
        });
    }


    /// <summary>
    /// Place services your api / server needs here.
    /// </summary>
    /// <param name="builder"></param>
    public override void AddServices(WebApplicationBuilder builder)
    {
        base.AddServices(builder);
        builder.Services.AddMudServices();
        builder.Services.AddScoped<IWeatherForecaster, ServerWeatherForecaster>();

    }


    /// <summary>
    /// This is the final chance for you to put customizations into the BuildApp process.  This is called at the very end of the BuildApp process.
    /// This is called after the Endpoints are mapped.  So, you can add custom endpoints here.
    /// </summary>
    /// <param name="app"></param>
    public override void BuildApp_Final(WebApplication app) { base.BuildApp_Final(app); }
}