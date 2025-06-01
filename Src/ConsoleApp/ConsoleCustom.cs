using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Common;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.Models;
using SlugEnt.HR.NextGen.Common;
using SlugEnt.IS.AppRun;

namespace ConsoleApp;
public  class ConsoleCustom : ConsoleCustomBase
{
    public ConsoleCustom(AppRuntime appRuntime) : base(appRuntime)
    {
        // We want to set the Sensitive AppSettings File to be used here.
        appRuntime.AppSettings.SensitiveName = @"D:\a_dev\AppSettings\BWA.Template_AppSettingsSensitive.json";
    }



    /// <summary>
    /// Configures the database for the application
    /// </summary>
    /// <param name="services"></param>
    /// <exception cref="ApplicationException"></exception>
    public override void ConfigureDB(IServiceCollection services)
    {
        string? connStr = _appRuntime.AppConfiguration.GetConnectionString(AppDbContext.DatabaseReferenceName());
        if (string.IsNullOrWhiteSpace(connStr))
        {
            _appRuntime.Logger!.Error("Database Connection:  No Connection String found in Configuration");
            throw new ApplicationException("No database connection string specified");
        }

        _appRuntime.Logger!.Warning("Database Connection:  Using value found in Configuration: " + connStr);
        services.AddDbContextPool<AppDbContext>(options =>
        {
            options.UseSqlServer(connStr)
                   .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))

                   // IF Debug then log all SQL to Console
                   .EnableDetailedErrors();
        });

    }



    /// <summary>
    /// Add Custom Services
    /// </summary>
    /// <param name="services"></param>
    public override void AddServices(IServiceCollection services)
    {
        base.AddServices(services);
        services.AddSingleton<GlobalAppData>();
        services.AddScoped<MainMenu>();
        services.AddScoped<IEntityRepositoryE2Int<User>, E2EntityRepositoryInt<User>>();

        services.AddTransient<StartupEngine>();
        services.AddTransient<AutomaticDataSeed>();

        // Add your custom services here
    }


    public override void AfterStarting(IHost host)
    {
        // This is called after the host has started.  This is a good place to put any code that needs to run immediately
        // after startup.  This is not the same as the Main method.  This is called after the host has started and all services have been registered.
        _appRuntime.Logger!.Information("ConsoleCustom: AfterStarting");

        GlobalAppData gData = host.Services.GetRequiredService<GlobalAppData>();
        GlobalAppData.ServiceProvider = host.Services;

        // Set the override email address for the entire app run.  This is a static field.
        // This is used for testing purposes only.  It will override all email addresses in the application.
        // This is not used in production code.
#if DEBUG

        // Example of using host....
        //SendInternalEmail emailer = host.Services.GetRequiredService<SendInternalEmail>();

        // TODO Replace with variable or argument or something...
        SendInternalEmail.OverrideEmailRecipient("scott.herrmann@domain.com");

#endif


    }

}
