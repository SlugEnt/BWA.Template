/// <summary>
/// This creates an Attribute for Models / Entities to set a SQL Default value
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class SqlDefaultValueAttribute : Attribute
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="defaultValue"></param>
    public SqlDefaultValueAttribute(string defaultValue)
    {
        DefaultValue = defaultValue;
    }


    /// <summary>
    /// The default value to be used in the SQL Table
    /// </summary>
    public string DefaultValue { get; set; }
}