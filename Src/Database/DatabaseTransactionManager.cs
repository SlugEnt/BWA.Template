using Microsoft.EntityFrameworkCore.Storage;
using SlugEnt.BWA.Database;
using SlugEnt.FluentResults;



/// <summary>
/// Assists with managing transactions across Entity Engines.
/// </summary>
public class DatabaseTransactionManager
{

    /// <summary>
    /// This provides Database Transactional services to Engines.  Specifically, it ensures that a given DB context only has 1 transaction at a time.  If a caller tries
    /// to start a transaction when one is already in place, then it just tells the caller everything is ok, but the original transaction persists.
    /// Conversely, on close, only the original caller that started the transaction can close it.  
    /// </summary>
    /// <param name="db"></param>
    public DatabaseTransactionManager(AppDbContext db) => DB = db;


    /// <summary>
    /// The Database Context transactions will take place in.
    /// </summary>
    private AppDbContext DB { get; set; }

    /// <summary>
    /// The Lock to ensure access to various properties we need.
    /// </summary>
    private readonly Lock transactionLock = new();

    /// <summary>
    /// The ID that identifies the current active transaction.  
    /// </summary>
    private uint TransactionId { get; set; } = 1;


    /// <summary>
    /// The actual Database transaction
    /// </summary>
    protected IDbContextTransaction? Transaction { get; set; } = null;


    /// <summary>
    /// Attempts to start a new transaction.  If the Database context is already in a transaction it will return Result.Ok with a TransactionId of Zero.
    /// If the database is not in a transaction then it will start a transaction and return a TransactionID that is non-zero.  You must supply this transaction ID on closing the transaction and it must match
    /// </summary>
    /// <returns></returns>
    public Result<uint> StartTransaction()
    {
        uint transactionId = 0;

        try
        {
            lock (transactionLock)
            {
                // Start a transaction as we are not in one yet.
                if (Transaction == null)
                {
                    // Start a transaction.
                    TransactionId++;
                    transactionId = TransactionId;

                    Transaction = DB.Database.BeginTransaction();
                }
            }

            return Result.Ok(transactionId);
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError("Failed to Start a Database Transaction:  Error: " + e.Message, e));
        }
    }


    /// <summary>
    /// Commits or Rool back a transaction based on the Transaction ID provided.
    /// <para></para>
    /// </summary>
    /// <param name="transactionId">If the transaction Id matches the Transaction ID that initiated the transaction will be committed or rolled back.   </param>
    /// <param name="commit">If the commit flag is true, the transaction is committed.  If false, it is rolled back.</param>
    /// <returns></returns>
    public Result CloseTransaction (uint transactionId, bool commit)
    {
        try
        {
            lock (transactionLock)
            {
                if (transactionId == 0)
                    return Result.Fail(
                        "You provided a transaction ID of 0.  This is not the owner of this transaction.");
                if (transactionId == TransactionId)
                {
                    if (Transaction == null)
                        // TODO Probably should log this
                        return Result.Ok();
                    if (commit)
                        Transaction!.Commit();
                    else
                        Transaction!.Rollback();
                    
                    Transaction!.Dispose();
                    Transaction = null;
                }
                else{ return Result.Fail("Invalid transaction ID provided.");}
            }
            return Result.Ok();
        }
        catch (Exception e)
        {
            return Result.Fail(new ExceptionalError("Failed to Close a Database Transaction:  Error: " + e.Message, e));
        }
    }
}