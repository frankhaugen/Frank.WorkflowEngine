using Microsoft.Extensions.DependencyInjection;

namespace Frank.WorkflowEngine;

public class ScopedServicesProvider(IServiceScope serviceScope) : IScopedServicesProvider
{
    public T GetService<T>() where T : notnull
        => serviceScope.ServiceProvider.GetRequiredService<T>();

    public T GetService<T>(object key) where T : notnull
        => serviceScope.ServiceProvider.GetRequiredKeyedService<T>(key);

    public IEnumerable<T> GetServices<T>() where T : notnull
        => serviceScope.ServiceProvider.GetServices<T>();

    public IEnumerable<T> GetServices<T>(object key) where T : notnull
        => serviceScope.ServiceProvider.GetKeyedServices<T>(key);

    public void Dispose() => serviceScope.Dispose();
}

public interface IScopedServicesProvider : IDisposable
{
    /// <summary>
    /// Retrieves a service of type T from the service container.
    /// </summary>
    /// <typeparam name="T">The type of the service to retrieve.</typeparam>
    /// <returns>The service of type T if found in the service container; otherwise, null.</returns>
    public T GetService<T>() where T : notnull;

    /// <summary>
    /// Gets the service instance of type T associated with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of the service instance to retrieve.</typeparam>
    /// <param name="key">The key associated with the service instance.</param>
    /// <returns>The service instance of type T associated with the specified key.</returns>
    public T GetService<T>(object key) where T : notnull;

    /// <summary>
    /// Retrieves the services of type T.
    /// </summary>
    /// <typeparam name="T">The type of services to retrieve.</typeparam>
    /// <returns>
    /// An enumerable collection of services of type T.
    /// </returns>
    public IEnumerable<T> GetServices<T>() where T : notnull;

    /// <summary>
    /// Gets the services of type T with the specified key.
    /// </summary>
    /// <typeparam name="T">The type of services to retrieve.</typeparam>
    /// <param name="key">The key used to identify the services.</param>
    /// <returns>
    /// An enumerable collection of services of type T with the specified key.
    /// </returns>
    public IEnumerable<T> GetServices<T>(object key) where T : notnull;
}