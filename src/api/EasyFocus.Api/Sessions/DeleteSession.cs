using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EasyFocus.Api.Sessions;

public sealed class DeleteSession : Endpoint<Session, bool>
{
    private readonly AppDbContext _appDbContext;

    public DeleteSession(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override void Configure()
    {
        Post("/sessions/delete");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Session session, CancellationToken ct)
    {
        var sessionToDelete = await _appDbContext.Sessions.FirstOrDefaultAsync(s => s.Id == session.Id, ct);

        if (sessionToDelete is null)
        {
            await SendAsync(false, cancellation: ct);
            return;
        }

        _appDbContext.Remove(sessionToDelete);
        await _appDbContext.SaveChangesAsync(ct);
        await SendAsync(true, cancellation: ct);
    }
}