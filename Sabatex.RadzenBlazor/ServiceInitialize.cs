
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using Sabatex.Core.RadzenBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sabatex.RadzenBlazor
{
    public static class ServiceInitialize
    {
        public static IServiceCollection AddSabatexRadzenBlazor(this IServiceCollection services) 
        {
            services.AddRadzenComponents();
            services.AddScoped<SabatexJsInterop>();
            services.AddSingleton<SabatexBlazorAppState>();
            return services;
        }
        public static IServiceCollection AddSabatexRadzenBlazor<TPWAPush>(this IServiceCollection services) where TPWAPush :class, IPWAPush
        {
            services.AddScoped<IPWAPush, TPWAPush>();
            return services.AddSabatexRadzenBlazor();
        }

        public static IServiceCollection AddSabatexRadzenBlazor<TDataAdapter,TKey>(this IServiceCollection services) where TDataAdapter : class, ISabatexRadzenBlazorDataAdapter<TKey>
        {
            services.AddSabatexRadzenBlazor().AddScoped<ISabatexRadzenBlazorDataAdapter<TKey>, TDataAdapter>();
            return services;
        }
    
    
        //public static void AddNotNull(this QueryBuilder queryBuilder,  string key,string? value)
        //{
        //    if (value != null) queryBuilder.Add(key, value);
        //}
    
    }
}
