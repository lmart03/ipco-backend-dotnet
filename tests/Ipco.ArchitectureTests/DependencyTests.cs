using NetArchTest.Rules;

namespace Ipco.ArchitectureTests;

public sealed class DependencyTests
{
    [Fact]
    public void Domain_DoesNotDependOnOuterLayers()
    {
        var result = Types.InAssembly(typeof(Ipco.Domain.Entity).Assembly)
            .ShouldNot().HaveDependencyOnAny("Ipco.Application", "Ipco.Infrastructure", "Ipco.Api", "Microsoft.AspNetCore", "Microsoft.EntityFrameworkCore")
            .GetResult();
        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void Application_DoesNotDependOnInfrastructureOrApi()
    {
        var result = Types.InAssembly(typeof(Ipco.Application.Messaging.ICommand).Assembly)
            .ShouldNot().HaveDependencyOnAny("Ipco.Infrastructure", "Ipco.Api")
            .GetResult();
        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void PatientsDomain_DoesNotDependOnApplicationInfrastructureOrAspNetCore()
    {
        var result = Types.InAssembly(typeof(Ipco.Modules.Patients.Domain.Patient).Assembly)
            .That().ResideInNamespace("Ipco.Modules.Patients.Domain")
            .ShouldNot().HaveDependencyOnAny(
                "Ipco.Modules.Patients.Application",
                "Ipco.Modules.Patients.Infrastructure",
                "Microsoft.EntityFrameworkCore",
                "Microsoft.AspNetCore")
            .GetResult();
        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    [Fact]
    public void PatientsApplication_DoesNotDependOnInfrastructureOrAspNetCore()
    {
        var result = Types.InAssembly(typeof(Ipco.Modules.Patients.Application.PatientService).Assembly)
            .That().ResideInNamespace("Ipco.Modules.Patients.Application")
            .ShouldNot().HaveDependencyOnAny("Ipco.Modules.Patients.Infrastructure", "Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore")
            .GetResult();
        Assert.True(result.IsSuccessful, FormatFailures(result));
    }

    private static string FormatFailures(TestResult result) => result.FailingTypeNames is null
        ? "Architecture rule failed."
        : string.Join(Environment.NewLine, result.FailingTypeNames);
}
