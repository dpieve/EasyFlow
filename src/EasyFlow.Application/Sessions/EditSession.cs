
using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Sessions;

public sealed class EditSessionCommand : IRequest<Result<bool>>
{
    public int SessionId { get; init; }

    public Session? Session { get; init; }
}

public sealed class EditSessionCommandHandler : IRequestHandler<EditSessionCommand, Result<bool>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public EditSessionCommandHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<Result<bool>> Handle(EditSessionCommand request, CancellationToken cancellationToken)
    {
        var sessionId = request.SessionId!;

        if (sessionId <= 0)
        {
            return Result<bool>.Failure(SessionsErrors.NotFound);
        }

        if (request.Session is null)
        {
            return Result<bool>.Failure(SessionsErrors.NotFound);
        }

        var session = request.Session;
        session.Id = sessionId;

        var success = await _sessionsRepository.EditAsync(session);
        return success ? Result<bool>.Success(true) : Result<bool>.Failure(SessionsErrors.NotFound);
    }
}
