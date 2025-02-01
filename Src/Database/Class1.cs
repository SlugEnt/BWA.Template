using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Reflection.Emit;
namespace Database;

public class AppDbContext : DbContext
{
    #region "Tables"


    //    public DbSet<Account> Accounts { get; set; }

    #endregion


    #region "BasicStuff"

    public AppDbContext() { Console.WriteLine("In AppDbContext Empty Constructor"); }


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // TODO Change to optional value or move back to 30 seconds.
        /*      Database.SetCommandTimeout(new TimeSpan(0,
                                                      0,
                                                      0,
                                                      5));
        */
        Console.WriteLine("In AppDb Options Constructor");
    }



    /// <summary>
    /// This 
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="ApplicationException"></exception>
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        Console.WriteLine("In OnConfiguring");

        //Console.WriteLine("Database:  Configuring DB Context Options");
        if (!options.IsConfigured)
        {
            throw new ApplicationException("AppDbContext is not yet configured.  It should be.");
        }
    }



    /// <summary>
    ///     Custom save logic, that stores Created and Modified information for those entities that have Auditable properties.
    /// </summary>
    /// <returns></returns>
    public override int SaveChanges()
    {
        CustomSaveChanges();
        return base.SaveChanges();
    }


    /// <summary>
    ///     Custom save logic, that stores Created and Modified information for those entities that have Auditable properties.
    /// </summary>
    /// <returns></returns>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        CustomSaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }


    /// <summary>
    ///     Implements custom logic to add Created and Modified audit entries upon save.  And prevent the updating of WORM
    ///     Fields (Fields that can only be written to on initial creation)
    /// </summary>
    private void CustomSaveChanges()
    {
        ChangeTracker tracker = ChangeTracker;

        foreach (EntityEntry entry in tracker.Entries())
        {
            if (entry.State == EntityState.Unchanged)
                continue;
            /*
            if (entry.Entity is AbstractBaseEntity)
            {
                IBaseEntity baseEntity = (IBaseEntity)entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        baseEntity.CreatedUTC = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        baseEntity.ModifiedUTC = DateTime.UtcNow;
                        break;
                }
            }
            */
        }
    }



    /// <summary>
    ///     Returns the value the Database is referenced by in AppSettings and other locations.
    /// </summary>
    /// <returns></returns>
    public static string DatabaseReferenceName() { return "SomeDB"; }

    #endregion


    /// <summary>
    ///     Override this method to further configure the model that was discovered by convention from the entity types
    ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting model may be cached
    ///     and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If a model is explicitly set on the options for this context (via <see cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />)
    ///         then this method will not be run. However, it will still run when creating a compiled model.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more information and
    ///         examples.
    ///     </para>
    /// </remarks>
    /// <param name="modelBuilder">
    ///     The builder being used to construct the model for this context. Databases (and other extensions) typically
    ///     define extension methods on this object that allow you to configure aspects of the model that are specific
    ///     to a given database.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
    }
}