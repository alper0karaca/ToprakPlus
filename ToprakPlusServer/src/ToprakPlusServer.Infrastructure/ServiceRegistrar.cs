using GenericRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Scrutor;
using ToprakPlusServer.Application.Services;
using ToprakPlusServer.Domain.Users;
using ToprakPlusServer.Infrastructure.Context;
using ToprakPlusServer.Infrastructure.Options;
using ToprakPlusServer.Infrastructure.Repositories;
using ToprakPlusServer.Infrastructure.Services;

namespace ToprakPlusServer.Infrastructure;

public static class ServiceRegistrar
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));  // jwt bilgileri eşleştirmesi
        services.ConfigureOptions<JwtSetupOptions>();
        services.AddAuthentication().AddJwtBearer();
        services.AddAuthorization();

        services.Configure<MailSettingOptions>(configuration.GetSection("MailSettings"));

        using var scoped = services.BuildServiceProvider().CreateScope();
        var mailSettings = scoped.ServiceProvider.GetRequiredService<IOptions<MailSettingOptions>>();
        if (string.IsNullOrEmpty(mailSettings.Value.UserId))
        {
            services.AddFluentEmail(mailSettings.Value.Email).AddSmtpSender(
                mailSettings.Value.Smtp,
                mailSettings.Value.Port);
        }
        else
        {
            services.AddFluentEmail(mailSettings.Value.Email)
                .AddSmtpSender(
                    mailSettings.Value.Smtp,
                    mailSettings.Value.Port,
                    mailSettings.Value.UserId,
                    mailSettings.Value.Password);
        } 
        
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            string connectionString = configuration.GetConnectionString("PostgresqlServer")!;
            opt.UseNpgsql(connectionString);
        });

        services.AddScoped<IUnitOfWork>(srv => srv.GetRequiredService<ApplicationDbContext>());
        
        // Burada DI'ları eklerken 'Scrutor' kütüphanesi kullandım 
        /*services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IJwtProvider, JwtProvider>();*/

        services.Scan(action => action
            .FromAssemblies(typeof(ServiceRegistrar).Assembly)
            .AddClasses(publicOnly: false)
            .UsingRegistrationStrategy(RegistrationStrategy.Skip)
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );
        
        return services;
    }
}