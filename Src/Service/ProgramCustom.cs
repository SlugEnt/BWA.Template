using Microsoft.EntityFrameworkCore;
using Quartz;
using Quartz.Logging;
using Service.Jobs;
using SlugEnt.BWA.Common;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.Models;
using SlugEnt.FluentResults;
using SlugEnt.HR.NextGen.Common;
using SlugEnt.IS.AppRun;


namespace Service;

/// <summary>
///     This is the class you put all your applications unique requirements for starting the app.
/// </summary>
public class ProgramCustom : ProgramCustomBase
{
    IScheduler _jobScheduler = null!;


    /// <summary>
    ///     Constructor for the Program Custom
    /// </summary>
    /// <param name="appRuntime"></param>
    public ProgramCustom(AppRuntime appRuntime) : base(appRuntime)
    {

        LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

    }


    /// <summary>
    ///     This is where you add all services that your app needs.
    /// </summary>
    /// <param name="builder"></param>
    public override void AddServices(WebApplicationBuilder builder)
    {
        base.AddServices(builder);

        // QUARTZ:  Add Quartz Services
        QuartzJobSchedulingConfig.SetConfiguration(builder.Services, builder.Configuration);

        builder.Services.AddScoped<StartupEngine>();
        builder.Services.AddSingleton<GlobalAppData>();


        // Add Application Specific Objects
        builder.Services.AddScoped<IEntityRepositoryE2Int<User>, E2EntityRepositoryInt<User>>();
        builder.Services.AddTransient<StartupEngine>();
        builder.Services.AddTransient<AutomaticDataSeed>();

        // Add Scheduled Job Objects
        builder.Services.AddTransient<JobSample>();


    }


    /// <summary>
    /// Allows application to add to the Build App process.  This is called near the end of the BuildApp process, but before
    /// the endpoints / controllers section.  This is most likely where you want to add something.
    /// </summary>
    /// <param name="app"></param>
    public override void BuildApp_UsesMiddle(WebApplication app)
    {
        
    }


    /// <summary>
    ///     This is the final place for you to add anything you need as part of the BuildApp Process.
    ///     This is called after the basic endpoints are defined.
    /// </summary>
    /// <param name="app"></param>
    public override void BuildApp_Final(WebApplication app)
    {
        base.BuildApp_Final(app);
    }


    /// <summary>
    ///     Configures the database for the application
    /// </summary>
    /// <param name="builder"></param>
    public override void ConfigureDB(WebApplicationBuilder builder)
    {
        base.ConfigureDB(builder);
        string? connStr = builder.Configuration.GetConnectionString(AppDbContext.DatabaseReferenceName());
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
    ///     After the application has started, this is called to allow for any post start logic
    /// </summary>
    public override async Task PostStartLogic(WebApplication app)
    {
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

        // QUARTZ: Create the Job Scheduler
        ISchedulerFactory schedulerFactory = app.Services.GetRequiredService<ISchedulerFactory>();
        //StdSchedulerFactory schedulerFactory = new();
        _jobScheduler = schedulerFactory.GetScheduler().Result;
        await _jobScheduler.Start();


        // QUARTZ:  Add Some Jobs
        JobSample job2 = new();
        job2.DefineJob(_jobScheduler);
    }


    /// <summary>
    /// Code to be executed just after the WebApp has shutdown, but prior to program exit
    /// </summary>
    /// <returns></returns>
    public override async Task PostShutdownLogic()
    {
        await _jobScheduler.Shutdown();

    }
}





public class ConsoleLogProvider : ILogProvider
{
    public Logger GetLogger(string name)
    {
        return (level,
                func,
                exception,
                parameters) =>
        {
            if (level >= Quartz.Logging.LogLevel.Info && func != null)
            {
                Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
            }

            return true;
        };
    }


    public IDisposable OpenNestedContext(string message)
    {
        throw new NotImplementedException();
    }


    public IDisposable OpenMappedContext(string key,
                                         object value,
                                         bool destructure = false)
    {
        throw new NotImplementedException();
    }
}