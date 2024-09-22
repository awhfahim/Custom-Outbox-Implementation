using Microsoft.Extensions.Options;
using Quartz;

namespace SecurityManagement.Infrastructure.OutboxProcessor;

internal sealed class ConfigureProcessOutboxJob
    : IConfigureOptions<QuartzOptions>
{
    public void Configure(QuartzOptions options)
    {
        var jobName = typeof(ProcessOutboxJob).FullName ?? nameof(ProcessOutboxJob);

        options
            .AddJob<ProcessOutboxJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure.ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(5).RepeatForever()));
    }
}
