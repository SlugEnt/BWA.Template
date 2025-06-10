using BWA.Server.Controllers.AppEntities;
using BWA.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using SlugEnt.BWA.Database;
using SlugEnt.BWA.Entities;
using SlugEnt.HR.NextGen.Entities.Models;

namespace SlugEnt.BWA.Server.Controllers;



/// <summary>
/// API Controller for the Companies Lookup Entity
/// </summary>
/// <remarks>
/// Constructor for the CompaniesController class.  This is where the logger, database context, and entity engine are injected
/// </remarks>
/// <param name="logger"></param>
/// <param name="db"></param>
/// <param name="repositoryE2Entity"></param>
[Route("api/[controller]")]
[ApiController]
public class SampleLongsController : EntityControllerLong<SampleLong>, IEntityControllerLong<SampleLong>

{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="logger">Logger for the Controller</param>
    /// <param name="db">Database Connection</param>
    /// <param name="repositoryE2Entity">Database Engine to communicate with</param>
    public SampleLongsController(ILogger<SampleLongsController> logger,
                                AppDbContext db,
                                IEntityRepositoryE2Long<SampleLong> repositoryE2Entity,
                                IServiceProvider serviceProvider) : base(logger,
                                                                         db,
                                                                         repositoryE2Entity,
                                                                         serviceProvider)
    {
        ControllerName = "SampleLongs";
    }
}
