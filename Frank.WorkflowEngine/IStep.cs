namespace Frank.WorkflowEngine;

public interface IStep<TIn, TOut>
{
    Task<TOut> ExecuteAsync(TIn input);
}