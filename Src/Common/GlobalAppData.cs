using SlugEnt.BWA.Entities.Models;

namespace SlugEnt.BWA.Common;

/// <summary>
/// Data that needs to be used across the app for global reasons.  Used by Console, Service and API / Web Applications.  Not applicable to Blazor WebAssembly client side.
/// </summary>
public class GlobalAppData
{

    /// <summary>
    /// The System Person is the person that is used for identifying when the system has made changes or updated entities.
    /// </summary>
    public static User? SystemUser { get; set; } = null;


    /// <summary>
    /// Base URL for the API
    /// </summary>
    public static Uri ApiBaseUrl { get; set; }


    /// <summary>
    /// The application level settings.
    /// </summary>
    public static AppSetting? AppSettings { get; set; }

    public static IServiceProvider ServiceProvider { get; set; } = null!;
}


public interface IGlobalAppData { }
