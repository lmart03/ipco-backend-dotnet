namespace Ipco.Application.Identity;

public interface ICurrentUser
{
    Guid? UserId { get; }
    bool IsAuthenticated { get; }
}
