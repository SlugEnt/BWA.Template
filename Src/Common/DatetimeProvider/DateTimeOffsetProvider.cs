namespace SlugEnt.BWA.Common;

public class DateTimeOffsetProvider : IDateTimeOffsetProvider
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
}