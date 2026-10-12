using FluentValidation;
using Ipco.Modules.Patients.Application;
using Ipco.Modules.Patients.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ipco.Modules.Patients.Presentation;

namespace Ipco.Modules.Patients;

public static class DependencyInjection
{
    public static IServiceCollection AddPatientsModule(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("IpcoDatabase")
            ?? throw new InvalidOperationException("Connection string 'IpcoDatabase' is not configured.");

        services.AddDbContext<PatientsDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<PatientService>();
        services.AddScoped<IValidator<PatientRequest>, PatientRequestValidator>();
        return services;
    }
}
