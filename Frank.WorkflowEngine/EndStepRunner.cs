using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace Frank.WorkflowEngine;

public class EndStepRunner<T>(ChannelReader<T> reader, IEndStep<T> action) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in reader.ReadAllAsync(stoppingToken))
        {
            await action.ExecuteAsync(item);
        }
    }
}