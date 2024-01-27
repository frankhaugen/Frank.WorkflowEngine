namespace Frank.WorkflowEngine;

public class StepResult
{
    // Indicates whether the step was executed successfully
    public bool Success { get; set; }

    // Contains a message or description of the result or any error
    public string Message { get; set; }

    // Any data produced by the step that may be needed downstream
    public Dictionary<string, object> OutputData { get; set; }

    public StepResult()
    {
        OutputData = new Dictionary<string, object>();
    }

    // Add methods or properties to handle different scenarios or results
}