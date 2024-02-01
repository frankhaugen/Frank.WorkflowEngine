using Frank.Testing.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Frank.WorkflowEngine.Tests;

public class WorkflowBuilderTests
{
    private readonly IServiceCollection _services;
    private readonly ILogger<WorkflowBuilder> _logger;

    public WorkflowBuilderTests(ITestOutputHelper outputHelper)
    {
        _services = Substitute.For<IServiceCollection>();
        _logger = outputHelper.CreateTestLogger<WorkflowBuilder>();
        
        _services.AddSingleton(_logger);
    }

}