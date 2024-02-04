namespace Frank.WorkflowEngine;

public interface IEndStep<T>
{
    Task ExecuteAsync(T result);
}