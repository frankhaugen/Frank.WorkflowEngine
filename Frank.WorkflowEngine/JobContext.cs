namespace Frank.WorkflowEngine;

public class JobContext
{
    public ICollection<IStep> Steps { get; } = new List<IStep>();
}