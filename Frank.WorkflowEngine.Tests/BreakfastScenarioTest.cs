using Frank.CronJobs.Cron;
using Frank.PulseFlow;
using Frank.Testing.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Frank.WorkflowEngine.Tests;

public class BreakfastScenarioTest
{
    private readonly ITestOutputHelper _outputHelper;

    public BreakfastScenarioTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }
    
    [Fact]
    public async Task RunAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(6));
        var host = CreateHost();
        await host.RunAsync(cancellationTokenSource.Token);
    }
    
    private IHost CreateHost()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddPulseFlowTestLoggingProvider(_outputHelper);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddPulseFlow(builder =>
                {
                });
                services.AddWorkflowEngine(context.Configuration);
                services.AddJob<MakeCoffeeJob>(MyTestIds.Instance.MakeCoffeeJob);
                services.AddJob<MakeToastJob>(MyTestIds.Instance.MakeToastJob);
                services.AddJob<MakeEggsJob>(MyTestIds.Instance.MakeEggsJob);
                services.AddJob<MakeBaconJob>(MyTestIds.Instance.MakeBaconJob);
                services.AddJob<ServeBreakfastJob>(MyTestIds.Instance.ServeBreakfastJob);
                
                services.AddStep<GrindCoffeeBeansStep>(MyTestIds.Instance.GrindCoffeeBeansStep);
                services.AddStep<BoilWaterStep>(MyTestIds.Instance.BoilWaterStep); 
                services.AddStep<BrewCoffeeStep>(MyTestIds.Instance.BrewCoffeeStep);
                services.AddStep<ToastBreadStep>(MyTestIds.Instance.ToastBreadStep);
                services.AddStep<FryEggsStep>(MyTestIds.Instance.FryEggsStep);
                services.AddStep<FryBaconStep>(MyTestIds.Instance.FryBaconStep);
                services.AddStep<ServeBreakfastStep>(MyTestIds.Instance.ServeBreakfastStep);
                
            })
            .Build();
    }
    
    [Fact]
    public void GenerateIds()
    {
        var workflowId = IdHelper.CreateWorkflowId();
        var makeCoffeeJobId = IdHelper.CreateJobId(workflowId);
        var makeToastJobId = IdHelper.CreateJobId(workflowId);
        var makeEggsJobId = IdHelper.CreateJobId(workflowId);
        var makeBaconJobId = IdHelper.CreateJobId(workflowId);
        var serveBreakfastJobId = IdHelper.CreateJobId(workflowId);
        
        var grindCoffeeBeansStepId = IdHelper.CreateStepId(makeCoffeeJobId);
        var boilWaterStepId = IdHelper.CreateStepId(makeCoffeeJobId);
        var brewCoffeeStepId = IdHelper.CreateStepId(makeCoffeeJobId);
        var toastBreadStepId = IdHelper.CreateStepId(makeToastJobId);
        var fryEggsStepId = IdHelper.CreateStepId(makeEggsJobId);
        var fryBaconStepId = IdHelper.CreateStepId(makeBaconJobId);
        var serveBreakfastStepId = IdHelper.CreateStepId(serveBreakfastJobId);
        
        var myIds = new MyTestIds
        {
            Workflow = workflowId,
            MakeCoffeeJob = makeCoffeeJobId,
            MakeToastJob = makeToastJobId,
            MakeEggsJob = makeEggsJobId,
            MakeBaconJob = makeBaconJobId,
            ServeBreakfastJob = serveBreakfastJobId,
            GrindCoffeeBeansStep = grindCoffeeBeansStepId,
            BoilWaterStep = boilWaterStepId,
            BrewCoffeeStep = brewCoffeeStepId,
            ToastBreadStep = toastBreadStepId,
            FryEggsStep = fryEggsStepId,
            FryBaconStep = fryBaconStepId,
            ServeBreakfastStep = serveBreakfastStepId
        };
        
        _outputHelper.WriteCSharp(myIds);
    }
    
    private class MyTestIds
    {
        public static MyTestIds Instance { get; } = new()
        {
            BoilWaterStep = new System.Guid("163445d6-9092-4a37-0000-000000000000"),
            BrewCoffeeStep = new System.Guid("163445d6-9092-4e96-0000-000000000000"),
            FryBaconStep = new System.Guid("163445d6-8e9f-4703-0000-000000000000"),
            FryEggsStep = new System.Guid("163445d6-2c20-4df3-0000-000000000000"),
            GrindCoffeeBeansStep = new System.Guid("163445d6-9092-4821-0000-000000000000"),
            MakeBaconJob = new System.Guid("163445d6-8e9f-0000-0000-000000000000"),
            MakeCoffeeJob = new System.Guid("163445d6-9092-0000-0000-000000000000"),
            MakeEggsJob = new System.Guid("163445d6-2c20-0000-0000-000000000000"),
            MakeToastJob = new System.Guid("163445d6-5610-0000-0000-000000000000"),
            ServeBreakfastJob = new System.Guid("163445d6-4f61-0000-0000-000000000000"),
            ServeBreakfastStep = new System.Guid("163445d6-4f61-4f07-0000-000000000000"),
            ToastBreadStep = new System.Guid("163445d6-5610-437f-0000-000000000000"),
            Workflow = new System.Guid("163445d6-0000-0000-0000-000000000000")
        };
        
        public Guid Workflow { get; set; }
        
        public Guid MakeCoffeeJob { get; set; }
        
        public Guid MakeToastJob { get; set; }
        
        public Guid MakeEggsJob { get; set; }
        
        public Guid MakeBaconJob { get; set; }
        
        public Guid ServeBreakfastJob { get; set; }
        
        // steps
        public Guid GrindCoffeeBeansStep { get; set; }
        
        public Guid BoilWaterStep { get; set; }
        
        public Guid BrewCoffeeStep { get; set; }
        
        public Guid ToastBreadStep { get; set; }
        
        public Guid FryEggsStep { get; set; }
        
        public Guid FryBaconStep { get; set; }
        
        public Guid ServeBreakfastStep { get; set; }
    }
    
    private class Workflow : IWorkflow
    {
        public Guid Id => MyTestIds.Instance.Workflow;
        public string Name => "Breakfast";
        public string? Description { get; set; }

        public JobContext CreateJobContext()
        {
            var jobContext = new JobContext
            {
                
            };
            return jobContext;
        }

        public ITrigger? Trigger => new CronTrigger("0 0 6 * * *");
    }

    private class MakeCoffeeJob : IJob
    {
        public Guid Id => MyTestIds.Instance.MakeCoffeeJob;
        public string Name => "Make coffee";
        public string? Description { get; set; }
        public Guid WorkflowId => MyTestIds.Instance.Workflow;
        
        public async Task<JobResult> ExecuteAsync(JobContext context)
        {
            var stepResults = new List<StepResult>();
            foreach (var step in context.Steps)
            {
                var stepInput = new StepInput
                {
                    WorkflowId = WorkflowId,
                    JobId = Id,
                    StepId = step.Id
                };
                var stepResult = await step.ExecuteAsync(stepInput);
                stepResults.Add(stepResult);
            }

            return new JobResult(stepResults);
        }
    }
    
    private class GrindCoffeeBeansStep : IStep
    {
        public Guid Id => MyTestIds.Instance.GrindCoffeeBeansStep;
        public string Name => "Grind coffee beans";
        public string? Description { get; set; }
        public Guid JobId => MyTestIds.Instance.MakeCoffeeJob;
        public int SortOrder => 1;
        
        public async Task<StepResult> ExecuteAsync(StepInput input)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return new StepResult();
        }
    }
    
    private class BoilWaterStep : IStep
    {
        public Guid Id => MyTestIds.Instance.BoilWaterStep;
        public string Name => "Boil water";
        public string? Description { get; set; }
        public Guid JobId => MyTestIds.Instance.MakeCoffeeJob;
        public int SortOrder => 2;
        
        public async Task<StepResult> ExecuteAsync(StepInput input)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return new StepResult();
        }
    }
    
    private class BrewCoffeeStep : IStep
    {
        public Guid Id => MyTestIds.Instance.BrewCoffeeStep;
        public string Name => "Brew coffee";
        public string? Description { get; set; }
        public Guid JobId => MyTestIds.Instance.MakeCoffeeJob;
        public int SortOrder => 3;
        
        public async Task<StepResult> ExecuteAsync(StepInput input)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return new StepResult();
        }
    }
    
    private class MakeToastJob : IJob
    {
        public Guid Id => MyTestIds.Instance.MakeToastJob;
        public string Name => "Make toast";
        public string? Description { get; set; }
        public Guid WorkflowId => MyTestIds.Instance.Workflow;
        
        public async Task<JobResult> ExecuteAsync(JobContext context)
        {
            var stepResults = new List<StepResult>();
            foreach (var step in context.Steps)
            {
                var stepInput = new StepInput
                {
                    WorkflowId = WorkflowId,
                    JobId = Id,
                    StepId = step.Id
                };
                var stepResult = await step.ExecuteAsync(stepInput);
                stepResults.Add(stepResult);
            }

            return new JobResult(stepResults);
        }
    }
    
    private class ToastBreadStep : IStep
    {
        public Guid Id => MyTestIds.Instance.ToastBreadStep;
        public string Name => "Toast bread";
        public string? Description { get; set; }
        public Guid JobId => MyTestIds.Instance.MakeToastJob;
        public int SortOrder => 1;
        
        public async Task<StepResult> ExecuteAsync(StepInput input)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return new StepResult();
        }
    }
    
    private class MakeEggsJob : IJob
    {
        public Guid Id => MyTestIds.Instance.MakeEggsJob;
        public string Name => "Make eggs";
        public string? Description { get; set; }
        public Guid WorkflowId => MyTestIds.Instance.Workflow;
        
        public async Task<JobResult> ExecuteAsync(JobContext context)
        {
            var stepResults = new List<StepResult>();
            foreach (var step in context.Steps)
            {
                var stepInput = new StepInput
                {
                    WorkflowId = WorkflowId,
                    JobId = Id,
                    StepId = step.Id
                };
                var stepResult = await step.ExecuteAsync(stepInput);
                stepResults.Add(stepResult);
            }

            return new JobResult(stepResults);
        }
    }
    
    private class FryEggsStep : IStep
    {
        public Guid Id => MyTestIds.Instance.FryEggsStep;
        public string Name => "Fry eggs";
        public string? Description { get; set; }
        public Guid JobId => MyTestIds.Instance.MakeEggsJob;
        public int SortOrder => 1;
        
        public async Task<StepResult> ExecuteAsync(StepInput input)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return new StepResult();
        }
    }
    
    private class MakeBaconJob : IJob
    {
        public Guid Id => MyTestIds.Instance.MakeBaconJob;
        public string Name => "Make bacon";
        public string? Description { get; set; }
        public Guid WorkflowId => MyTestIds.Instance.Workflow;
        
        public async Task<JobResult> ExecuteAsync(JobContext context)
        {
            var stepResults = new List<StepResult>();
            foreach (var step in context.Steps)
            {
                var stepInput = new StepInput
                {
                    WorkflowId = WorkflowId,
                    JobId = Id,
                    StepId = step.Id
                };
                var stepResult = await step.ExecuteAsync(stepInput);
                stepResults.Add(stepResult);
            }

            return new JobResult(stepResults);
        }
    }
    
    private class FryBaconStep : IStep
    {
        public Guid Id => MyTestIds.Instance.FryBaconStep;
        public string Name => "Fry bacon";
        public string? Description { get; set; }
        public Guid JobId => MyTestIds.Instance.MakeBaconJob;
        public int SortOrder => 1;
        
        public async Task<StepResult> ExecuteAsync(StepInput input)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return new StepResult();
        }
    }
    
    private class ServeBreakfastJob : IJob
    {
        public Guid Id => MyTestIds.Instance.ServeBreakfastJob;
        public string Name => "Serve breakfast";
        public string? Description { get; set; }
        public Guid WorkflowId => MyTestIds.Instance.Workflow;
        
        public async Task<JobResult> ExecuteAsync(JobContext context)
        {
            var stepResults = new List<StepResult>();
            foreach (var step in context.Steps)
            {
                var stepInput = new StepInput
                {
                    WorkflowId = WorkflowId,
                    JobId = Id,
                    StepId = step.Id
                };
                var stepResult = await step.ExecuteAsync(stepInput);
                stepResults.Add(stepResult);
            }

            return new JobResult(stepResults);
        }
    }
    
    private class ServeBreakfastStep : IStep
    {
        public Guid Id => MyTestIds.Instance.ServeBreakfastStep;
        public string Name => "Serve breakfast";
        public string? Description { get; set; }
        public Guid JobId => MyTestIds.Instance.ServeBreakfastJob;
        public int SortOrder => 1;
        
        public async Task<StepResult> ExecuteAsync(StepInput input)
        {
            await Task.Delay(TimeSpan.FromSeconds(1));
            return new StepResult();
        }
    }
    
    
}

internal class CronTrigger : ITrigger
{
    private readonly string _cronExpression;
    
    public CronTrigger(string cronExpression)
    {
        _cronExpression = cronExpression;
    }
    
    public bool Evaluate(DateTime triggerTime)
    {
        return true;
    }
}