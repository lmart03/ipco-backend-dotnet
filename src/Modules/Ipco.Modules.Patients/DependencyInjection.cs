using FluentValidation;
using Ipco.Modules.Patients.Application;
using Ipco.Modules.Patients.Infrastructure;
using Ipco.Modules.Patients.Presentation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ipco.Modules.Patients;

public static class DependencyInjection
{
    public static IServiceCollection AddPatientsModule(this IServiceCollection services)
    {
        services.AddDbContext<PatientsDbContext>((serviceProvider, options) =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("IpcoDatabase")
                ?? throw new InvalidOperationException("Connection string 'IpcoDatabase' is not configured.");
            options.UseNpgsql(connectionString);
        });
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<PatientService>();
        services.AddScoped<IValidator<PatientRequest>, PatientRequestValidator>();
        return services;
    }
}
