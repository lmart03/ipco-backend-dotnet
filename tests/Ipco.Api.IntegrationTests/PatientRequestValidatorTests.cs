using FluentValidation;
using Ipco.Modules.Patients.Presentation;

namespace Ipco.Api.IntegrationTests;

public sealed class PatientRequestValidatorTests
{
    private static readonly TimeProvider Time = new FixedTimeProvider();
    private readonly PatientRequestValidator _validator = new(Time);

    [Fact]
    public async Task MinimalValidRequest_IsValid()
    {
        var result = await _validator.ValidateAsync(Request());
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task RequiredFieldsMissing_AreReportedByProperty()
    {
        var result = await _validator.ValidateAsync(Request() with { FirstName = null!, LastName = null!, Phone = null! });
        Assert.Contains(result.Errors, x => x.PropertyName == "FirstName");
        Assert.Contains(result.Errors, x => x.PropertyName == "LastName");
        Assert.Contains(result.Errors, x => x.PropertyName == "Phone");
    }

    [Fact]
    public async Task IncompleteDocument_IsInvalid()
    {
        var result = await _validator.ValidateAsync(Request() with { DocumentNumber = "12345678" });
        Assert.Contains(result.Errors, x => x.PropertyName == "document");
    }

    [Fact]
    public async Task FutureBirthDate_IsInvalid()
    {
        var result = await _validator.ValidateAsync(Request() with { BirthDate = new DateOnly(2026, 10, 13) });
        Assert.Contains(result.Errors, x => x.PropertyName == "BirthDate");
    }

    private static PatientRequest Request() => new(null, null, "Ana", "Pérez", "987654321", null,
        null, null, null, null, null, null, null);

    private sealed class FixedTimeProvider : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(2026, 10, 12, 1, 0, 0, TimeSpan.Zero);
    }
}
