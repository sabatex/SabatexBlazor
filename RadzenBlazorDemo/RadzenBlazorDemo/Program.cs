using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RadzenBlazorDemo.Client.Pages;
using RadzenBlazorDemo.Components;
using RadzenBlazorDemo.Data;
using Sabatex.Core.Identity;
using Sabatex.RadzenBlazor;
using Sabatex.RadzenBlazor.Server;

namespace RadzenBlazorDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("MemoryTestDataBase"));
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


            builder.Services.AddSingleton<Sabatex.Core.IEmailSender<ApplicationUser>, IdentityEmailSender>();
            builder.Services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            builder.Services.AddSabatexRadzenBlazor();
            builder.Services.AddScoped<IIdentityAdapter,IdentityAdapterServer>();
            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>();
            builder.Services.AddScoped<AuthenticationStateProvider, PersistingServerAuthenticationStateProvider>();

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
                .AddAdditionalAssemblies(typeof(Client._Imports).Assembly,typeof(Sabatex.RadzenBlazor._Imports).Assembly);

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }
    }
}
