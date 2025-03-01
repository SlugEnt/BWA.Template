namespace SlugEnt.BWA.Common;
/// <summary>
///     Provides an Interface for being able to override DateTime for testing
/// </summary>
public interface IDateTimeOffsetProvider
{
    DateTimeOffset Now { get; }
    DateTimeOffset UtcNow { get; }
}