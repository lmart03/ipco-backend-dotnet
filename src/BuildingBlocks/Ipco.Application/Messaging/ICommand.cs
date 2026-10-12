namespace Ipco.Application.Messaging;

public interface ICommand;

public interface ICommandHandler<in TCommand, TResponse> where TCommand : ICommand
{
    Task<TResponse> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
