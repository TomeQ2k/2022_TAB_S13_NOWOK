﻿using Library.Shared.Clients.Factories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VenueList.API.Application.Abstractions;
using VenueList.API.Infrastructure.Caching;
using VenueList.API.Infrastructure.Clients.Factories;
using VenueList.API.Infrastructure.Services;

namespace VenueList.API.DI
{
    public static class ServicesDIConfig
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<ICategoryDataService, CategoryDataService>();

            services
                .AddSingleton<IRestClientFactory, CategoryRestClientFactory>();

            
            services
                .AddSingleton<ICategoriesCacheRepository, CategoriesCacheRepository>();
            
            return services;
        }
    }
}