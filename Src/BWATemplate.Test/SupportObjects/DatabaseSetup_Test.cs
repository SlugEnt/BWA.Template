#define RESET_DATABASE



using Microsoft.EntityFrameworkCore;
using SlugEnt.BWA.Database;

namespace BWATemplate.Test;

//[TestFixture]
public static class DatabaseSetup_Test
{
    private static bool _databaseInitialized;

    private static readonly object _lock = new();



    /// <summary>
    ///     Constructor
    /// </summary>
    static DatabaseSetup_Test() { }


    /// <summary>
    ///     The actual Database Context for testing
    /// </summary>
    public static AppDbContext AppDbContext { get; private set; }


    /// <summary>
    ///     The connection string to the Unit Test Database
    /// </summary>
    private static string ConnectionString { get; set; }


    /// <summary>
    ///     Used to stored Id's for objects used during testing.
    /// </summary>
    public static Dictionary<string, object> IdLookupDictionary { get; } = new();


    /// <summary>
    ///     Create the Database Context for testing
    /// </summary>
    /// <returns></returns>
    public static AppDbContext CreateContext()
    {
        DbContextOptionsBuilder<AppDbContext> optionsBuilder = new();
        optionsBuilder.UseSqlServer(ConnectionString);
        optionsBuilder.EnableSensitiveDataLogging();

        AppDbContext = new AppDbContext(optionsBuilder.Options);
        return AppDbContext;
    }


    private static void SeedData(AppDbContext db)
    {
        /*
        // Add Accounts.  We will create 3 accounts
        Account accountB = new()
        {
            HeldAt = "Beta Schwab",
            Name   = "Beta Schwab Account"
        };
        Account accountA = new()
        {
            HeldAt = "Alpha Fidelity",
            Name   = "Alpha Fidelity Account"
        };
        Account accountC = new()
        {
            HeldAt = "Kappa ETrade",
            Name   = "Kappa ETrade Account"
        };
        db.AddRange(accountA, accountB, accountC);
        db.SaveChanges();

        IdLookupDictionary.Add("AcctA", accountA);
        IdLookupDictionary.Add("AcctB", accountB);
        IdLookupDictionary.Add("AcctC", accountC);


        // Add a few ticker symbols
        TickerSymbol symbolAAPL = new()
        {
            Symbol      = "AAPL",
            Description = "Apple Inc",
            AssetType   = EnumAssetType.Equity
        };
        TickerSymbol symbolJPM = new()
        {
            Symbol      = "JPM",
            Description = "JP Morgan Chase Bank",
            AssetType   = EnumAssetType.Equity
        };
        TickerSymbol symbolMU = new()
        {
            Symbol      = "MU",
            Description = "Micron Technology",
            AssetType   = EnumAssetType.Equity
        };
        db.AddRange(symbolAAPL, symbolJPM, symbolMU);
        db.SaveChanges();

        IdLookupDictionary.Add("symAAPL", symbolAAPL);
        IdLookupDictionary.Add("symJPM", symbolJPM);
        IdLookupDictionary.Add("symMU", symbolMU);
        */

        /*        // Add Applications
                Application appA = new()
                {
                    Name     = "App_A",
                    Token    = TestConstants.APPA_TOKEN,
                    IsActive = true,
                };
                Application appB = new()
                {
                    Name     = "App_B",
                    Token    = TestConstants.APPB_TOKEN,
                    IsActive = true,
                };
                db.Add(appA);
                db.Add(appB);
                db.SaveChanges();
                IdLookupDictionary.Add(appA.Name, appA);
                IdLookupDictionary.Add(appB.Name, appB);


                // Add a Root Object For each Application
                RootObject rootA = new()
                {
                    ApplicationId = appA.Id,
                    Name          = "Claim #",
                    Description   = "Claim Number of Auto Policy",
                    IsActive      = true,
                };

                RootObject rootB = new()
                {
                    ApplicationId = appB.Id,
                    Name          = "Movie #",
                    Description   = "The movie",
                    IsActive      = true,
                };

                RootObject rootC = new()
                {
                    ApplicationId = appB.Id,
                    Name          = "Actor",
                    Description   = "The actors professional screen writers Id",
                    IsActive      = true,
                };

                db.Add(rootA);
                db.Add(rootB);
                db.Add(rootC);
                db.SaveChanges();
                IdLookupDictionary.Add("Root_A", rootA);
                IdLookupDictionary.Add("Root_B", rootB);
                IdLookupDictionary.Add("Root_C", rootC);


                //  Add ServerHost
                //  - HostA must always be the localhost DNS name or unit tests will fail.
                string localHost = Dns.GetHostName();
                ServerHost hostA = new()
                {
                    IsActive = true,
                    NameDNS  = localHost,
                    FQDN     = localHost + ".abc.local",
                    Path     = @"C:\hostA",
                };
                db.Add(hostA);

                // HostB can be anything
                ServerHost hostB = new()
                {
                    IsActive = true,
                    NameDNS  = "otherHost",
                    FQDN     = "localhost",
                    Path     = @"t:\programmingTest\OtherHost\hostB",
                };
                db.Add(hostB);
                db.SaveChanges();
                IdLookupDictionary.Add("ServerHost_A", hostA);
                IdLookupDictionary.Add("ServerHost_B", hostB);


                // Add Storage Nodes
                StorageNode testA = new(TestConstants.STORAGE_NODE_TEST_A,
                                        "Test Node A - Primary",
                                        true,
                                        EnumStorageNodeLocation.HostedSMB,
                                        EnumStorageNodeSpeed.Hot,
                                        TestConstants.FOLDER_TEST_PRIMARY,
                                        true);
                testA.ServerHostId = hostA.Id;

                StorageNode testB = new(TestConstants.STORAGE_NODE_TEST_B,
                                        "Test Node B - Secondary",
                                        true,
                                        EnumStorageNodeLocation.HostedSMB,
                                        EnumStorageNodeSpeed.Hot,
                                        TestConstants.FOLDER_TEST_SECONDARY,
                                        true);
                testB.ServerHostId = hostA.Id;

                // True Production Nodes
                StorageNode prodX = new(TestConstants.STORAGE_NODE_PROD_X,
                                        "Production Node X - Primary",
                                        false,
                                        EnumStorageNodeLocation.HostedSMB,
                                        EnumStorageNodeSpeed.Hot,
                                        TestConstants.FOLDER_PROD_PRIMARY,
                                        true);
                prodX.ServerHostId = hostA.Id;

                StorageNode prodY = new(TestConstants.STORAGE_NODE_PROD_Y,
                                        "Production Node Y - Secondary",
                                        false,
                                        EnumStorageNodeLocation.HostedSMB,
                                        EnumStorageNodeSpeed.Hot,
                                        TestConstants.FOLDER_PROD_SECONDARY,
                                        true);
                prodY.ServerHostId = hostA.Id;

                StorageNode testC1 = new("testC1",
                                         "Test Node C1 - Main Host",
                                         false,
                                         EnumStorageNodeLocation.HostedSMB,
                                         EnumStorageNodeSpeed.Hot,
                                         TestConstants.FOLDER_TEST_PRIMARYC,
                                         true);
                testC1.ServerHostId = hostA.Id;

                StorageNode testC2 = new("testC2",
                                         "Test Node C2 - Other Host",
                                         false,
                                         EnumStorageNodeLocation.HostedSMB,
                                         EnumStorageNodeSpeed.Hot,
                                         TestConstants.FOLDER_TEST_SECONDARYC,
                                         true);
                testC2.ServerHostId = hostB.Id;
                db.AddRange(testA,
                            testB,
                            testC1,
                            prodX,
                            prodY,
                            testC2);
                db.SaveChanges();
                IdLookupDictionary.Add("StorageNode_A", testA);
                IdLookupDictionary.Add("StorageNode_B", testB);
                IdLookupDictionary.Add("StorageNode_C1", testC1);
                IdLookupDictionary.Add("StorageNode_C2", testC2);
                IdLookupDictionary.Add("StorageNode_X", prodX);
                IdLookupDictionary.Add("StorageNode_Y", prodY);

                // Add Document Types
                DocumentType docA = new()
                {
                    Name               = TestConstants.DOCTYPE_TEST_A,
                    Description        = "Test Doc Type A - WORM",
                    Application        = appA,
                    RootObject         = rootA,
                    StorageFolderName  = "WormA",
                    StorageMode        = EnumStorageMode.WriteOnceReadMany,
                    ActiveStorageNode1 = testA,
                    ActiveStorageNode2 = null,
                    IsActive           = true
                };
                DocumentType docB = new()
                {
                    Name               = TestConstants.DOCTYPE_TEST_B,
                    Description        = "Test Doc Type B - Temporary",
                    Application        = appA,
                    RootObject         = rootB,
                    StorageFolderName  = "TempB",
                    StorageMode        = EnumStorageMode.Temporary,
                    ActiveStorageNode1 = testA,
                    ActiveStorageNode2 = null,
                    IsActive           = true
                };
                DocumentType docC = new()
                {
                    Name               = TestConstants.DOCTYPE_TEST_C,
                    Description        = "Test Doc Type C - Editable",
                    Application        = appA,
                    RootObject         = rootC,
                    StorageFolderName  = "EditC",
                    StorageMode        = EnumStorageMode.Editable,
                    ActiveStorageNode1 = testA,
                    ActiveStorageNode2 = null,
                    IsActive           = true
                };
                DocumentType docX = new()
                {
                    Name               = TestConstants.DOCTYPE_PROD_X,
                    Description        = "Prod Doc Type X - WORM",
                    Application        = appB,
                    RootObject         = rootA,
                    StorageFolderName  = "PWormX",
                    StorageMode        = EnumStorageMode.WriteOnceReadMany,
                    ActiveStorageNode1 = prodX,
                    ActiveStorageNode2 = null,
                    IsActive           = true
                };
                DocumentType docY = new()
                {
                    Name               = TestConstants.DOCTYPE_PROD_Y,
                    Description        = "Prod Doc Type Y - Temporary",
                    Application        = appB,
                    RootObject         = rootB,
                    StorageFolderName  = "PTempY",
                    StorageMode        = EnumStorageMode.Temporary,
                    ActiveStorageNode1 = prodX,
                    ActiveStorageNode2 = null,
                    IsActive           = true
                };
                DocumentType docRA = new()
                {
                    Name               = TestConstants.DOCTYPE_REPLACE_A,
                    Description        = "Prod Doc Type RA - Replaceable",
                    Application        = appB,
                    RootObject         = rootC,
                    StorageFolderName  = "PReplaceeRA",
                    StorageMode        = EnumStorageMode.Replaceable,
                    ActiveStorageNode1 = prodX,
                    ActiveStorageNode2 = prodY,
                    IsActive           = true
                };

                db.AddRange(docA,
                            docB,
                            docC,
                            docX,
                            docY,
                            docRA);
                db.SaveChanges();


                IdLookupDictionary.Add("DocType_A", docA);
                IdLookupDictionary.Add("DocType_B", docB);
                IdLookupDictionary.Add("DocType_C", docC);
                IdLookupDictionary.Add("DocType_X", docX);
                IdLookupDictionary.Add("DocType_Y", docY);
                IdLookupDictionary.Add("DocType_RA", docRA);
        */
    }


    /// <summary>
    ///     Creates unit test DB, Seeds data
    /// </summary>
    public static void Setup(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentException("Connection String cannot be null or empty");
        }

        ConnectionString = connectionString;


#if RESET_DATABASE
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                AppDbContext context = CreateContext();
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                SeedData(context);

                _databaseInitialized = true;
            }
        }
#else
        CreateContext();
#endif
    }
}