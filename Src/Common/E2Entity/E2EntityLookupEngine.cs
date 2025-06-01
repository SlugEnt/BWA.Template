using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;

namespace SlugEnt.BWA.Common;

#pragma warning disable CS1998

/// <summary>
/// This is a generic repository for working with LookupInt entities.  This is a generic interface that can be used to work with any entity that has an integer key and implements the IIdIntModel interface.
/// This is not implemented yet.
/// </summary>
/// <typeparam name="TEntity">An Entity classified object.</typeparam>
/// <summary>
/// Constructor for the IntLookupRepository.  This will set the database context and logger for the repository.  This is a generic repository that can be used to work with any entity that has an integer key and implements the IIdIntModel interface.
/// </summary>
public class E2EntityLookupEngine<TEntity> : E2EntityRepositoryInt<TEntity>  where TEntity : AbstractEntityLookups, IEntityLookupInt, new()
//:  IEntityRepositoryE2<T>  where T : class, IEntityLookup
    {

    /// <summary>
    /// Constructor for the E2EntityLookupEngine.  This will set the database context and logger for the repository.  This is a generic repository that can be used to work with any entity that has an integer key and implements the IIdIntModel interface.
    /// </summary>
    /// <param name="db"></param>
    /// <param name="logger"></param>
    [ActivatorUtilitiesConstructor]
    public E2EntityLookupEngine(AppDbContext db, ILogger<E2EntityRepositoryInt<TEntity>> logger) : base(db, logger) { }
}
#pragma warning restore CS1998
