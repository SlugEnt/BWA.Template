using Microsoft.Extensions.DependencyInjection;
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

}

