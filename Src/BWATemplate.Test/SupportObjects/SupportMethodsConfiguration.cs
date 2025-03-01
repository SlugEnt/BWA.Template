namespace BWATemplate.Test;

public class SupportMethodsConfiguration
{
    /// <summary>
    ///     If true AutoMapper will be setup and configured.
    /// </summary>
    public bool UseAutoMapper { get; set; } = false;

    /// <summary>
    ///     If true, the database will be setup and utilized.  If false, it will not
    /// </summary>
    public bool UseDatabase { get; set; } = true;

    /// <summary>
    ///     If using the database, this determines if the system should use transactions or not.
    ///     The use of Transactions will result in a specific unit test rolling back all of its testing
    ///     at the end of the test.  However, some of the tests involve transactions and cannot be nested.
    ///     For these the UseTransactions should be set to false.
    /// </summary>
    public bool UseTransactions { get; set; } = true;
}