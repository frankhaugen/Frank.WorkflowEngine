using Frank.Channels.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.WorkflowEngine;

public class WorkflowBuilder(IServiceCollection services)
{
    private bool _hasStartStep = false;
    
    public WorkflowBuilder StartWith<TStep, TOut>() where TStep : class, IStartStep<TOut> where TOut : class
    {
        if (_hasStartStep)
            throw new InvalidOperationException("Start step already defined");
        _hasStartStep = true;
        services.AddChannel<TOut>();
        services.AddSingleton<IStartStep<TOut>, TStep>();
        return this;
    }
    
    public WorkflowBuilder Then<TStep, TIn, TOut>() where TStep : class, IStep<TIn, TOut> where TIn : class where TOut : class
    {
        services.AddSingleton<IStep<TIn, TOut>, TStep>();
        services.AddChannel<TIn>();
        services.AddChannel<TOut>();
        services.AddHostedService<StepRunner<TIn, TOut>>();
        return this;
    }
    
    public WorkflowBuilder ThenEndWith<TStep, TIn>() where TStep : class, IEndStep<TIn> where TIn : class
    {
        if (!_hasStartStep)
            throw new InvalidOperationException("Start step not defined");
        services.AddSingleton<IEndStep<TIn>, TStep>();
        services.AddChannel<TIn>();
        services.AddHostedService<EndStepRunner<TIn>>();
        return this;
    }
}
