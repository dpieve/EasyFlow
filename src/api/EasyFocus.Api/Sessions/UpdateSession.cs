using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EasyFocus.Api.Sessions;

public sealed class UpdateSession : Endpoint<Session, bool>
{
    private readonly AppDbContext _appDbContext;

    public UpdateSession(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override void Configure()
    {
        Post("/sessions/update");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Session session, CancellationToken ct)
    {
        var sessionToUpdate = await _appDbContext.Sessions.FirstOrDefaultAsync(s => s.Id == session.Id, ct);

        if (sessionToUpdate is null)
        {
            await SendAsync(true, cancellation: ct);
            return;
        }

        sessionToUpdate.Description = session.Description;
        sessionToUpdate.DurationSeconds = session.DurationSeconds;
        sessionToUpdate.TagId = session.TagId;
        sessionToUpdate.FinishedDateTime = session.FinishedDateTime;
        sessionToUpdate.DurationSeconds = session.DurationSeconds;
        sessionToUpdate.SessionType = session.SessionType;

        _appDbContext.Update(sessionToUpdate);

        await _appDbContext.SaveChangesAsync(ct);

        await SendAsync(true, cancellation: ct);
    }
}