namespace Frank.WorkflowEngine;

public interface IStartStep<TOut>
{
    Task RunAsync(CancellationToken cancellationToken);
}