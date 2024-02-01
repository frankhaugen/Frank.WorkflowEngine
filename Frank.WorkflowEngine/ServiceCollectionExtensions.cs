using System.Collections.Concurrent;
using System.Threading.Channels;
using Frank.Mapping;
using Frank.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.WorkflowEngine;

public interface IStep
{
    Task ExecuteAsync();
}

public abstract class Step(IServiceScope serviceScope) : IStep
{
    protected IServiceProvider Services => serviceScope.ServiceProvider;
    
    public abstract Task ExecuteAsync();
}

public class WorkflowBuilder(IServiceCollection services)
{
    public WorkflowBuilder AddStartStep<TStep, TNextStepTrigger>() where TStep : class, IStep where TNextStepTrigger : class, new()
    {
        services.AddSingleton<TStep>();
        services.AddChannelBinding<FirstStepTrigger, TNextStepTrigger>(result => new TNextStepTrigger());
        services.AddChannel<FirstStepTrigger>();
        services.AddChannel<TNextStepTrigger>();
        return this;
    }
    
    public WorkflowBuilder AddStep<TStep, TStepResult, TNextStepTrigger>() where TStep : class, IStep where TStepResult : class, new() where TNextStepTrigger : class, new()
    {
        services.AddSingleton<TStep>();
        services.AddChannelBinding<TStepResult, TNextStepTrigger>(result => new TNextStepTrigger());
        services.AddChannel<TStepResult>();
        services.AddChannel<TNextStepTrigger>();
        return this;
    }
    
    public WorkflowBuilder AddEndStep<TStep, TStepResult>() where TStep : class, IStep where TStepResult : class, new()
    {
        services.AddSingleton<TStep>();
        services.AddChannelBinding<TStepResult, LastStepResult>(result => new LastStepResult());
        services.AddChannel<TStepResult>();
        services.AddChannel<LastStepResult>();
        return this;
    }
}

public record LastStepResult();

public record FirstStepTrigger();

public sealed class ChannelBinding<TSource, TTarget> : IDisposable
{
    private readonly ChannelReader<TSource> _reader;
    private readonly ChannelWriter<TTarget> _writer;
    private readonly IMappingDefinition<TSource, TTarget> _transform;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _task;

    public ChannelBinding(ChannelReader<TSource> reader, ChannelWriter<TTarget> writer, IMappingDefinition<TSource, TTarget> transform)
    {
        _reader = reader;
        _writer = writer;
        _transform = transform;
        _task = Task.Run(ExecuteAsync, _cancellationTokenSource.Token);
    }

    private async Task ExecuteAsync()
    {
        try
        {
            await foreach (var item in _reader.ReadAllAsync(_cancellationTokenSource.Token))
            {
                await _writer.WriteAsync(_transform.Map(item), _cancellationTokenSource.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // ignored
        }
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _task.Wait();
        _cancellationTokenSource.Dispose();
    }
}

public class WorkflowEngine
{
    private readonly IServiceProvider _services;

    public WorkflowEngine(IServiceProvider services)
    {
        _services = services;
    }

    public async Task StartAsync<T>() where T : IStep
    {
        var step = _services.GetRequiredService<T>();
        await step.ExecuteAsync();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowEngine(this IServiceCollection services, Action<WorkflowBuilder> configure)
    {
        services.AddChannelFactory();
        services.AddSingleton<WorkflowEngine>();
        var builder = new WorkflowBuilder(services);
        configure(builder);
        return services;
    }
    
    public static IServiceCollection AddChannelBinding<TSource, TTarget>(this IServiceCollection services, Func<TSource, TTarget> transform) where TSource : class where TTarget : class
    {
        services.AddSimpleMapping(transform);
        services.AddSingleton(provider => new ChannelBinding<TSource, TTarget>(
            provider.GetRequiredService<ChannelReader<TSource>>(),
            provider.GetRequiredService<ChannelWriter<TTarget>>(),
            provider.GetRequiredService<IMappingDefinition<TSource, TTarget>>()));
        return services;
    }
    
    public static IServiceCollection AddChannelFactory(this IServiceCollection services, Action<ChannelFactoryBuilder>? configure = null)
    {
        services.AddSingleton<ChannelFactory>();
        ChannelFactoryBuilder builder = new(services);
        configure?.Invoke(builder);
        return services;
    }
    
    public static IServiceCollection AddChannel<T>(this IServiceCollection services) where T : class
    {
        services.AddSingleton<Channel<T>>(provider => provider.GetRequiredService<ChannelFactory>().CreateChannel<T>());
        services.AddSingleton<ChannelReader<T>>(provider => provider.GetRequiredService<Channel<T>>().Reader);
        services.AddSingleton<ChannelWriter<T>>(provider => provider.GetRequiredService<Channel<T>>().Writer);
        return services;
    }
}

public class ChannelFactoryBuilder(IServiceCollection services)
{
    public ChannelFactoryBuilder AddChannel<T>() where T : class
    {
        services.AddChannel<T>();
        return this;
    }
}

public class ChannelFactory()
{
    private readonly ConcurrentDictionary<string, object> _cache = new();
    
    public Channel<T> CreateChannel<T>() where T : class => _cache.GetOrAdd(typeof(T).GetDisplayName(), Value<T>) as Channel<T> ?? throw new InvalidOperationException($"{typeof(Channel<T>).GetDisplayName()} not found");

    private static object Value<T>(string arg) where T : class =>
        Channel.CreateUnbounded<T>(new UnboundedChannelOptions()
        {
            SingleReader = true,
            SingleWriter = true
        });
}