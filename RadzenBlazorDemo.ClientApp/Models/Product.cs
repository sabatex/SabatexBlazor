using Sabatex.Core;
namespace RadzenBlazorDemo.ClientApp.Models;

public class Product: IEntityBase<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
}