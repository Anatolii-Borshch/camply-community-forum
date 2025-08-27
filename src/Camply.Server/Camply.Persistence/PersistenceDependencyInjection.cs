using Camply.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Camply.Persistence
{
    public static class PersistenceDependencyInjection
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CamplyDbContext>(options =>
            {
                var connectionString = configuration.GetConnectionString("DbConnectionString");
                options.UseSqlServer(connectionString);
            });
            
            return services;
        }
    }
}