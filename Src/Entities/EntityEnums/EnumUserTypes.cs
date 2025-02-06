namespace SlugEnt.BWA.Entities.EntityEnums;

/// <summary>
/// Defines the type of Person entities
/// </summary>
public enum EnumUserTypes : byte
{
    // Non person system related object used to impersonate a person in some scenarios
    System = 0,

    SuperUser  = 10,
    NormalUser = 20,


    // Terminations
    Termed = 254
}