namespace Ipco.Application.Messaging;

public interface IQuery;

public interface IQueryHandler<in TQuery, TResponse> where TQuery : IQuery
{
    Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
