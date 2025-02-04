using Microsoft.AspNetCore.Mvc;

namespace TelegramIfood.API.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;
    public HealthCheckController(ILogger<HealthCheckController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult HealthCheck()
    {
        
        var result = $"Healthy {DateTime.Now}";

        _logger.LogInformation(result);

        return Ok(result);
    }
}