using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities.EntityEnums;
using SlugEnt.BWA.Entities.Models;
using SlugEnt.FluentResults;

namespace SlugEnt.BWA.Common;

/// <summary>
///     The Startup Engine is called upon initial program running.
///     It is used to check that all required information in the database exists.
///     It really only does anything on new databases or updates that requires new information.
/// </summary>
/// <remarks>
///     Constructor.  Checks for the System User and creates it if it does not exist.
/// </remarks>
/// <exception cref="ApplicationException"></exception>
public class StartupEngine
{
    // TODO replace this at some point....
    public const string SU_PRIMARY = "SU_Primary@sys.tech";


    private readonly AppDbContext _db;
    private readonly ILogger<StartupEngine> _logger;
    private readonly GlobalAppData _globalAppData;
    private readonly AutomaticDataSeed _automaticDataSeed;


    /// <summary>
    /// Constructs the Startup Engine
    /// </summary>
    /// <param name="db"></param>
    /// <param name="logger"></param>
    /// <param name="globalAppData"></param>
    public StartupEngine(AppDbContext db,
                         ILogger<StartupEngine> logger,
                         AutomaticDataSeed automaticDataSeed)
    {
        _db = db;
        _logger = logger;
        _automaticDataSeed = automaticDataSeed;
    }


    /// <summary>
    /// Performs the Startup Engines Tasks
    /// 1. Creates a company called Internal_Use.
    /// 2. Creates a subdivision called Internal_Use.
    /// 3. Creates a system user
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ApplicationException"></exception>
    public Result Initialize()
    {
        Result resultInit = new Result();
        try
        {
            if (_db == null)
            {
                resultInit.AddError(new Error("StartupEngine: Initialization:  No Database provided.  A Database object is required."));
                _logger.LogError("StartupEngine: Initialization: No Database provided. A Database object is required.");
                return resultInit;
            }

            // Setup System Person
            Result result = InitializeSystemUser();
            if (result.IsFailed)
                Result.Merge(resultInit, result);


            // Setup the AppSettings Table with initial startup settings values (AppSettings Table)
            result = InitializeSettingsObj();
            if (result.IsFailed)
                Result.Merge(resultInit, result);

            result = InitializeApplicationData();
            if (result.IsFailed)
                Result.Merge(resultInit, result);

            if (resultInit.IsFailed)
                return resultInit;
            return Result.Ok();
        }
        catch (Exception e)
        {
            _logger.LogError("StartupEngine: Initialize: Error --> " + e.Message);
            return Result.Fail(new ExceptionalError("StartupEngine: Initialize: Error --> " + e.Message, e));
        }
    }

    /// <summary>
    /// Reads the Settings (Setup table) from the database.  If not found, it creates default entries.  
    /// </summary>
    /// <returns></returns>
    internal Result InitializeSettingsObj()
    {
        const string settingName = "Primary";

        try
        {
            if (GlobalAppData.AppSettings == null)
            {
                // Read the Settings object from the database
                if (_db.AppSettings.Count() > 0)
                    GlobalAppData.AppSettings = _db.AppSettings.FirstOrDefault(s => s.Name == settingName);
                else
                {
                    // Insert new first value
                    AppSetting appsetting = new()
                    {
                        CurrentDbCodeVersion = 0,
                        CreatedById          = GlobalAppData.SystemUser.Id,
                        Name                 = settingName,
                    };
                    _db.Add(appsetting);
                    _db.SaveChanges();
                    GlobalAppData.AppSettings = appsetting;
                }

                // If still null then it does not exist, so create it.
                if (GlobalAppData.AppSettings == null)
                {
                    // If it does not exist, create a new one.
                    GlobalAppData.AppSettings = new AppSetting()
                    {
                        Name                 = settingName,
                        CurrentDbCodeVersion = 0, // Set to very first initial value
                        IsActive             = true,
                    };

                    _db.AppSettings.Add(GlobalAppData.AppSettings);
                    _db.SaveChanges();
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogError("StartupEngine: InitializeSettingsObj:  Error --> " + e.Message);
            return Result.Fail(new ExceptionalError("StartupEngine: InitializeSettingsObj: Error --> " + e.Message, e));
        }
        return Result.Ok();
    }



    /// <summary>
    /// PErforms Data Seeding and Migration updates to the database.
    /// </summary>
    /// <returns></returns>
    internal Result InitializeApplicationData()
    {
        try
        {
            // Run thru Data Seed / Migration updates
            return LoadInitialSeedData();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError("StartupEngine: InitializeApplicationData: Error --> " + e.Message, e));
        }
    }



    /// <summary>
    /// Checks if the System Person has been initialized, and if not, initializes it.
    /// </summary>
    /// <returns></returns>
    internal Result InitializeSystemUser()
    {
        try
        {
            User? systemUser = null;

            // See if Primary System User has already been set.
            if (GlobalAppData.SystemUser == null)
            {
                // Read the System User
                systemUser = _db.Users.Where(p => p.Type == EnumUserTypes.System && p.Email == SU_PRIMARY).SingleOrDefault();
                if (systemUser != null)
                {
                    GlobalAppData.SystemUser = systemUser;
                    return Result.Ok();
                }


                // Make sure there is a system user
                systemUser = new User
                {
                    FirstName     = "Primary",
                    LastName      = "SystemUser",
                    PhoneExternal = "000-000-0000",
                    Email         = SU_PRIMARY,
                    IsActive      = true,
                    UPN           = null,
                    Type          = EnumUserTypes.System,
                };
                _db.Users.Add(systemUser);
                _db.SaveChanges();
                GlobalAppData.SystemUser = systemUser;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("StartupEngine: InitializeSystemUser: Error --> " + e.Message);
            return Result.Fail("Failed to initialize the System User");
        }

        return Result.Ok();
    }


    internal Result LoadInitialSeedData()
    {
        try
        {
            
            Result seedResult = _automaticDataSeed.SeedData();
            return seedResult;
        }
        catch (Exception exception)
        {
            return Result.Fail(new Error("Failure during Seeding of Tables.").CausedBy(exception));
        }
    }
}