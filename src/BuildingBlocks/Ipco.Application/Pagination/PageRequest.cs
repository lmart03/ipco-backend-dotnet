namespace Ipco.Application.Pagination;

public sealed record PageRequest(int Page = 1, int PageSize = 20)
{
    public const int MaximumPageSize = 100;
}
