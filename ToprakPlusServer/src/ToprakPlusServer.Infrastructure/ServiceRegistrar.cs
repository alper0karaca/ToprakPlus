using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToprakPlusServer.Application.Services;
using ToprakPlusServer.Infrastructure.Context;
using ToprakPlusServer.Infrastructure.Services;

namespace ToprakPlusServer.Infrastructure;

public static class ServiceRegistrar
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            string connectionString = configuration.GetConnectionString("PostgresqlServer")!;
            opt.UseNpgsql(connectionString);
        });

        // Burada DI'ları eklerken 'Scrutor' kütüphanesi kullanılabilir. 
        services.AddScoped<IUserContext, UserContext>();

        return services;
    }
}