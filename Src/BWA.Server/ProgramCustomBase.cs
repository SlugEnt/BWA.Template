using SlugEnt.IS.AppRun;

namespace SlugEnt.BWA.Server;

/// <summary>
/// This is the base class for ProgramCustom.  
/// </summary>
public abstract class ProgramCustomBase
{
    /// <summary>
    /// App Runtime object - contains things like the Configuration, Logger, etc.
    /// </summary>
    protected readonly AppRuntime _appRuntime;


    /// <summary>
    ///    Constructor for the Program Custom Base
    /// </summary>
    /// <param name="appRuntime"></param>
    public ProgramCustomBase(AppRuntime appRuntime) { _appRuntime = appRuntime; }


    /// <summary>
    /// Configures the Database
    /// </summary>
    public virtual void ConfigureDB(WebApplicationBuilder builder) { }


    /// <summary>
    /// Allows application to add its own needed custom services
    /// </summary>
    /// <param name="builder"></param>
    public virtual void AddServices(WebApplicationBuilder builder) { }


    /// <summary>
    /// Allows application to add to the Build App process.  This is called early in the BuildApp process
    /// </summary>
    /// <param name="app"></param>
    public virtual void BuildApp_UsesStart(WebApplication app) { }


    /// <summary>
    /// Allows application to add to the Build App process.  This is called near the end of the BuildApp process
    /// </summary>
    /// <param name="app"></param>
    public virtual void BuildApp_UsesEnd(WebApplication app) { }


    /// <summary>
    /// Performs any final steps in the BuildApp process.  This is called at the very end of the BuildApp process
    /// </summary>
    /// <param name="app"></param>
    public virtual void BuildApp_Final(WebApplication app) { }


    /// <summary>
    /// After the application has started, this is called to allow for any post start logic
    /// </summary>
    public virtual void PostStartLogic(WebApplication app) { }
}