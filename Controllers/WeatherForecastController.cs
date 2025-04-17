using Microsoft.AspNetCore.Mvc;

namespace TempModbusProject.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet(Name = "Login")]
    public void login()
    {
        Console.WriteLine("一切的开始");
    }
}
