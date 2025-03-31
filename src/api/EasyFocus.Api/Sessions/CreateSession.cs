using FastEndpoints;

namespace EasyFocus.Api.Sessions;

public sealed class CreateSession : Endpoint<Session, Session>
{
    private readonly AppDbContext _appDbContext;

    public CreateSession(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override void Configure()
    {
        Post("/sessions/create");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Session session, CancellationToken ct)
    {
        var createdSession = await _appDbContext.Sessions.AddAsync(session, ct);
        await _appDbContext.SaveChangesAsync(ct);
        await SendAsync(createdSession.Entity, cancellation: ct);
    }
}