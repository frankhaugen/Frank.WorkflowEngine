namespace Frank.WorkflowEngine;

public interface ITrigger
{
    bool Evaluate(DateTime triggerTime);
}