using Sabatex.Core;

namespace RadzenBlazorDemo.Client.Models;

public class WeatherForecast : IEntityBase<Guid>
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
