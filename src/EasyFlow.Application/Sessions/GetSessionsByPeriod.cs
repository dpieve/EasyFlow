using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Sessions;

public sealed class GetSessionsByPeriodQuery : IRequest<Result<List<Session>>>
{
    public int NumDays { get; set; }
}

public sealed class GetSessionsByPeriodQueryHandler : IRequestHandler<GetSessionsByPeriodQuery, Result<List<Session>>>
{
    private readonly ISessionsRepository _sessionsRepository;

    public GetSessionsByPeriodQueryHandler(ISessionsRepository sessionsRepository)
    {
        _sessionsRepository = sessionsRepository;
    }

    public async Task<Result<List<Session>>> Handle(GetSessionsByPeriodQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _sessionsRepository.GetAllAsync();

        var numDays = request.NumDays!;

        var now = DateTime.Now;
        var start = now.AddDays(-numDays);

        var sessionsFiltered = sessions
                .Where(sessions => sessions.FinishedDate >= start)
                .ToList();

        return Result<List<Session>>.Success(sessionsFiltered);
    }
}