using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Common;
using SlugEnt.BWA.Database;
using SlugEnt.FluentResults;
using SlugEnt.IS.AppRun;

namespace ConsoleApp;

public partial class MainMenu
{
    private readonly AppDbContext      Db;
    private readonly IServiceProvider  _serviceProvider;
    private readonly ILogger<MainMenu> _logger;
    private          Faker             Faker = new Faker();

    public MainMenu(AppDbContext appDb, ILogger<MainMenu> logger,
                                                         IServiceProvider serviceProvider)
    {
        Db               = appDb;
        _serviceProvider = serviceProvider;
        _logger          = logger;
    }



    public AppRuntime AppRuntime { get; set; }


    /// <summary>
    /// Prepares the Main Menu for use
    /// </summary>
    /// <param name="appRuntime"></param>
    public void Initialize(AppRuntime appRuntime)
    {
        AppRuntime = appRuntime;
        AppRuntime.Logger!.Information("Main Menu Initialized");


        StartupEngine startupEngine = _serviceProvider.GetRequiredService<StartupEngine>();
        Result        result        = startupEngine.Initialize();
        if (!result.IsSuccess)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Error Encountered during StartupEngine Initialization.  This is  critical service and the app will likely not work correctly.   Error: " + result.ToStringWithLineFeeds());
            Console.WriteLine();
        }

    }


    /// <summary>
    /// Displays the Main Menu
    /// </summary>
    /// <returns></returns>
    internal async Task Display()
    {
        Console.WriteLine(AppRuntime.AppFullName);
        Console.WriteLine();
        Console.WriteLine("  ====================================================================================================================");
        Console.WriteLine("Press:  ");
        Console.WriteLine(" ( A ) Do Something            [ {0} ]", "current value");
//        Console.WriteLine("    Base Folder:              [ {0} ]", BaseFolder);
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(" ( Z ) To Seed the database");
        Console.WriteLine(" ( X ) To Exit the Application");
    }


    /// <summary>
    /// Processes the User Input from the Main Menu
    /// </summary>
    /// <returns></returns>
    internal async Task<bool> MainMenuUserInput()
    {
        try
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                switch (keyInfo.Key)
                {
                    case ConsoleKey.D4:
                        break;

                    case ConsoleKey.D5:
                        break;
                    case ConsoleKey.D6:
                        break;
                    case ConsoleKey.D7:
                        break;

                    case ConsoleKey.D9:
                        break;


                    case ConsoleKey.A:
                        break;

                    case ConsoleKey.H:
                        break;


                    case ConsoleKey.B:
                        break;

                    case ConsoleKey.C:
                        break;

                    case ConsoleKey.D:
                        break;

                    case ConsoleKey.S:
                        DisplayStats();
                        break;

                    // Upload / Save Document
                    case ConsoleKey.U:
/*                        Console.WriteLine("Enter complete path where file you wish to upload is at.  Empty value will use the Source Folder as the source directory.");
                        string folder = Console.ReadLine();
                        if (folder == string.Empty)
                            folder = SourceFolder;

                        Console.WriteLine("Enter Filename to upload");
                        string fileName = Console.ReadLine();
*/
                        break;


                    case ConsoleKey.Z:
                        Console.WriteLine("Seeding the Database...");
                        await SeedDataAsync();
                        break;

                    case ConsoleKey.X: return false;
                }
            }

            // Empty Key Queue
            while (Console.KeyAvailable)
                Console.ReadKey();
        }
        catch (Exception ex)
        {
            AppRuntime.Logger.Error(ex.Message);
        }

        Display();
        return true;
   }



    internal void DisplayStats()
    {
        Console.WriteLine("Overall Statistics:    *****");
//        Console.WriteLine("  Exact Match Sessions: {0}", StatSessionsExactMatch);
        
    }


    /// <summary>
    /// Startup of Main Menu
    /// </summary>
    /// <returns></returns>
    internal async Task Start()
    {
        bool keepProcessing = true;
//        Console.WriteLine("Using an API Key of: {0}", ApiKey);
        Display();

        while (keepProcessing)
        {
            if (Console.KeyAvailable)
            {
                Display();
                keepProcessing = await MainMenuUserInput();
            }
            else
            {
                Thread.Sleep(1000);
            }
        }
    }
}