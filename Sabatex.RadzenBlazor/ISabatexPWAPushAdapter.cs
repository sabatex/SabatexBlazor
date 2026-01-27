using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    /// <summary>
    /// Defines a contract for retrieving the public key used for push notifications in a Progressive Web App (PWA)
    /// context.
    /// </summary>
    public interface ISabatexPWAPushAdapter
    {
        /// <summary>
        /// Retrieves the public key associated with the current instance.
        /// </summary>
        /// <returns>A string containing the public key in a standard encoded format. The string is empty if no public key is
        /// available.</returns>
        string GetPublicKey();

    }
}
