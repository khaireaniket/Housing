using Housing.Application.Common.Interface.Persistence;
using Housing.Application.Common.Interface.Services;
using Housing.Infrastructure.Persistence;
using Housing.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DomainEntities = Housing.Domain.Entities;

namespace Housing.Infrastructure.IoC
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddDbContext<HousingDbContext>(options => options.UseInMemoryDatabase(databaseName: "HousingDB")
                                                                            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking), ServiceLifetime.Singleton);

            services.AddTransient<IHousingRepository<DomainEntities.House>, HousingRepository>();

            services.AddHttpClient<IHousingHttpClient, HousingHttpClient>(client => {
                client.BaseAddress = new Uri("https://partnerapi.funda.nl");
            });

            return services;
        }
    }
}
