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
    public const string LoginCallbackAction = "LoginCallback";
    public const string LinkLoginCallbackAction = "LinkLoginCallback";


    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static AuthenticationBuilder AddIsConfiguredGoogle(this AuthenticationBuilder builder,IConfiguration configuration)
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
    /// 
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="userFileConfig"></param>
    /// <returns></returns>
    public static ConfigurationManager AddUserConfiguration(this ConfigurationManager manager,string userFileConfig)
    {
        var confogFileName = $"/etc/sabatex/{userFileConfig}";
        if (File.Exists(confogFileName))
            manager.AddJsonFile(confogFileName,optional:true,reloadOnChange:false);
        return manager;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="manager"></param>
    /// <param name="userFileConfig"></param>
    /// <returns></returns>
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
    public static async Task<IdentityRole> GetOrCreateRoleAsync(this RoleManager<IdentityRole> roleManager,string roleName)
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
    /// <param name="password">user password</param>
    /// <param name="roleName">user role</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<ApplicationUser> GetOrCreateUserAsync(this UserManager<ApplicationUser> userManager, string userName, string password,  string roleName)
    {
        var user = await userManager.FindByEmailAsync(userName);
        if (user == null)
        {
            user = new ApplicationUser
            {
                Email = userName,
                    UserName = userName
                };

                IdentityResult result;
                result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to create user");
                }
                // grand user role
                result = await userManager.AddToRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    throw new Exception("Failed to add role ClientUser to user");
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
 
        var usersInRole = await userManager.GetUsersInRoleAsync(IUserAdminRole.Administrator);
        if (usersInRole.Count() == 0)
        {
            var resultAddRole = await userManager.AddToRoleAsync(user, IUserAdminRole.Administrator);
            if (!resultAddRole.Succeeded)
            {
                throw new Exception("Failed to add role Administrator to user");
            }
        }
        else
        {
            throw new Exception("User already has role Administrator");
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




    /// <summary>
    /// Run web application with command line arguments
    /// </summary>
    /// <param name="app"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static async Task RunAsync(this WebApplication app, string[] args)
    {
        var cmd = app.Services.CreateScope().ServiceProvider.GetRequiredService<ICommandLineOperations>();
        foreach (var arg in args)
        {
            if (arg == "--migrate")
            {
                await cmd.MigrateAsync();
                return;
            }

            if (arg.StartsWith("--admin"))
            {
                var user = arg.Replace("--admin", string.Empty);
                await cmd.GrandUserAdminRoleAsync(user);
                return;
            }

            if (arg == "--initialDemo")
            {
                await cmd.InitialDemoDataAsync();
                return;
            }
        }
        app.Run();

    }


}
