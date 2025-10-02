using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;
/// <summary>
/// Object have index by Name  
/// </summary>

public interface IEntityFieldDescription
{
    /// <summary>
    /// Field name (person,item...)
    /// </summary>
    string Description { get; set; }
}

