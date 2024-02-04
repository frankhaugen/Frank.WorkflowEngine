using Microsoft.Extensions.DependencyInjection;

namespace Frank.WorkflowEngine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflow(this IServiceCollection services, Action<WorkflowBuilder> configure)
    {
        var builder = new WorkflowBuilder(services);
        configure(builder);
        
        return services;
    }
}