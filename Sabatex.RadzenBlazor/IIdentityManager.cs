using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    internal interface IIdentityManager
    {
        /// <summary>
        /// Uses the current identity to get a user-friendly name.
        /// </summary>
        /// <returns></returns>
        Task<string> GetUserFriendlyNameAsync();
    }
}
