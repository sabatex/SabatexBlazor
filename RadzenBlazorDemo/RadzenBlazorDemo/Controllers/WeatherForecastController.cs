using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadzenBlazorDemo.Client.Models;
using RadzenBlazorDemo.Data;
using Sabatex.Core.RadzenBlazor;
using Sabatex.RadzenBlazor;
using Sabatex.RadzenBlazor.Server;
using System.Security.Claims;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sabatex.Bakery.Controllers
{
    [AllowAnonymous]
    public class WeatherForecastController : BaseController<WeatherForecast>
    {
        public WeatherForecastController(ApplicationDbContext context, ILogger<WeatherForecastController> logger) : base(context, logger)
        {
        }
        public override Task<QueryResult<WeatherForecast>> Get([FromBody] QueryParams queryParams)
        {
            return base.Get(queryParams);
        }
        protected override async Task<bool> CheckAccess(WeatherForecast item, WeatherForecast? updated)
        {
            await Task.Yield();
            return true;
        }

    }
}
