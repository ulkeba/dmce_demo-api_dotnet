using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace WeatherForecastAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class VersionInfoController : ControllerBase
{

    private readonly ILogger<VersionInfoController> _logger;

    private static Lazy<string> lazyVersion = new Lazy<string>(InitializeVersion);
    private static Lazy<string> lazyEnvironmentName = new Lazy<string>(InitializeEnvironmentName);

    private static string InitializeVersion()
    {
        string? retVal = Environment.GetEnvironmentVariable("API_VERSION");
        return retVal != null ? retVal : "<unknown>";
    }

    private static string InitializeEnvironmentName()
    {
        string? retVal = Environment.GetEnvironmentVariable("ENVIRONMENT_NAME");
        return retVal != null ? retVal : "<unknown>";
    }
    
    public VersionInfoController(ILogger<VersionInfoController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetVersionInfo")]
    public IActionResult Get()
    {
        return Ok(new
        {
            version = lazyVersion.Value,
            environment = lazyEnvironmentName.Value
        });
    }
}   