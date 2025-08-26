using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RadzenBlazorDemo.Client.Pages;
using RadzenBlazorDemo.Components;
using RadzenBlazorDemo.Data;
using RadzenBlazorDemo.Services;
using Sabatex.Core.Identity;
using Sabatex.Core.RadzenBlazor;
using Sabatex.RadzenBlazor;
using Sabatex.RadzenBlazor.Server;

            var builder = WebApplication.CreateBuilder(args);

            //var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddRoles<IdentityRole>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents()
                .AddAuthenticationStateSerialization();

            builder.Services.AddCascadingAuthenticationState();
            

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIsConfiguredMicrosoft(builder.Configuration)
                .AddIsConfiguredGoogle(builder.Configuration);
            builder.Services.AddAuthorization();


            builder.Services.AddSingleton<Sabatex.Core.Identity.IEmailSender<ApplicationUser>, IdentityEmailSender>();
            builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            builder.Services.AddControllers();
            builder.Services.AddSabatexRadzenBlazor();
            builder.Services.AddScoped<IIdentityAdapter,IdentityAdapterServer>();
            builder.Services.AddScoped<ISabatexRadzenBlazorDataAdapter<Guid>, SabatexServerRadzenBlazorODataAdapter>();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
            builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }
                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.UseRequestLocalization(
                new RequestLocalizationOptions() { ApplyCurrentCultureToResponseHeaders = true }
                .AddSupportedCultures(new[] { "en-US", "uk-UA" })
                .AddSupportedUICultures(new[] { "en-US", "uk-UA" })
                .SetDefaultCulture("uk-UA")
                );

            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(typeof(RadzenBlazorDemo.Client._Imports).Assembly,typeof(Sabatex.RadzenBlazor._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();
            app.MapControllers();
            await app.RunAsync(args,
                async () => 
                {
                    var serviceProvider = app.Services.CreateScope().ServiceProvider;
                    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    var adminRole = await roleManager.GetOrCreateRoleAsync(Sabatex.Core.ISecurityRoles.Administrator);
                    var userRole =  await roleManager.GetOrCreateRoleAsync("ApplicationUser");

                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                    var admin = await userManager.GetOrCreateUserAsync("testAdmin@mail.com", "Test Admin", "Aa1234567890-");
                    var user = await userManager.GetOrCreateUserAsync("testUser@mail.com", "Test User", "Aa1234567890-");
                },
                async (string userName) => 
                {
                    var serviceProvider = app.Services.CreateScope().ServiceProvider;
                    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    await userManager.GrandUserAdminRoleAsync(userName);
                },
                async () => 
                {
                    var serviceProvider = app.Services.CreateScope().ServiceProvider;
                    var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                    //await dbContext.Database.MigrateAsync();

                });
