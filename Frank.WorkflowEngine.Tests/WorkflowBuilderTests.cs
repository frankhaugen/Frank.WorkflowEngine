using System.Threading.Channels;
using Frank.Reflection;
using Frank.Testing.TestBases;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Frank.WorkflowEngine.Tests;

public class WorkflowBuilderTests : HostApplicationTestBase
{
    private readonly ITestOutputHelper _outputHelper;
    
    public WorkflowBuilderTests(ITestOutputHelper outputHelper) : base(outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    /// <inheritdoc />
    protected override Task SetupAsync(HostApplicationBuilder builder)
    {
        builder.Services.AddWorkflow(workflow =>
            {
                workflow.StartWith<StartStep, MyString>()
                    .Then<Step1, MyString, MyInt>()
                    .ThenEndWith<EndStep, MyInt>();
            });
        
        return Task.CompletedTask;
    }

    [Fact]
    public async Task Test()
    {
        // var thing = Services.GetService<IStep<MyString, MyInt>>();
        // thing.Should().NotBeNull();
        await Task.Delay(2500);
        await Task.CompletedTask;
    }

    public record MyString(string Value);
    public record MyInt(int Value);
    

    public class StartStep(ILogger<StartStep> logger, ChannelWriter<MyString> writer) : ITriggerStep<MyString>
    {
        /// <inheritdoc />
        public async Task RunAsync(CancellationToken cancellationToken)
        {
            var myValue = new MyString("Start");
            logger.LogInformation("StartStep executed with input {Input}", myValue.Value);
            await writer.WriteAsync(myValue, cancellationToken);
        }
    }

    public class Step1(ILogger<Step1> logger) : IStep<MyString, MyInt>
    {
        /// <inheritdoc />
        public async Task<MyInt> ExecuteAsync(MyString input)
        {
            logger.LogInformation("Step1 executed with input {Input}", input.Value);
            return await Task.FromResult(new MyInt(666));
        }
    }
    
    public class EndStep(ILogger<EndStep> logger) : IEndStep<MyInt>
    {
        /// <inheritdoc />
        public async Task ExecuteAsync(MyInt result)
        {
            logger.LogInformation("EndStep executed with result {Result}", result.Value);
            
            await Task.CompletedTask;
        }
    }
}
