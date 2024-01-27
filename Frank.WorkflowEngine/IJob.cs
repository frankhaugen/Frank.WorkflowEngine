namespace Frank.WorkflowEngine;

public interface IJob : IIdentity
{
    Guid WorkflowId { get; }
    
    Task<JobResult> ExecuteAsync(JobContext context);
}