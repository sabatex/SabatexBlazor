﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Sabatex.RadzenBlazor.Server;

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
    /// <param name="app"></param>
    /// <param name="args"></param>
    /// <param name="setAdminPrivilegeAsync"></param>
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

        var initial = parseResult.GetValue<bool>("--initial");
        if (initial)
        {
            if (initialDatabaseAsync != null)
                await initialDatabaseAsync();
        }

        app.Run();



    }

}
