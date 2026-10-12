using Ipco.Application.Identity;
using Ipco.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Ipco.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddIpcoInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IUuidGenerator, UuidV7Generator>();
        return services;
    }
}
