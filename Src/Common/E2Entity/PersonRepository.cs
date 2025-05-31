using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities.Models;
using SlugEnt.FluentResults;


namespace SlugEnt.BWA.Common;


/// <summary>
/// This manages the User Entity.  It is responsible for adding, updating, deleting and other operations on the User Entity to the database.
/// </summary>
public class UserRepository :  E2EntityRepository<User>
{
    /// <summary>
    /// Constructor for the User Engine
    /// </summary>
    /// <param name="db">The database context</param>
    /// <param name="logger">The logger</param>
    public UserRepository (AppDbContext db, ILogger<UserRepository> logger) : base(db, logger) { }

    /// <summary>
    /// Default Constructor for the User Engine
    /// </summary>
    public UserRepository () : base() { }

    /// <summary>
    /// Creates a new User object in the database.  Performs Validation on the person fields.
    /// Adds Actions to Add user to AD and other related  items.
    /// <para>This method respects Existing DB transactions.   IF the flag is true, then it will not start a new
    /// transaction, nor will it commit or rollback on success/failure.  It is upto the caller to handle the result and manage
    /// the transaction.  If this is false, then this method starts a transaction and commits or roll backs when done.</para>
    /// </summary>
    /// <param name="person">The User to add</param>
    /// <returns>Result.Success if saved successfully.  Result.Failed with message indicating reason for failure.</returns>
    public async Task<Result<User>> AddAsync(User person, int addedByUserId)
    {
        //IDbContextTransaction? transaction   = null;
        bool methodSuccess = false;
        uint transactionId = 0;

        try
        {
            // A.  Initial validation logic
            person.UPN = null;

            if (string.IsNullOrEmpty(person.FirstName))
                return Result.Fail(new Error("First Name was not provided. Is Required"));
            if (string.IsNullOrEmpty(person.LastName))
                return Result.Fail(new Error("Last Name was not provided. Is Required"));


            // B. Database logic
            Result<uint> result = _db.DatabaseTransactionManager.StartTransaction();
            if (result.IsFailed) return Result.Fail(result.ToStringWithLineFeeds());

            transactionId = result.Value;


            await _db.Users.AddAsync(person);
            await _db.SaveChangesAsync();

            methodSuccess = true;
            return Result.Ok(person);
        }
        catch (Exception ex)
        {
            string msg = "Failed to Add User to databasae.  Error: " + ex.Message;

            return Result.Fail(new ExceptionalError(msg, ex));
        }
        finally
        {
            _db.DatabaseTransactionManager.CloseTransaction(transactionId, methodSuccess);
        }
    }


}