namespace Frank.WorkflowEngine;

public class StepInput
{
    // Identifiers or references to relevant entities
    public Guid WorkflowId { get; set; }
    public Guid JobId { get; set; }
    public Guid StepId { get; set; }

    // Parameters or data necessary for the step's execution
    public Dictionary<string, object> Parameters { get; set; }

    public StepInput()
    {
        Parameters = new Dictionary<string, object>();
    }

    // Add any other properties or methods that might be useful for passing data to a step
}