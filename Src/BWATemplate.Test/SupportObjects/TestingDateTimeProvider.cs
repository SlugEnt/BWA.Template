

using SlugEnt.BWA.Common;

namespace BWATemplate.Test.SupportObjects;

internal class TestingDateTimeProvider : IDateTimeOffsetProvider
{
    /// <summary>
    ///     The current UTC offset for the object
    /// </summary>
    public int UTC_Offset { get; set; } = 5;


    /// <summary>
    ///     Gets current date and time.  Allows to override the returned value
    /// </summary>
    public DateTimeOffset Now { get; set; }


    /// <summary>
    ///     Gets current UTC date and time.  Allows to override the returned value
    /// </summary>
    public DateTimeOffset UtcNow { get; set; }
}