using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EasyFocus.Api.Tags;

public class GetTags : EndpointWithoutRequest<List<Tag>>
{
    private readonly AppDbContext _appDbContext;

    public GetTags(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override void Configure()
    {
        Get("/tags");
        //AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var tags = await _appDbContext.Tags.Include(t => t.Sessions).ToListAsync(ct);
        await SendAsync(tags, cancellation: ct);
    }
}