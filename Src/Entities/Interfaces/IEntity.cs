using SlugEnt.HR.NextGen.Entities;

namespace SlugEnt.BWA.Entities;

public interface IEntity : IActiveModel,  IAuditModel
{
    public string FullName { get; }

    /// <summary>
    /// Whether the Entity allows Deletes
    /// </summary>
    public static bool AllowDelete { get; }

    /// <summary>
    /// Returns the ID of the Entity as a string.
    /// </summary>
    public string KeyId { get; }

}