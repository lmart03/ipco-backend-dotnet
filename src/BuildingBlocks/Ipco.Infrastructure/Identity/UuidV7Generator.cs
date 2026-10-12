using Ipco.Application.Identity;

namespace Ipco.Infrastructure.Identity;

public sealed class UuidV7Generator(TimeProvider timeProvider) : IUuidGenerator
{
    public Guid NewId() => Guid.CreateVersion7(timeProvider.GetUtcNow());
}
