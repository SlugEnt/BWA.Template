
namespace SlugEnt.BWA.Common;
/// <summary>
///     Provides the .Net Core DateTime Standard Methods
/// </summary>
public class SystemDateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}