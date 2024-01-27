using Frank.CronJobs.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.WorkflowEngine;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowEngine(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<TimeProvider, WorkflowTimeProvider>();
        services.AddTransient<IScopedServicesProvider>(provider => new ScopedServicesProvider(provider.GetRequiredService<IServiceScopeFactory>().CreateScope()));
        services.AddCronJobs(configuration, builder =>
        {
            builder.AddCronJob<WorkflowEngine>("0 * * * * *");
        });

        return services;
    }
    
    public static IServiceCollection AddJob<TJob>(this IServiceCollection services, Guid jobId) where TJob : class, IJob
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));
        services.AddKeyedScoped<IJob, TJob>(jobId);
        return services;
    }
    
    public static IServiceCollection AddStep<TStep>(this IServiceCollection services, Guid jobId) where TStep : class, IStep
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));
        services.AddKeyedTransient<IStep, TStep>(jobId);
        return services;
    }
    
    public static IServiceCollection AddCondition<TCondition>(this IServiceCollection services, Guid jobId) where TCondition : class, ICondition
    {
        if (jobId == Guid.Empty)
            throw new ArgumentException("Job ID cannot be empty.", nameof(jobId));
        services.AddKeyedTransient<ICondition, TCondition>(jobId);
        return services;
    }
}