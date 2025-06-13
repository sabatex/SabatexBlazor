using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Server;

public interface ICommandLineOperations
{
    /// <summary>
    /// Run migrate  database
    /// </summary>
    /// <returns></returns>
    Task MigrateAsync();
    /// <summary>
    /// Garand user admin role
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    Task GrandUserAdminRoleAsync(string userName);
    Task InitialRolesAsync();
    Task InitialDemoDataAsync();

} 

