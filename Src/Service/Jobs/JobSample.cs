using Quartz;
using SlugEnt.BWA.Common;
using SlugEnt.BWA.Database;


namespace Service.Jobs;


/// <summary>
/// Runs the Expiring Passwords Report Job
/// </summary>
public class JobSample : JobTask
{
    public override void DefaultSetup()
    {
        JobDetail = JobBuilder.Create<JobSample>()
                              .WithIdentity("Sample", "Group Sample")
                              .Build();
        Trigger = TriggerBuilder.Create().WithIdentity("Sample Trigger", "Group Sample B")
                                .StartNow()
                                .WithSimpleSchedule(x => 
                                                        x.WithIntervalInSeconds(60)
                                                         .WithRepeatCount(4))
                                .Build();
    }


    /// <summary>
    /// Runs the Job
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public override async Task Execute(IJobExecutionContext ctx)
    {
        // Put your code that needs to run here...
        int i = 3;
        i++;
        await Console.Out.WriteAsync("Hello from Sample Job");
        await Console.Out.WriteAsync(Environment.NewLine);
    }
}