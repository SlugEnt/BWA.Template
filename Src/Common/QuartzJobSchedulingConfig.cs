using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SlugEnt.BWA.Database;

namespace SlugEnt.BWA.Common;


/// <summary>
/// Configures the Quartz Job Scheduling engine
/// </summary>
public static class QuartzJobSchedulingConfig
{
    /// <summary>
    /// Performs the actual configuration of the Job Scheduling Engine
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void SetConfiguration(IServiceCollection services,
                                        IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.UsePersistentStore(s =>
            {
                s.UseProperties = true;
                s.UseSystemTextJsonSerializer();
                s.UseSqlServer(o =>
                {
                    string? connStr = configuration.GetConnectionString(AppDbContext.DatabaseReferenceName());
                    o.ConnectionString = connStr;
                    o.TablePrefix      = "[QuartZ].QRTZ_";
                });
            });
        });
    }
}