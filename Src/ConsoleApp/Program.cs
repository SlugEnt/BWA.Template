using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using SlugEnt.IS.AppRun;
using ILogger = Serilog.ILogger;

namespace ConsoleApp;


/// <summary>
/// You should not NEED TO MAKE ANY CHANGES to this FILE!
/// </summary>
public class Program
{
    private static AppRuntime _appRun;
    private static ConsoleCustom _consoleCustom;
    private static ILogger _logger;


    public static async Task<int> Main(bool developmentMode = false,
                                       bool debugMode = false)
    {
        try

        {
            // A. Initialize AppRun Object
            AppRuntimeInfo appRuntimeInfo = new()
            {
                DevelopmentMode = developmentMode,
                DebugMode = debugMode,
                AssemblyQualifiedName = typeof(Program).AssemblyQualifiedName!
            };
            _appRun = new AppRuntime("SlugEnt.HR.Nextgen25.ConsoleApp", appRuntimeInfo);

            // IF you want to use a Sensitive File for storing secrets, put the path and name here.  Or Add Enviuronment varables, etc
            _appRun.AppSettings.SensitiveName = @"D:\a_dev\AppSettings\Daborg69.HrNextGen25_AppSettingsSensitive.json";

            
            // These MUST BE in this order!
            _consoleCustom = new ConsoleCustom(_appRun);
            _appRun.Build();
        }


        catch (Exception ex)
        {
            string msg = "An unexpected error occured during initial application startup.  There is likely no log of this anywhere else.";
            if (_appRun.Logger == null)
            {
                Console.WriteLine(msg);
                Console.WriteLine(ex.ToString());
                Console.WriteLine(ex.Message);

                return 1969;
            }

            _appRun.Logger.Fatal(ex, msg);
            Log.CloseAndFlush();
            return 1;
        }


        // Build App
        try
        {
            using IHost host = Host.CreateDefaultBuilder()
                                   .ConfigureAppConfiguration(config =>
                                   {
                                       config.AddConfiguration(_appRun.AppConfiguration);
                                   })
                                      .UseSerilog((context, services, configuration) =>
                                      {
                                          configuration.ReadFrom.Configuration(context.Configuration)
                                                       .ReadFrom.Services(services);
                                      })
                                     .ConfigureServices((context, services) =>
                                     {
                                         // Add the custom services
                                         _consoleCustom.ConfigureDB(services);
                                         _consoleCustom.AddServices(services);
                                     })
                                   .Build();

            // B. Run the Main App. Do NOT AWAIT call
            host.StartAsync();

            // Do any post start up logic
            _consoleCustom.AfterStarting(host);

            // Call Main Menu and await return
            MainMenu mainMenu = host.Services.GetRequiredService<MainMenu>();
            mainMenu.Initialize(_appRun);
            await mainMenu.Start();

            host.WaitForShutdownAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fatal Error Occurred:  Error--> " + ex.Message);
            _appRun.Logger.Error(ex, "An unexpected error occured during the main application run. - " + ex.Message);
            Log.CloseAndFlush();
            return 1;
        }
    }
}