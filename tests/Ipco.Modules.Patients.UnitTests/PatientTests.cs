using Ipco.Modules.Patients.Domain;

namespace Ipco.Modules.Patients.UnitTests;

public sealed class PatientTests
{
    private static readonly DateTimeOffset Now = new(2026, 10, 12, 1, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_WithMinimalValidData_NormalizesValues()
    {
        var result = Patient.Create(Guid.CreateVersion7(), ValidData() with
        {
            FirstName = "  Ana ", LastName = " Pérez ", Phone = "+51 987-654-321", Email = " ANA@MAIL.COM "
        }, Now);

        Assert.True(result.IsSuccess);
        Assert.Equal("Ana", result.Value.FirstName);
        Assert.Equal("Pérez", result.Value.LastName);
        Assert.Equal("+51987654321", result.Value.Phone);
        Assert.Equal("ana@mail.com", result.Value.Email);
    }

    [Theory]
    [InlineData(null, "12345678")]
    [InlineData("00000000-0000-0000-0000-000000000001", null)]
    public void Create_WithIncompleteDocument_Fails(string? typeId, string? number)
    {
        var data = ValidData() with { DocumentTypeId = typeId is null ? null : Guid.Parse(typeId), DocumentNumber = number };
        var result = Patient.Create(Guid.CreateVersion7(), data, Now);
        Assert.True(result.IsFailure);
        Assert.Equal("patients.document_invalid", result.Error.Code);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("abc")]
    [InlineData("+51 (abc)")]
    public void Create_WithInvalidPhone_Fails(string phone)
    {
        var result = Patient.Create(Guid.CreateVersion7(), ValidData() with { Phone = phone }, Now);
        Assert.True(result.IsFailure);
        Assert.Equal("patients.phone_invalid", result.Error.Code);
    }

    [Theory]
    [InlineData("1899-12-31")]
    [InlineData("2026-10-13")]
    public void Create_WithBirthDateOutsideRange_Fails(string date)
    {
        var result = Patient.Create(Guid.CreateVersion7(), ValidData() with { BirthDate = DateOnly.Parse(date) }, Now);
        Assert.True(result.IsFailure);
        Assert.Equal("patients.birth_date_invalid", result.Error.Code);
    }

    [Fact]
    public void Delete_WhenCalledTwice_PreservesFirstDeletionTime()
    {
        var patient = Patient.Create(Guid.CreateVersion7(), ValidData(), Now).Value;
        patient.Delete(Now.AddMinutes(1));
        patient.Delete(Now.AddMinutes(2));
        Assert.True(patient.IsDeleted);
        Assert.Equal(Now.AddMinutes(1), patient.DeletedAtUtc);
    }

    private static PatientData ValidData() => new(null, null, "Ana", "Pérez", "987654321", null,
        null, null, null, null, null, null, null);
}
