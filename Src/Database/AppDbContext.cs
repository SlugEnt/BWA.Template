using ByteAether.Ulid;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SlugEnt.BWA.Entities;
using SlugEnt.BWA.Entities.Models;
using SlugEnt.HR.NextGen.Entities.Models;

namespace SlugEnt.BWA.Database;

public class AppDbContext : DbContext
{
    /// <summary>
    ///     Override this method to further configure the model that was discovered by convention from the entity types
    ///     exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" /> properties on your derived context. The resulting
    ///     model may be cached
    ///     and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         If a model is explicitly set on the options for this context (via
    ///         <see
    ///             cref="M:Microsoft.EntityFrameworkCore.DbContextOptionsBuilder.UseModel(Microsoft.EntityFrameworkCore.Metadata.IModel)" />
    ///         )
    ///         then this method will not be run. However, it will still run when creating a compiled model.
    ///     </para>
    ///     <para>
    ///         See <see href="https://aka.ms/efcore-docs-modeling">Modeling entity types and relationships</see> for more
    ///         information and
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
        Console.WriteLine("In OnModelCreating");
        base.OnModelCreating(modelBuilder);


        //Add Auditing fields to the entities that need it!
        AddAuditing<AppSetting>(modelBuilder);
        AddAuditing<SampleInt>(modelBuilder);
        AddAuditing<SampleUlid>(modelBuilder);
        AddAuditing<SampleString>(modelBuilder);
        AddAuditing<SampleLong>(modelBuilder);
        AddAuditing<SampleGuid>(modelBuilder);


    }


    /// <summary>
    /// Appends the LastModifiedBy / CreatedBy and the LastModifiedAt / CreatedAt fields to the entities
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="mb"></param>
    public static void AddAuditing<T>(ModelBuilder mb) where T : AbstractAuditableEntity
    {
        string tableName = typeof(T).Name;

        // TODO  - We no longer link to a User Id, but rather just store the User Id in the field.
/*        mb.Entity<T>()
          .HasOne(x => x.CreatedByUser)
          .WithMany()
          .HasForeignKey("CreatedById" + tableName);

        mb.Entity<T>()
          .HasOne(x => x.LastModifiedByUser)
          .WithMany()
          .HasForeignKey("LastModifiedById" + tableName);
*/
    }


    #region "Tables"

        public DbSet<AppSetting> AppSettings { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<SampleInt> SampleInt { get; set; }
    public DbSet<SampleString> SampleString { get; set; }
    public DbSet<SampleUlid> SampleUlids { get; set; }
    public DbSet<SampleLong> SampleLongs { get; set; }
    public DbSet<SampleGuid> SampleGuids { get; set; }


    #endregion


    #region "BasicStuff"

    /// <summary>
    /// Keeps track of transactions in the DB.
    /// </summary>
    public DatabaseTransactionManager DatabaseTransactionManager;


    public AppDbContext()
    {
        Console.WriteLine("In AppDbContext Empty Constructor");
        Initialize();
    }


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // TODO Change to optional value or move back to 30 seconds.
        /*      Database.SetCommandTimeout(new TimeSpan(0,
                                                      0,
                                                      0,
                                                      5));
        */

        Console.WriteLine("In AppDb Options Constructor");
        Initialize();
    }


    /// <summary>
    /// Sets up the Database Transaction Manager
    /// </summary>
    private void Initialize()
    {
        // Initiate the transaction manager.  Nothing happens until the caller decides it wants a transaction.
        DatabaseTransactionManager = new DatabaseTransactionManager(this);
    }



    /// <summary>
    ///     This
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
            

            if (entry.Entity is AbstractAuditableEntity)
            {
                AbstractAuditableEntity auditableEntity = (AbstractAuditableEntity)entry.Entity;

                switch (entry.State)
                {
                    case EntityState.Added:
                        auditableEntity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Deleted:
                    case EntityState.Modified:
                        auditableEntity.LastModifiedAt = DateTime.UtcNow;
                        break;
                }
            }

        }
    }




    /// <summary>
    /// Enable the SQL Server Attribute for Default Values
    /// </summary>
    /// <param name="configurationBuilder"></param>
    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Conventions.Add(services =>
                                                 new SqlDefaultValueConvention(
                                                                               services.GetRequiredService<ProviderConventionSetBuilderDependencies>()));

        // Required for the Ulid to work with the model properties correctly.
        configurationBuilder.Properties<Ulid>()
                            .HaveConversion<UlidToBytesConverter>();
    }


    /// <summary>
    ///     Returns the value the Database is referenced by in AppSettings and other locations.
    /// </summary>
    /// <returns></returns>
    public static string DatabaseReferenceName() { return "MyAppDB"; }

#endregion
}