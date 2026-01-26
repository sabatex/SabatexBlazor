using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.CommandLine;

namespace Sabatex.RadzenBlazor.Server;
/// <summary>
/// Provides extension methods for configuring and running a web application with support for command-line arguments,
/// such as database migration, initial data seeding, and assigning administrative privileges.
/// </summary>
/// <remarks>These extensions enable integration of command-line operations into the application's startup
/// process, allowing for tasks like database migration or user role assignment to be triggered via command-line
/// options. This is particularly useful for automating setup and maintenance tasks in development or deployment
/// scenarios.</remarks>
public static class WebApplicationExtensions
{


    static RootCommand InitialCMD()
    {
        Option<bool> migrateOption = new("--migrate", new string[] { "-m" })
        {
            Description = "The migrate database after on start application."
        };
        var admin = new Option<string>("--admin", "-a")
        {
            Description = "Grand user admin privileges."
        };

        var initial = new Option<bool>("--initial", "-i")
        {
            Description = "Initial database with default data."
        };

        RootCommand rootCommand = new("Sabatex Blazor command line args")
        {
            migrateOption,
            admin,
            initial
        };

        return rootCommand;
    }




    /// <summary>
    /// Run web application with command line arguments
    /// </summary>
    /// <param name="app">Application context</param>
    /// <param name="args">Command line arguments</param>
    /// <param name="setAdminPrivilegeAsync">CallBack for setting admin privileges</param>
    /// <param name="initialDatabaseAsync">CallBack for initial database setup</param>
    /// <param name="migrateAsync">CallBack for database migration</param>
    /// <returns></returns>
    public static async Task RunAsync(this WebApplication app, string[] args, Func<Task>? initialDatabaseAsync = null, Func<string,Task>? setAdminPrivilegeAsync=null,Func<Task>? migrateAsync = null)
    {
        var serviceProvider = app.Services.CreateScope().ServiceProvider;
        //var cmd = serviceProvider.GetRequiredService<ICommandLineOperations>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var rootCommand = InitialCMD();
        var parseResult = rootCommand.Parse(args);

        if (parseResult.Errors.Count > 0)
        {
            Environment.Exit(exitCode: 1);
        }
        var migrateRequest =  parseResult.GetValue<bool>("--migrate");
        if (migrateRequest)
        {
            if (migrateAsync != null)
                await migrateAsync();
        }

 
        var initial = parseResult.GetValue<bool>("--initial");
        if (initial)
        {
            if (initialDatabaseAsync != null)
                await initialDatabaseAsync();
        } 
        
        var admin = parseResult.GetValue<string>("--admin");
        if (!string.IsNullOrEmpty(admin))
        {
            if (setAdminPrivilegeAsync != null)
                await setAdminPrivilegeAsync(admin);
            else
            {
                await userManager.GrandUserAdminRoleAsync(admin);
            }
        }


        app.Run();



    }

    /// <summary>
    /// Determines whether the current HTTP request path matches any registered WebAssembly (WASM) client route.
    /// </summary>
    /// <remarks>Use this method to identify requests that should be handled by a WASM client route, such as
    /// for routing or middleware decisions.</remarks>
    /// <param name="httpContext">The HTTP context for the current request. Cannot be null.</param>
    /// <returns>true if the request path starts with the prefix of any registered WASM client route; otherwise, false.</returns>
    public static bool IsRouteWASMClient(this HttpContext httpContext)
    {
        foreach (var WASMClient in RadzenBlazor.WASMClient.WASMClients)
        {
            if (httpContext.Request.Path.StartsWithSegments(WASMClient.PrefixRoute))
            {
                return true;
            }
        }
        return false;
    }


    

}
