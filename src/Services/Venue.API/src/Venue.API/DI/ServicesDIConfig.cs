using Library.Shared.Clients.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Venue.API.Application.Abstractions;
using Venue.API.Application.Services;
using Venue.API.Infrastructure.Caching;
using Venue.API.Infrastructure.Clients.Factories;
using Venue.API.Infrastructure.Services;

namespace Venue.API.DI
{
    public static class ServicesDIConfig
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IReadOnlyVenueService, ReadOnlyVenueService>()
                .AddScoped<IVenueService, VenueService>();

            services
                .AddSingleton<IRestClientFactory, CategoryRestClientFactory>()
                .AddSingleton<IRestClientFactory, FileStorageRestClientFactory>();

            services
                .AddSingleton<ICategoryDataService, CategoryDataService>()
                .AddSingleton<IFileStorageDataService, FileStorageDataService>();

            services
                .AddSingleton<ICategoriesCacheRepository, CategoriesCacheRepository>();

            return services;
        }
    }
}