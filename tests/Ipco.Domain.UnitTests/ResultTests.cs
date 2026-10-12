using Ipco.Domain;

namespace Ipco.Domain.UnitTests;

public sealed class ResultTests
{
    [Fact]
    public void Failure_WithErrorNone_Throws() =>
        Assert.Throws<ArgumentException>(() => Result.Failure(Error.None));

    [Fact]
    public void GenericFailure_WhenReadingValue_Throws()
    {
        var result = Result<string>.Failure(Error.NotFound("resource.not_found", "The resource was not found."));
        Assert.Throws<InvalidOperationException>(() => result.Value);
    }

    [Fact]
    public void Success_WithNullableValue_PreservesNull()
    {
        var result = Result<string?>.Success(null);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ErrorFactory_PreservesMachineReadableCodeAndType()
    {
        var error = Error.Conflict("resource.conflict", "The resource conflicts with current state.");
        Assert.Equal("resource.conflict", error.Code);
        Assert.Equal(ErrorType.Conflict, error.Type);
    }
}
