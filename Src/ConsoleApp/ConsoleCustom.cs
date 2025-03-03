using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Database;
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
        services.AddScoped<MainMenu>();

        // Add your custom services here
    }
}
