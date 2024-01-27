namespace Frank.WorkflowEngine;

public class JobResult
{
    public JobResult(List<StepResult> stepResults)
    {
        StepResults = stepResults;
        Success = stepResults.All(x => x.Success);
    }

    public bool Success { get; set; }
    public string? Message { get; set; }
    
    public List<StepResult> StepResults { get; set; }
}