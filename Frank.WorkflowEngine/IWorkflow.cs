namespace Frank.WorkflowEngine;

public interface IWorkflow : IIdentity
{
    JobContext CreateJobContext();
    
    public ITrigger? Trigger { get; }
}