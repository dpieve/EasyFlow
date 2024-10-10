using EasyFlow.Application.Common;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Sessions;

public sealed class DeleteSessionCommand : IRequest<Result<bool>>
{
    public int SessionId { get; init; }
}

public sealed class DeleteSessionCommandHandler : IRequestHandler<DeleteSessionCommand, Result<bool>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public DeleteSessionCommandHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<Result<bool>> Handle(DeleteSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionId = request.SessionId!;
        
        if (sessionId <= 0)
        {
            return Result<bool>.Failure(SessionsErrors.NotFound);
        }

        var success = await _sessionsRepository.DeleteAsync(sessionId);
        return success != 0 ? Result<bool>.Success(true) : Result<bool>.Failure(SessionsErrors.NotFound);
    }
}

public static partial class SessionsErrors
{
    public static readonly Error NotFound = new($"Session.NotFound",
       "Failed to delete the session");
}
