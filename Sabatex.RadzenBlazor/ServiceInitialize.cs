
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
    /// <summary>
    /// Provides extension methods for registering Sabatex Radzen Blazor services with an <see
    /// cref="IServiceCollection"/>.
    /// </summary>
    /// <remarks>These methods enable the integration of Sabatex Radzen Blazor components, JavaScript interop,
    /// application state management, and optional custom service implementations into a Blazor application's dependency
    /// injection container.</remarks>
    public static class ServiceInitialize
    {
        /// <summary>
        /// Adds Sabatex Radzen Blazor services to the specified <see cref="IServiceCollection"/>.
        /// </summary>
        /// <remarks>This method registers the required services for Sabatex Radzen Blazor, including
        /// Radzen components, Sabatex JavaScript interop, and application state management.</remarks>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddSabatexRadzenBlazor(this IServiceCollection services) 
        {
            services.AddRadzenComponents();
            services.AddScoped<SabatexJsInterop>();
            services.AddSingleton<SabatexBlazorAppState>();
            services.AddScoped<IComponentStateStore, PWALocalStorageStore>();
            return services;
        }
        /// <summary>
        /// Adds Sabatex Radzen Blazor services to the specified <see cref="IServiceCollection"/> and registers a custom
        /// implementation of <see cref="IPWAPush"/>.
        /// </summary>
        /// <remarks>This method registers the specified implementation of <see cref="IPWAPush"/> as a
        /// scoped service and then adds the default Sabatex Radzen Blazor services.</remarks>
        /// <typeparam name="TPWAPush">The type of the class implementing the <see cref="IPWAPush"/> interface.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddSabatexRadzenBlazor<TPWAPush>(this IServiceCollection services) where TPWAPush :class, IPWAPush
        {
            services.AddScoped<IPWAPush, TPWAPush>();
            return services.AddSabatexRadzenBlazor();
        }
        /// <summary>
        /// Adds Sabatex Radzen Blazor services to the specified <see cref="IServiceCollection"/> and registers a custom
        /// data adapter implementation.
        /// </summary>
        /// <remarks>This method registers the default Sabatex Radzen Blazor services and adds a scoped
        /// service for the specified data adapter type <typeparamref name="TDataAdapter"/>.</remarks>
        /// <typeparam name="TDataAdapter">The type of the data adapter to register. Must implement <see
        /// cref="ISabatexRadzenBlazorDataAdapter"/>.</typeparam>
        /// <typeparam name="TKey">The type of the key used by the data adapter.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the services will be added.</param>
        /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddSabatexRadzenBlazor<TDataAdapter,TKey>(this IServiceCollection services) where TDataAdapter : class, ISabatexRadzenBlazorDataAdapter
        {
            services.AddSabatexRadzenBlazor().AddScoped<ISabatexRadzenBlazorDataAdapter, TDataAdapter>();
            return services;
        }
    }
}
