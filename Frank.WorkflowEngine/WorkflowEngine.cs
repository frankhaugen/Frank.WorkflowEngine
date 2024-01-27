using Frank.CronJobs.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.WorkflowEngine;

public class WorkflowEngine : ICronJob
{
    private readonly IScopedServicesProvider _scopedServicesProvider;

    public WorkflowEngine(IScopedServicesProvider scopedServicesProvider)
    {
        _scopedServicesProvider = scopedServicesProvider;
    }

    public async Task RunAsync()
    {
        var workflows = _scopedServicesProvider.GetServices<IWorkflow>();
        var timeProvider = _scopedServicesProvider.GetService<TimeProvider>();
        var now = timeProvider.GetUtcNow();
        
        foreach (var workflow in workflows)
        {
            if (workflow.Trigger is not null && !workflow.Trigger.Evaluate(now.UtcDateTime))
                continue;
        }
    }
}