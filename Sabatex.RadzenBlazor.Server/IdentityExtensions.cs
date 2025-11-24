using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor.Server;

/// <summary>
/// Extended Identity
/// </summary>
public static class IdentityExtensions
{
    /// <summary>
    /// Represents the action name used for login callback operations.
    /// </summary>
    public const string LoginCallbackAction = "LoginCallback";
    /// <summary>
    /// Represents the action name used for the link login callback in authentication workflows.
    /// </summary>
    /// <remarks>This constant is typically used to identify the callback endpoint when linking external login
    /// providers. It can be referenced in routing or controller logic to ensure consistency across authentication
    /// operations.</remarks>
    public const string LinkLoginCallbackAction = "LinkLoginCallback";


    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddIsConfiguredGoogle(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        var clientId = configuration["Authentication:Google:ClientId"];
        var clientSecret = configuration["Authentication:Google:ClientSecret"];
        if (clientId != null && clientSecret != null)
        {
            builder.Services.AddAuthentication().AddGoogle(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });
        }
        return builder;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddIsConfiguredMicrosoft(this AuthenticationBuilder builder, IConfiguration configuration)
    {
        var clientId = configuration["Authentication:Microsoft:ClientId"];
        var clientSecret = configuration["Authentication:Microsoft:ClientSecret"];
        if (clientId != null && clientSecret != null)
        {
            builder.Services.AddAuthentication().AddMicrosoftAccount(options =>
            {
                options.ClientId = clientId;
                options.ClientSecret = clientSecret;
            });
        }
        return builder;
    }
    /// <summary>
    /// Adds a user-specific JSON configuration file to the specified <see cref="ConfigurationManager"/> instance if the
    /// file exists.
    /// </summary>
    /// <remarks>If the specified configuration file does not exist, no changes are made to the <see
    /// cref="ConfigurationManager"/>. The configuration file is added as optional and will not reload on
    /// change.</remarks>
    /// <param name="manager">The <see cref="ConfigurationManager"/> instance to which the user configuration file will be added.</param>
    /// <param name="userFileConfig">The name of the user configuration file to add, located in the '/etc/sabatex/' directory. Cannot be null.</param>
    /// <returns>The <see cref="ConfigurationManager"/> instance with the user configuration file added if the file exists;
    /// otherwise, the original instance.</returns>
    public static ConfigurationManager AddUserConfiguration(this ConfigurationManager manager, string userFileConfig)
    {
        var confogFileName = $"/etc/sabatex/{userFileConfig}";
        if (File.Exists(confogFileName))
            manager.AddJsonFile(confogFileName, optional: true, reloadOnChange: false);
        return manager;
    }
    /// <summary>
    /// Adds user-specific configuration to the specified <see cref="ConfigurationManager"/> instance using the current
    /// project's name.
    /// </summary>
    /// <remarks>This method is intended for use in extension scenarios where user-specific configuration
    /// should be loaded based on the executing project's name. If called multiple times, user configuration may be
    /// merged or overwritten depending on the underlying implementation of <see
    /// cref="ConfigurationManager.AddUserConfiguration(string)"/>.</remarks>
    /// <param name="manager">The <see cref="ConfigurationManager"/> instance to which the user configuration will be added. Cannot be null.</param>
    /// <returns>The <see cref="ConfigurationManager"/> instance with user configuration added for the current project.</returns>
    public static ConfigurationManager AddUserConfiguration(this ConfigurationManager manager)
    {
        Assembly assembly = Assembly.GetExecutingAssembly(); // Retrieve the project name 
        string projectName = assembly.GetName().Name;
        return manager.AddUserConfiguration(projectName);
    }

    /// <summary>
    /// Get or create role
    /// </summary>
    /// <param name="roleManager">RoleManager</param>
    /// <param name="roleName">Role name</param>
    /// <returns>object IdentityRole </returns>
    /// <exception cref="Exception"></exception>
    public static async Task<IdentityRole> GetOrCreateRoleAsync(this RoleManager<IdentityRole> roleManager, string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded == false)
                throw new Exception($"Error! Do not create {roleName} role!");
            role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                throw new Exception($"Error! Do not get {roleName} role after create !");
            }
        }
        return role;

    }
    /// <summary>
    /// Get or create user
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="userName">user email</param>
    /// <param name="FullName">Frendly user name</param>
    /// <param name="password">user password </param>
    /// <param name="roleNames">user role</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<ApplicationUser> GetOrCreateUserAsync(this UserManager<ApplicationUser> userManager, string userName, string FullName,string password, string? roleNames = null)
    {
        var user = await userManager.FindByEmailAsync(userName);
        if (user == null)
        {
            user = new ApplicationUser
            {
                Email = userName,
                UserName = userName,
                FullName = FullName
            };

            IdentityResult result;
            result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create user");
            }
            if (!string.IsNullOrEmpty(roleNames))
            {
                var roles = roleNames.Split(new char[] { ',', ';' });
                foreach (var roleName in roles)
                {
                    var role = await userManager.GetRolesAsync(user);
                    if (!role.Contains(roleName))
                    {
                        result = await userManager.AddToRoleAsync(user, roleName);
                        if (!result.Succeeded)
                        {
                            throw new Exception($"Failed to add role {roleName} to user");
                        }
                    }
                }

            }
        }
        return user;
    }
    /// <summary>
    /// Grand user role Administrator if user in roles Administrator not exist
    /// </summary>
    /// <param name="userManager"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task GrandUserAdminRoleAsync(this UserManager<ApplicationUser> userManager, string userName)
    {
        var user = await userManager.FindByEmailAsync(userName);
        if (user == null)
        {
            throw new Exception("Failed to find user");
        }

        if (!await userManager.IsInRoleAsync(user,IUserAdminRole.Administrator))
        {
           var resultAddRole = await userManager.AddToRoleAsync(user, IUserAdminRole.Administrator);
            if (!resultAddRole.Succeeded)
            {
                throw new Exception("Failed to add role Administrator to user");
            }

        }
    }


    /// <summary>
    /// add sabatex identity UI services
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddSabatexIdentityUI(this IServiceCollection services)
    //    where TDBContext : IdentityDbContext
    //    where TCmd : CommandLineOperations<TDBContext>
    {
        //    services.AddDbContext<TDBContext>();
        //    services.AddIdentity<ApplicationUser, IdentityRole>()
        //        .AddEntityFrameworkStores<TDBContext>()
        //        .AddDefaultTokenProviders();
        //    services.AddScoped<ICommandLineOperations,TCmd>();
        return services;
    }

}