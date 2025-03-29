using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Api.Tags;

public sealed class DeleteTag : Endpoint<Tag, bool>
{
    private readonly AppDbContext _appDbContext;

    public DeleteTag(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override void Configure()
    {
        Post("/tags/delete");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Tag tag, CancellationToken ct)
    {
        var tagToDelete = await _appDbContext.Tags.Include(t => t.Sessions).FirstOrDefaultAsync(t => t.Id == tag.Id, ct);
        if (tagToDelete is null)
        {
            await SendAsync(false, cancellation: ct);
            return;
        }

        _appDbContext.Remove(tagToDelete);

        await _appDbContext.SaveChangesAsync(ct);

        await SendAsync(true, cancellation: ct);
    }
}