using System.Diagnostics.Metrics;

namespace WeatherForecastAPI.Services;

public class TimedHostedService : IHostedService, IDisposable
{
    private Timer? _timer = null;
    private int executionCount = 0;

    private Counter<int> counter;
    private ObservableGauge<double> gauge;

    public readonly String[] DEMO_TAG_VALUES = { "value-a", "value-b", "value-c" };
    private Random random = new Random();

    private readonly ILogger<TimedHostedService> _logger;

    public TimedHostedService(ILogger<TimedHostedService> logger, Meter appMeter)
    {
        _logger = logger;
        counter = appMeter.CreateCounter<int>("some-counter");
        gauge = appMeter.CreateObservableGauge<double>("some-gauge", () =>
        {
            double nowInMs = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            double scaled = (nowInMs / (10 * 1000f));
            return Math.Sin(scaled);
        });
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(2500));
        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var count = Interlocked.Increment(ref executionCount);

        int r = random.Next(DEMO_TAG_VALUES.Length);
        int increment = random.Next((r + 1) * 5);
        counter.Add(increment, new KeyValuePair<string, object?>("demo-tag", DEMO_TAG_VALUES[r]));

        _logger.LogInformation("Timed Hosted Service is working. Count: {Count}", count);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service is stopping.");
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}