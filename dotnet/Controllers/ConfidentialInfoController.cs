using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WeatherForecastAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ConfidentialInfoController : ControllerBase
{

    private IHttpContextAccessor httpContextAccessor;

    private String LogInUser = "<undefined>";

    public ConfidentialInfoController(IHttpContextAccessor _httpContextAccessor) 
    {
        this.httpContextAccessor = _httpContextAccessor;
        this.LogInUser = this.httpContextAccessor.HttpContext.User.Identity.Name;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var scopes = this.httpContextAccessor.HttpContext.User.Claims.Where(c => c.Type.Equals("http://schemas.microsoft.com/identity/claims/scope")).Select(c => c.Value).ToList();
        return Ok(new
        {
            confidentialInfo = "Don't tell anyone!",
            logInUser = LogInUser,
            scopes = scopes
        });
    }
}
