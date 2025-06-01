// This should normally be defined.  This allows unit tests to run without colliding into each other as everything happens in 
// a transaction that is never committed.  
//  - Undef it for specialized cases where you need to see what happened with a particular unit test case by looking at the 
//    database.
#define ENABLE_TRANSACTIONS

//using AutoMapper;
using Bogus;
using BWATemplate.Test;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SlugEnt.BWA.Database;

using ILogger = Serilog.ILogger;

namespace BWATemplate.Test;

/// <summary>
///     This sets up a Test Data class of common items that are needed for each test scenario
///     * A Mock File System
///     * A Database Connection to a Unit Test Database
///     * The DocumentServerEngine
///     * Some Support Methods that can be used during testing
/// </summary>
public class SupportMethods
{
    private static Faker   _faker;
    private static ILogger serilog;

    private readonly SupportMethodsConfiguration _smConfiguration;


    /// <summary>
    ///     The Preferred Constructor...
    /// </summary>
    /// <param name="smConfiguration"></param>
    public SupportMethods(SupportMethodsConfiguration smConfiguration)
    {
        _smConfiguration = smConfiguration;
        SetupStage1();
    }


    /// <summary>
    ///     Constructore.  This is the simple constructor.  Can set Database and Transactions.  For more complex you must use
    ///     the SupportMethodsConfiguration constructor
    ///     <param name="useTransactions">
    ///         If using the database, this determines if the system should use transactions or not.  Some of the tests use
    ///         code that cannot have a nested transaction so it needs to be false.
    ///     </param>
    ///     <param name="useDatabase">If true, the database will be setup and utilized.  If false, it will not.</param>
    /// </summary>
    public SupportMethods(bool useDatabase = true,
                          bool useTransactions = true)
    {
        _smConfiguration = new SupportMethodsConfiguration
        {
            UseDatabase     = useDatabase,
            UseTransactions = useTransactions,
            UseAutoMapper   = false
        };
        SetupStage1();
    }




    /// <summary>
    ///     Returns the DB Context
    /// </summary>
    public AppDbContext DB { get; private set; }


    /// <summary>
    ///     The Database connection string
    /// </summary>
    private static string DBConnectionString { get; set; } = "";



    /// <summary>
    ///     Returns the faker instance
    /// </summary>
    public Faker Faker => _faker;


    /// <summary>
    ///     Returns the ID Lookup Dictionary
    /// </summary>
    public Dictionary<string, object> IdLookupDictionary => DatabaseSetup_Test.IdLookupDictionary;


    /// <summary>
    ///     This is the Initialize Task.  Must be called to finalize setup of key objects
    /// </summary>
    public Task Initialize { get; private set; }



    /// <summary>
    ///     Returns True if all initialization is completed.
    /// </summary>
    public bool IsInitialized { get; private set; }


    /// <summary>
    ///     Returns the Actual Serilog logger
    /// </summary>
    public ILogger Logger => serilog;


    /// <summary>
    ///     The Automapper
    /// </summary>

    //public IMapper? Mapper { get; private set; } = null;

    public ServiceProvider? ServiceProvider { get; private set; }


    /// <summary>
    ///     Provides an IServiceCollection
    /// </summary>
    public IServiceCollection? Services { get; private set; }


    public string PrintArg(string name,
                           object value)
    {
        return " " + name + " [ " + value + " ] ";
    }


    /// <summary>
    ///     Reads the Configuration for Database and other settings from the appsettings.json file
    ///     This file MUST at a minimum contain the DB Connection string
    /// </summary>
    protected void ReadConfiguration()
    {
        // We have already read the connection string in a prior start
        if (DBConnectionString != string.Empty)
        {
            return;
        }

        string             settingsFile = @"D:\a_dev\AppSettings\SlugEnt.BWA.Template_UT_AppSettingsSensitive.json";
        IConfigurationRoot config       = new ConfigurationBuilder().AddJsonFile(settingsFile).Build();
        DBConnectionString = config.GetConnectionString("HRNextGen25_UT");

    }


    /// <summary>
    ///     Setsup the Services Collection if it has not been done so already
    /// </summary>
    private void SetupServices()
    {
        if (Services == null)
        {
            Services = new ServiceCollection();
        }
    }



    /// <summary>
    ///     Performs initial setup of the SupportMethods object.  Then starts the SetupStage2Async
    /// </summary>
    private void SetupStage1()
    {
        serilog = new LoggerConfiguration().WriteTo.Console().Enrich.FromLogContext().CreateLogger();

        if (_faker == null)
        {
            _faker = new Faker();
        }

        // Read Configuration from the Secured Sensitive File.
        ReadConfiguration();

        SetupServices();

/*        if (_smConfiguration.UseAutoMapper)
        {
            Services.AddAutoMapper(typeof(PrimaryDTOs));
        }
*/
        ServiceProvider = Services.BuildServiceProvider();
/*
        if (_smConfiguration.UseAutoMapper)
        {
            Mapper = ServiceProvider.GetRequiredService<IMapper>();
        }
*/

        Initialize = SetupStage2Async();
    }


    /// <summary>
    ///     Performs Constructor level async operations.
    ///     caller must call await Initialize to ensure these operations have completed.
    /// </summary>
    /// <returns></returns>
    private async Task SetupStage2Async()
    {
        // Create a Context specific to this object.  Everything will be run in an uncommitted transaction
        if (_smConfiguration.UseDatabase)
        {
            DatabaseSetup_Test.Setup(DBConnectionString);
            DB = DatabaseSetup_Test.CreateContext();
            string tmsg = "";


            ConsoleColor color = ConsoleColor.Green;
            if (_smConfiguration.UseTransactions)
            {
                DB.Database.BeginTransaction();
                tmsg =
                    "This means nothing is committed to the database from this unit test.  In some test scenarios this can cause unexpected results.  It can be turned off in those cases.";
                color = ConsoleColor.DarkRed;
            }

            Console.ForegroundColor = color;
            Console.WriteLine("*************$$$$$$$$$$$$$$$$$$$$   Using Transactions: {0}   $$$$$$$$$$$$$$$$$$$$*************", _smConfiguration.UseTransactions.ToString());
            Console.WriteLine(tmsg);
            Console.ForegroundColor = ConsoleColor.White;

            // TODO
            //  TradingEngine = new TradingEngine(DB);
        }

        IsInitialized = true;
    }
}