using System.Threading.Channels;
using Microsoft.Extensions.DependencyInjection;

namespace Frank.WorkflowEngine.Tests;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IServiceScope>(provider => provider.CreateScope());
        services.AddWorkflowEngine(builder => builder
            .AddStartStep<FirstStep, SecondStepTrigger>()
            .AddStep<SecondStep, SecondStepTrigger, ThirdStepTrigger>()
            .AddEndStep<LastStep, ThirdStepTrigger>());
        
        // Act
        var provider = services.BuildServiceProvider();
  
        // Assert
        var engine = provider.GetService<WorkflowEngine>();
        engine.Should().NotBeNull();
        engine.Should().BeOfType<WorkflowEngine>();
        
        await engine!.StartAsync<FirstStep>();
    }
    
    // Define the StepTrigger classes
    public record SecondStepTrigger();
    public record ThirdStepTrigger();

// Define Real Workflow Steps
    public class FirstStep : Step 
    {
        public FirstStep(IServiceScope serviceScope) : base(serviceScope) {}
    
        public override async Task ExecuteAsync() 
        {
            Console.WriteLine("First step executed.");
            Channel<SecondStepTrigger> channel = Services.GetService<Channel<SecondStepTrigger>>();
            await channel.Writer.WriteAsync(new SecondStepTrigger());
        }
    }

    public class SecondStep : Step 
    {
        public SecondStep(IServiceScope serviceScope) : base(serviceScope) {}
    
        public override async Task ExecuteAsync() 
        {
            Console.WriteLine("Second step executed.");
            Channel<ThirdStepTrigger> channel = Services.GetService<Channel<ThirdStepTrigger>>();
            await channel.Writer.WriteAsync(new ThirdStepTrigger());
        }
    }

    public class LastStep : Step 
    {
        public LastStep(IServiceScope serviceScope) : base(serviceScope) {}
    
        public override async Task ExecuteAsync() 
        {
            Console.WriteLine("Last step executed.");
        }
    }
}
