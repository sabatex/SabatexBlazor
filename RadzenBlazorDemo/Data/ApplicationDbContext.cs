using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RadzenBlazorDemo.ClientApp.Models;
using RadzenBlazorDemo.ClientApp.Pages;
using Sabatex.RadzenBlazor.Server;

namespace RadzenBlazorDemo.Data
{
   
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            base.ConfigureConventions(configurationBuilder);
        }
    }
}
