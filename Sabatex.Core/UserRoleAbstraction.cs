using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UserRoleAbstraction
    {
        /// <summary>
        /// Administrator role
        /// </summary>
        public const string Administrator = "Administrator";
        /// <summary>
        /// Get Avaliable roles
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<string> GetAvaliableRoles();
    }
}
