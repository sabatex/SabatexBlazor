using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RadzenBlazorDemo.ClientApp.Models;
using RadzenBlazorDemo.Data;
using Sabatex.Core.RadzenBlazor;
using Sabatex.RadzenBlazor;
using Sabatex.RadzenBlazor.Server;
using System.Security.Claims;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Sabatex.Bakery.Controllers
{
    public class PersonController : BaseController<Person>
    {
        public PersonController(ApplicationDbContext context, ILogger<PersonController> logger) : base(context, logger)
        {
        }

        protected override async Task<bool> CheckAccess(Person item, Person? updated)
        {
            await Task.Yield();
            return true;
        }

    }
}
