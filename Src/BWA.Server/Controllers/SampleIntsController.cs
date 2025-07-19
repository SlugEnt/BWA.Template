using BWA.Server.Controllers.AppEntities;
using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.HR.NextGen.Entities.Models;

namespace BWA.Server.Controllers.IntLookups;


/// <summary>
/// API Controller for the SampleInts Entity
/// Entity is SampleInt
/// Based upon the base entity type: Int (Valid Values are Int, Long, Str, Guid, Ulid)
/// 
/// </summary>
/// <remarks>
/// Constructor for the SampleIntsController class.  This is where the logger, database context, and entity engine are injected
/// </remarks>
/// <param name="logger"></param>
/// <param name="db"></param>
/// <param name="repositoryE2Entity"></param>
[Route("api/[controller]")]
[ApiController]
public class SampleIntsController : EntityControllerInt<SampleInt>, IEntityControllerInt<SampleInt>

{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger for the Controller</param>
    /// <param name="db">Database Connection</param>
    /// <param name="repositoryE2Entity">Database Engine to communicate with</param>
    public SampleIntsController(ILogger<SampleIntsController> logger,
                       AppDbContext db,
                       IEntityRepositoryE2Int<SampleInt> repositoryE2Entity,
                       IServiceProvider serviceProvider) : base(logger,
                                                                db,
                                                                repositoryE2Entity,
                                                                serviceProvider)
    {
        ControllerName = "SampleInts";
    }
}