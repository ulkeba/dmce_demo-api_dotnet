using WeatherForecastAPI.Services;

using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

const String MeterName = "WeatherForecastAPI";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var oidcAudience = Environment.GetEnvironmentVariable("OIDC_AUDIENCE");
if (string.IsNullOrEmpty(oidcAudience))
    throw new Exception("Environment variable OIDC_AUDIENCE has not been set.");
var oidcAuthority = Environment.GetEnvironmentVariable("OIDC_AUTHORITY");
if (string.IsNullOrEmpty(oidcAuthority))
    throw new Exception("Environment variable OIDC_AUTHORITY has not been set.");
    
builder.Services.AddAuthentication().AddJwtBearer(options => 
{
    options.Audience = oidcAudience;
    options.Authority = oidcAuthority;
});

builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHostedService<TimedHostedService>();

builder.Services.AddSingleton<Meter>(s => new Meter(MeterName));

builder.Services.AddOpenTelemetryMetrics(b =>
{
    b.AddPrometheusExporter(options =>
        {
            options.ScrapeResponseCacheDurationMilliseconds = 0;
        })
        .AddRuntimeInstrumentation()
        .AddMeter(MeterName);
});

if (!builder.Environment.IsDevelopment())
{    
    builder.Host.ConfigureLogging((_, logging) =>
        {
            logging.AddJsonConsole();
        });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();
