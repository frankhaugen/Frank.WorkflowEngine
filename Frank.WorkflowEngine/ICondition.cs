namespace Frank.WorkflowEngine;

public interface ICondition
{
    bool Evaluate(object context);
}