using Ipco.Domain;

namespace Ipco.Domain.UnitTests;

public sealed class EntityTests
{
    [Fact]
    public void Constructor_WithEmptyId_Throws() =>
        Assert.Throws<ArgumentException>(() => new TestEntity(Guid.Empty));

    [Fact]
    public void Equality_WithSameTypeAndId_IsTrue()
    {
        var id = Guid.CreateVersion7();
        Assert.Equal(new TestEntity(id), new TestEntity(id));
    }

    [Fact]
    public void Equality_WithDifferentRuntimeTypes_IsFalse()
    {
        var id = Guid.CreateVersion7();
        Assert.NotEqual<Entity>(new TestEntity(id), new OtherEntity(id));
    }

    private sealed class TestEntity(Guid id) : Entity(id);
    private sealed class OtherEntity(Guid id) : Entity(id);
}
