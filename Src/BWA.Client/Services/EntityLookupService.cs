using Microsoft.Extensions.Caching.Memory;
using SlugEnt.BWA.BusinessComponents.Abstracts.ErrorManagement;
using SlugEnt.BWA.Entities;

namespace BWA.Client.Services;

/// <summary>
/// Service that provides the interface to accessing a specfic Lookup Entities data.
/// This is 99.4% the same code as the EntityDataService, but it is used for Lookup entities.
/// </summary>
/// <typeparam name="TLookup">Is a IEntityLookup type of object which is a form of Entity.</typeparam>
public class EntityLookupService<TLookup> : EntityDataServiceInt<TLookup>
    where TLookup : AbstractEntityInt, IEntityLookupInt, IEntityInt, new()


{
    public EntityLookupService(HttpClient httpClient,
                               IMemoryCache memoryCache,
                               ErrorManager errorManager) : base(httpClient, memoryCache, errorManager) { }
}