using Bogus;
using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Database;
using SlugEnt.FluentResults;
using SlugEnt.HR.NextGen.Entities.Models;

namespace SlugEnt.BWA.Common;

/// <summary>
/// Class Keeps track of each significant Database Version / Update required.
/// </summary>
internal class VersionNumbers
{
    public const int SampleInt  = 1;
    public const int SampleGuid = 2;
    public const int SampleUlid = 3;
    public const int SampleStr  = 4;
    public const int SampleLong = 5;
}

/// <summary>
/// Class that performs initial and major database change actions
/// </summary>
/// <remarks>You can remove the Faker objects in your own project as you probably do not need Fake Data!</remarks>
public class AutomaticDataSeed
{
    private AppDbContext _db;
    private ILogger<AutomaticDataSeed> _logger;

    private Faker faker = new Faker();

    private Faker<SampleInt>? _sampleIntFaker = null;
    private Faker<SampleGuid>? _sampleGuidFaker = null;
    private Faker<SampleUlid>? _sampleUlidFaker = null;
    private Faker<SampleString>? _sampleStrFaker = null;
    private Faker<SampleLong>? _sampleLongFaker = null;




    /// <summary>
    /// Constructs the Automatic Data Seed class with the database context.
    /// </summary>
    /// <param name="db"></param>
    public AutomaticDataSeed(AppDbContext db, ILogger<AutomaticDataSeed> logger)
    {
        _db = db;
        _logger = logger;
    }


    /// <summary>
    /// Performs the Data Seed / Data Migration for the application.
    /// </summary>
    /// <returns></returns>
    public Result SeedData()
    {
        int newVersionCode = GlobalAppData.AppSettings.CurrentDbCodeVersion;

        try
        {
            Result seedResult = Result.Ok();

            // Update the Sample Ints
            seedResult = Result.Merge(seedResult, ImplementResult(SeedData_SampleInts, ref newVersionCode));
            if (seedResult.IsFailed)
                return seedResult;


            // Update the Sample Guids
            seedResult = Result.Merge(seedResult, ImplementResult(SeedData_SampleGuids, ref newVersionCode));
            if (seedResult.IsFailed)
                return seedResult;


            // Update the Sample Ulids
            seedResult = Result.Merge(seedResult, ImplementResult(SeedData_SampleUlids, ref newVersionCode));
            if (seedResult.IsFailed)
                return seedResult;

            // Update the Sample Strings
            seedResult = Result.Merge(seedResult, ImplementResult(SeedData_SampleStrs, ref newVersionCode));
            if (seedResult.IsFailed)
                return seedResult;

            // Update the Sample Longs
            seedResult = Result.Merge(seedResult, ImplementResult(SeedData_SampleLongs, ref newVersionCode));
            if (seedResult.IsFailed)
                return seedResult;

            return seedResult;
        }
        catch (Exception e)
        {
            // Log the Error.
            _logger.LogError(e, "Unplanned exception in {Class}:{Method} {Message}", nameof(AutomaticDataSeed), nameof(SeedData), e.Message);
            return Result.Fail(new ExceptionalError($"Unplanned exception in {nameof(AutomaticDataSeed)}:{nameof(SeedData)}: Error: {e.Message}", e));
        }
        finally
        {
            // Ensure we save any updates we made to the tables and save the current Version Code Level
            // Note:  We must clear the change tracker as it is possible a failure has left changes in the change tracker that will cause us to fail as well.  We don't care
            // about the changes as they failed in a transaction in one of the above methods.
            if (newVersionCode > GlobalAppData.AppSettings.CurrentDbCodeVersion)
            {
                _db.ChangeTracker.Clear();

                // Update the version code in the AppSettings
                GlobalAppData.AppSettings.CurrentDbCodeVersion = newVersionCode;
                _db.Update(GlobalAppData.AppSettings);
                _db.SaveChanges();
            }
        }
    }



    /// <summary>
    /// Runs a Seed Method and increments the version code if successful.
    /// </summary>
    /// <param name="seedAction"></param>
    /// <param name="version"></param>
    /// <returns></returns>
    private Result ImplementResult(Func<int, Result<int>> seedAction,
                                   ref int version)
    {
        bool success = false;
        Result<uint> transactionResult = _db.DatabaseTransactionManager.StartTransaction();
        if (transactionResult.IsFailed)
            return transactionResult.ToResult();

        try
        {
            Result<int> result = seedAction(version);
            if (result.IsFailed)
            {
                return Result.Fail(result.Errors);
            }

            if (result.Value > version)
            {
                // If the version is greater than the current version, we update it.
                version = result.Value;
            }
            success = true;
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError("Error while attempting to initalize or update the system.  Error in method: " + nameof(seedAction), e));
        }
        finally
        {
            // Commit or Rollback the transaction based upon success
            _db.DatabaseTransactionManager.CloseTransaction(transactionResult.Value, success);
        }

    }



    /// <summary>
    /// Seed the SampleInt table
    /// </summary>
    /// <param name="versionCheck"></param>
    /// <returns></returns>
    private Result<int> SeedData_SampleInts(int versionCheck)
    {
        if (versionCheck >= VersionNumbers.SampleInt)
            return Result.Ok(VersionNumbers.SampleInt);

        // Create the Generator
        if (_sampleIntFaker == null)
            IntGenerator();


        // Perform some action to get it up to current version
        IEnumerable<SampleInt> sampleInts = new List<SampleInt>();
        sampleInts = _sampleIntFaker.Generate(25);
        _db.SampleInt.AddRange(sampleInts);
        _db.SaveChanges();
        return Result.Ok(VersionNumbers.SampleInt);
    }




    /// <summary>
    /// Seed the SampleGuid table
    /// </summary>
    /// <param name="versionCheck"></param>
    /// <returns></returns>
    private Result<int> SeedData_SampleGuids(int versionCheck)
    {
        if (versionCheck >= VersionNumbers.SampleGuid)
            return Result.Ok(VersionNumbers.SampleGuid);

        // Create the Generator
        if (_sampleGuidFaker == null)
            GuidGenerator();


        // Perform some action to get it up to version 1
        IEnumerable<SampleGuid> sampleGuids = new List<SampleGuid>();
        sampleGuids = _sampleGuidFaker.Generate(32);
        _db.SampleGuids.AddRange(sampleGuids);
        _db.SaveChanges();
        return Result.Ok(VersionNumbers.SampleGuid);
    }




    /// <summary>
    /// Seed the SampleUlid table
    /// </summary>
    /// <param name="versionCheck"></param>
    /// <returns></returns>
    private Result<int> SeedData_SampleUlids(int versionCheck)
    {
        if (versionCheck >= VersionNumbers.SampleUlid)
            return Result.Ok(VersionNumbers.SampleUlid);

        // Create the Generator
        if (_sampleUlidFaker == null)
            UlidGenerator();


        // Perform some action to get it up to version 1
        IEnumerable<SampleUlid> sampleUlids = new List<SampleUlid>();
        sampleUlids = _sampleUlidFaker.Generate(46);
        _db.SampleUlids.AddRange(sampleUlids);
        _db.SaveChanges();
        return Result.Ok(VersionNumbers.SampleUlid);
    }




    /// <summary>
    /// Seed the SampleString table
    /// </summary>
    /// <param name="versionCheck"></param>
    /// <returns></returns>
    private Result<int> SeedData_SampleStrs(int versionCheck)
    {
        if (versionCheck >= VersionNumbers.SampleStr)
            return Result.Ok(VersionNumbers.SampleStr);

        // Create the Generator
        if (_sampleStrFaker == null)
            StringGenerator();


        // Perform some action to get it up to version 1
        IEnumerable<SampleString> sampleStrings = new List<SampleString>();
        sampleStrings = _sampleStrFaker.Generate(46);
        _db.SampleString.AddRange(sampleStrings);
        _db.SaveChanges();
        return Result.Ok(VersionNumbers.SampleStr);
    }




    /// <summary>
    /// Seed the SampleLongs table
    /// </summary>
    /// <param name="versionCheck"></param>
    /// <returns></returns>
    private Result<int> SeedData_SampleLongs(int versionCheck)
    {
        if (versionCheck >= VersionNumbers.SampleLong)
            return Result.Ok(VersionNumbers.SampleLong);

        // Create the Generator
        if (_sampleLongFaker == null)
            LongGenerator();


        // Perform some action to get it up to the correct version
        IEnumerable<SampleLong> sampleLongs = new List<SampleLong>();
        sampleLongs = _sampleLongFaker.Generate(446);
        _db.SampleLongs.AddRange(sampleLongs);
        _db.SaveChanges();
        return Result.Ok(VersionNumbers.SampleLong);
    }


    /// <summary>
    /// Generates Random SampleInt's
    /// </summary>
    /// <returns></returns>
    public Faker<SampleInt> IntGenerator()
    {
        if (_sampleIntFaker == null)
        {
            _sampleIntFaker = new Faker<SampleInt>()
                              .UseSeed(1969)
                              .RuleFor(p => p.Name, f => f.Name.FirstName())
                              .RuleFor(p => p.IsActive, f => f.Random.Bool(0.75f))
                              ;
        }

        return _sampleIntFaker;
    }



    /// <summary>
    /// Generates Random SampleGuid's
    /// </summary>
    /// <returns></returns>
    public Faker<SampleGuid> GuidGenerator()
    {
        if (_sampleGuidFaker == null)
        {
            _sampleGuidFaker = new Faker<SampleGuid>()
                              .UseSeed(1969)
                              .RuleFor(p => p.SampleName, f => f.Name.LastName())
                              .RuleFor(p => p.SampleNumber, f => f.Random.Int(76, 16000))
                              .RuleFor(p => p.IsActive, f => f.Random.Bool(0.75f))
                ;
        }

        return _sampleGuidFaker;
    }





    /// <summary>
    /// Generates Random SampleUlid's
    /// </summary>
    /// <returns></returns>
    public Faker<SampleUlid> UlidGenerator()
    {
        if (_sampleUlidFaker == null)
        {
            _sampleUlidFaker = new Faker<SampleUlid>()
                               .UseSeed(1969)
                               .RuleFor(p => p.SampleName, f => f.Name.LastName())
                               .RuleFor(p => p.SampleNumber, f => f.Random.Int(76, 16000))
                               .RuleFor(p => p.IsActive, f => f.Random.Bool(0.75f))
                ;
        }

        return _sampleUlidFaker;
    }




    /// <summary>
    /// Generates Random SampleStrings
    /// </summary>
    /// <returns></returns>
    public Faker<SampleString> StringGenerator()
    {
        if (_sampleStrFaker == null)
        {
            _sampleStrFaker = new Faker<SampleString>()
                               .UseSeed(1969)
                               .RuleFor(p => p.Id, f => f.Random.AlphaNumeric(12))
                               .RuleFor(p => p.SampleName, f => f.Name.LastName())
                               .RuleFor(p => p.SampleNumber, f => f.Random.Int(76, 16000))
                               .RuleFor(p => p.IsActive, f => f.Random.Bool(0.75f))
                ;
        }

        return _sampleStrFaker;
    }


    /// <summary>
    /// Generates Random SampleLong's
    /// </summary>
    /// <returns></returns>
    public Faker<SampleLong> LongGenerator()
    {
        if (_sampleLongFaker == null)
        {
            _sampleLongFaker = new Faker<SampleLong>()
                              .UseSeed(1969)
                              .RuleFor(p => p.SampleName, f => f.Company.CompanyName())
                              .RuleFor(p => p.IsActive, f => f.Random.Bool(0.75f))
                              .RuleFor(p => p.SampleNumber, f => f.Random.Int(int.MaxValue - 50000, int.MaxValue - 10))
                ;
        }

        return _sampleLongFaker;
    }

}