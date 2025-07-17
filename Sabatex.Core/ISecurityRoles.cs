using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.Core
{
    /// <summary>
    /// Defines the contract for managing security roles within the application.
    /// </summary>
    /// <remarks>This interface extends <see cref="IUserAdminRole"/> to provide additional functionality 
    /// specific to security roles. It includes constants and methods for working with roles  in the
    /// application.</remarks>
    public interface ISecurityRoles:IUserAdminRole
    {
        /// <summary>
        /// Represents the name of the role manager used in the application.
        /// </summary>
        const string RoleManager = "RoleManager";

    }
}
