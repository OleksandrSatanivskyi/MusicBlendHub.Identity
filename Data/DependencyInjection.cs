using Microsoft.EntityFrameworkCore;
using MusicBlendHub.Identity.Data.DbContexts;

namespace MusicBlendHub.Identity.Data
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["SqlServerDbConnection"];
            services.AddDbContext<SqlServerDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<IAuthDbContext>(provider => provider.GetService<SqlServerDbContext>());

            return services;
        }
    }
}
