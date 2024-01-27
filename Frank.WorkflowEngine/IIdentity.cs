namespace Frank.WorkflowEngine;

public interface IIdentity
{
    Guid Id { get; }
    string Name { get; }
    string? Description { get; }
}