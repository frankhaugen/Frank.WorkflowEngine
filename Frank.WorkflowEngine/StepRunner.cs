using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Frank.WorkflowEngine;

public class StepRunner<TIn, TOut>(ChannelReader<TIn> reader, ChannelWriter<TOut> writer, IStep<TIn, TOut> action, ILogger<StepRunner<TIn, TOut>> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var item in reader.ReadAllAsync(stoppingToken))
        {
            logger.LogInformation("StepRunner received input {Input}", item);
            await writer.WriteAsync(await action.ExecuteAsync(item), stoppingToken);
        }
    }
}