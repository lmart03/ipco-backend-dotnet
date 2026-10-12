namespace Ipco.Api.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<PatientsApiFactory>
{
    public const string Name = "API integration tests";
}
