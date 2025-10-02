using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;
/// <summary>
/// Represents an object that can be identified as external and associated with an external identifier.
/// </summary>
/// <remarks>This interface is typically implemented by objects that need to be marked as external and linked to
/// an external system or data source using a unique identifier.</remarks>
public interface IExternalObjectIdentifier
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
