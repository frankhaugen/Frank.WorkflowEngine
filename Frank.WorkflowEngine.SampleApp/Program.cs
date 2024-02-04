using System.Text.Json;
using System.Threading.Channels;
using Frank.WorkflowEngine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");

var builder = new HostBuilder()
    .UseConsoleLifetime()
    .ConfigureLogging(logging =>
    {
        logging.AddDebug();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Debug);
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHttpClient();
        services.AddWorkflow(builder =>
        {
            builder
                .StartWith<GetTimeZonesStep, string>()
                .Then<ParseRawTimeZonesStep, string, StringList>()
                .Then<ParseTimeZonesStep, StringList, TimezoneList>()
                .ThenEndWith<SaveTimeZonesStep, TimezoneList>();
        });
    });

var host = builder.Build();

await host.RunAsync();
    
public class GetTimeZonesStep(IHttpClientFactory httpClientFactory, ChannelWriter<string> writer) : ITriggerStep<string>
{
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        var response = await httpClientFactory.CreateClient().GetAsync("https://worldtimeapi.org/api/timezone", cancellationToken);
        var timezones = await response.Content.ReadAsStringAsync(cancellationToken);
        await writer.WriteAsync(timezones, cancellationToken);
    }
}

public class ParseRawTimeZonesStep : IStep<string, StringList>
{
    public async Task<StringList> ExecuteAsync(string input) => await Task.FromResult(JsonSerializer.Deserialize<StringList>(input) ?? throw new InvalidOperationException("Failed to parse timezones"));
}

public class ParseTimeZonesStep : IStep<StringList, TimezoneList>
{
    public async Task<TimezoneList> ExecuteAsync(StringList input) =>
        await Task.FromResult(new TimezoneList(input.Select(timezone => timezone.Split('/').Select(part => part.Trim()).ToArray()).Select(parts => parts switch
        {
            { Length: 1 } => new Timezone(parts[0], null, null),
            { Length: 2 } => new Timezone(parts[0], parts[1], null),
            _ => new Timezone(parts[0], parts[1], parts[2])
        })));
}

public class SaveTimeZonesStep : IEndStep<TimezoneList>
{
    public async Task ExecuteAsync(TimezoneList input)
    {
        await File.WriteAllTextAsync("timezones.json", JsonSerializer.Serialize(input, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        await Task.Delay(1000);
        Environment.Exit(0);
    }
}

public readonly record struct Timezone(string Continent, string? Region, string? City);

public class StringList : List<string>;
public class TimezoneList(IEnumerable<Timezone> timezones) : List<Timezone>(timezones);