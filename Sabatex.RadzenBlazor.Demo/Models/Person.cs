using Sabatex.Core;

namespace Sabatex.RadzenBlazor.Demo.Models;

public class Person:IEntityBase<Guid>
{
    public string Name { get; set; }
    public Guid Id {get; set; }
}
