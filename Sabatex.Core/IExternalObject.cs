using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;

public interface IExternalObject
{
    /// <summary>
    /// mark object as external 
    /// </summary>
    public bool IsExternal { get; set; }
    /// <summary>
    /// External Id (number,string,uuid)
    /// indexed for exchange
    /// </summary>
    public string ExternalId { get; set; }
}
