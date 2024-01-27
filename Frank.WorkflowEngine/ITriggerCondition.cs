namespace Frank.WorkflowEngine;

public interface ITriggerCondition : ICondition, ITrigger
{
    // This interface might inherit the Evaluate methods from both interfaces.
    // You can add additional methods or properties if needed for combined conditions.
}