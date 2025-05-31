using BWA.Weather;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Extensions;
using MudBlazor.Services;
using SlugEnt.BWA.Client.Weather;
using SlugEnt.BWA.Database;
using SlugEnt.IS.AppRun;
using MudExtensions.Services;
using SlugEnt.BWA.Common;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
using SlugEnt.FluentResults;
using SlugEnt.HR.NextGen.Entities.Models;

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
    /// Place services your api / server needs here.
    /// </summary>
    /// <param name="builder"></param>
    public override void AddServices(WebApplicationBuilder builder)
    {
        base.AddServices(builder);
        builder.Services.AddMudServices();
        builder.Services.AddMudExtensions();
        builder.Services.AddMudServicesWithExtensions();
        builder.Services.AddMudExtensions();

        builder.Services.AddTransient<AutomaticDataSeed>();
        builder.Services.AddScoped<StartupEngine>();
        builder.Services.AddSingleton<GlobalAppData>();
        builder.Services.AddSingleton<ErrorManager>();


        // Add your own controllers here:
        builder.Services.AddTransient<IEntityRepositoryE2Int<AppSetting>, E2EntityRepositoryInt<AppSetting>>();

        builder.Services.AddScoped<IWeatherForecaster, ServerWeatherForecaster>();

    }




    /// <summary>
    /// Allows application to add to the Build App process.  This is called near the end of the BuildApp process, but before
    /// the endpoints / controllers section.  This is most likely where you want to add something.
    /// </summary>
    /// <param name="app"></param>
    public override void BuildApp_UsesMiddle(WebApplication app)
    {
        app.Use(MudExWebApp.MudExMiddleware);
        app.UseODataRouteDebug();
    }


    /// <summary>
    /// Configures the database for the application
    /// </summary>
    /// <param name="builder"></param>
    public override void ConfigureDB(WebApplicationBuilder builder)
    {
        base.ConfigureDB(builder);

        string connStr = builder.Configuration.GetConnectionString(AppDbContext.DatabaseReferenceName());
        if (string.IsNullOrWhiteSpace(connStr))
        {
            _appRuntime.Logger!.Error("Database Connection:  No Connection String found in Configuration");
            throw new ApplicationException("No database connection string specified");
        }

        _appRuntime.Logger!.Warning("Database Connection:  Using value found in Configuration: " + connStr);
        builder.Services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseSqlServer(connStr)
                   .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))

                   // IF Debug then log all SQL to Console
                   .EnableDetailedErrors();
        });
    }



    /// <summary>
    /// This is the final chance for you to put customizations into the BuildApp process.  This is called at the very end of the BuildApp process.
    /// This is called after the Endpoints are mapped.  So, you can add custom endpoints here.
    /// </summary>
    /// <param name="app"></param>
    public override void BuildApp_Final(WebApplication app) { base.BuildApp_Final(app); }

    /// <summary>
    ///     After the application has started, this is called to allow for any post start logic
    /// </summary>
    public override void PostStartLogic(WebApplication app)
    {
        base.PostStartLogic(app);

        IServiceScope scope = app.Services.CreateScope();

        // Instantiate the GlobalAppData.  It's a singleton so this is only time this done.
        GlobalAppData gData = app.Services.GetRequiredService<GlobalAppData>();

        // Grab the Startup Engine and run it
        StartupEngine startupEngine = scope.ServiceProvider.GetService<StartupEngine>() ?? throw new ApplicationException("Startup Engine Not Found");
        Result result = startupEngine.Initialize();


        if (result.IsFailed)
        {
            _appRuntime.Logger!.Error("Startup Engine Failed: " + result.Errors.FirstOrDefault()?.Message);
            throw new ApplicationException("Startup Engine Failed: " + result.Errors.FirstOrDefault()?.Message);
        }
        else
        {
            _appRuntime.Logger!.Information("Startup Engine Succeeded");
        }


    }



    protected void SetupOData(WebApplicationBuilder builder)
    {
        var modelBuilder = new ODataConventionModelBuilder();


        modelBuilder.EntitySet<SampleLong>("SampleLongs");
        

        IEdmModel model = modelBuilder.GetEdmModel();
        model.MarkAsImmutable();

        //builder.Services.AddSingleton<IStreamBasedJsonWriterFactory>
        builder.Services.AddControllers().AddOData(options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
                                                    "odata",
                                                    model));
    }

}