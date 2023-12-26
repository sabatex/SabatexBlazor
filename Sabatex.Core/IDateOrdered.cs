using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core;
/// <summary>
/// object ordered by Date 
/// </summary>
public interface IDateOrdered
{
    /// <summary>
    /// The date 
    /// </summary>
    public DateTime Date { get; set; }
}

