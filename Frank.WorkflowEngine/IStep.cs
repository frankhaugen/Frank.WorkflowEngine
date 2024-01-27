namespace Frank.WorkflowEngine;

public interface IStep : IIdentity
{
    int SortOrder { get; }
    
    Task<StepResult> ExecuteAsync(StepInput input);
}