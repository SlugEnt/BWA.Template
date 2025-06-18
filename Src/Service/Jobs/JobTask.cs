using Quartz;

namespace Service.Jobs;

/// <summary>
/// This is the base class for all Jobs.  
/// </summary>
public abstract class JobTask : IJob
{
    /// <summary>
    /// The code that the job should run
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public abstract Task Execute(IJobExecutionContext ctx);

    /// <summary>
    /// Defines the base setup for the job, including the default JobDetails and Trigger
    /// </summary>
    public abstract void DefaultSetup();

    /// <summary>
    /// Defines the job using the Job's own defaults for the JobDetails and Trigger
    /// </summary>
    /// <param name="scheduler"></param>
    /// <returns></returns>
    public async Task DefineJob(IScheduler scheduler)
    {
        DefaultSetup();
        await AddToSchedule(scheduler);
    }


    /// <summary>
    /// Defines the job using the provided JobDetails and Trigger.  If either is null, then the Job's own defaults are used.
    /// </summary>
    /// <param name="scheduler">The scheduler to add the job to</param>
    /// <param name="jobDetail">The actual details about the job</param>
    /// <param name="trigger">The actual trigger that determines when the job runs</param>
    /// <returns></returns>
    public async Task DefineJob(IScheduler scheduler,
                                IJobDetail jobDetail = null,
                                ITrigger trigger = null)
    {
        if (jobDetail == null || trigger == null)
            DefaultSetup();
        if (jobDetail != null)
            JobDetail = jobDetail;
        if (trigger != null)
            Trigger   = trigger;
        await AddToSchedule(scheduler);
    }


    /// <summary>
    /// Adds the job to the scheduler
    /// </summary>
    /// <param name="scheduler"></param>
    /// <returns></returns>
    protected virtual async Task AddToSchedule(IScheduler scheduler)
    {
        await scheduler.ScheduleJob(JobDetail, Trigger);
    }

    /// <summary>
    /// The trigger that determines when the job is run
    /// </summary>
    public ITrigger Trigger { get; set; } = null;

    /// <summary>
    /// The details about the job.
    /// </summary>
    public IJobDetail JobDetail { get; set; } = null;

}
