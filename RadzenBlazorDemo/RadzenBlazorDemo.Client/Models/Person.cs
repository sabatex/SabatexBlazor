using Sabatex.Core;

namespace RadzenBlazorDemo.Client.Models;

public class Person:IEntityBase<Guid>
{
    public string Name { get; set; }
    public Guid Id {get; set; }
}
