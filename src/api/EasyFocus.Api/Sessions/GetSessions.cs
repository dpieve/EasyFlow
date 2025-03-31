using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EasyFocus.Api.Sessions;

public sealed class GetSessions : EndpointWithoutRequest<List<Session>>
{
    private readonly AppDbContext _appDbContext;

    public GetSessions(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override void Configure()
    {
        Get("/sessions");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sessions = await _appDbContext.Sessions.Include(s => s.Tag).ToListAsync(ct);
        await SendAsync(sessions, cancellation: ct);
    }
}