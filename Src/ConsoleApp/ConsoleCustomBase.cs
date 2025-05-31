using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SlugEnt.IS.AppRun;

namespace ConsoleApp;

/// <summary>
/// Serves as the base abstract class for removing custom hostbuilder logic out of the program.cs file for console apps.
/// </summary>
public abstract class ConsoleCustomBase
{
    /// <summary>
    /// App Runtime object 
    /// </summary>
    protected readonly AppRuntime _appRuntime;


    /// <summary>
    ///    Constructor for the Program Custom Base
    /// </summary>
    /// <param name="appRuntime"></param>
    public ConsoleCustomBase(AppRuntime appRuntime)
    {
        _appRuntime = appRuntime;
    }


    public virtual void ConfigureDB(IServiceCollection services) { }


    /// <summary>
    /// Allows application to add its own needed custom services
    /// </summary>
    /// <param name="builder"></param>
    public virtual void AddServices(IServiceCollection services) { }



    /// <summary>
    /// This method is called after the host has started.  This is a good place to put any code that needs to run immediately
    /// after startup.  This is not the same as the Main method.  This is called after the host has started and all services have been registered.
    /// </summary>
    public virtual void AfterStarting(IHost host) { }
}

