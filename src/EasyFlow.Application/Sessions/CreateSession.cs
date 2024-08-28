using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Sessions;

public sealed class CreateSessionCommand : IRequest<Result<Session>>
{
    public Session? Session { get; init; }
}

public sealed class CreateSessionCommandHandler : IRequestHandler<CreateSessionCommand, Result<Session>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public CreateSessionCommandHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<Result<Session>> Handle(CreateSessionCommand request, CancellationToken cancellationToken)
    {
        var session = request.Session!;
        if (session.Id == 0)
        {
            var result = await _sessionsRepository.CreateAsync(session);
            return result != 0 ? Result<Session>.Success(session) : Result<Session>.Failure(SessionsErrors.CreateFail);
        }

        var success = await _sessionsRepository.UpdateAsync(session);
        return success != 0 ? Result<Session>.Success(session) : Result<Session>.Failure(SessionsErrors.UpdateFail);
    }
}

public static partial class SessionsErrors
{
    public static readonly Error CreateFail = new($"Session.CreateFail",
       "Failed to create the session");

    public static readonly Error UpdateFail = new($"Session.UpdateFail",
       "Failed to update the session");
}
