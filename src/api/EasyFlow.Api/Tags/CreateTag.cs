using FastEndpoints;

namespace EasyFlow.Api.Tags;

public sealed class CreateTag : Endpoint<Tag, Tag>
{
    private readonly AppDbContext _appDbContext;

    public CreateTag(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public override void Configure()
    {
        Post("/tags/create");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Tag tag, CancellationToken ct)
    {
        var createdTag = await _appDbContext.Tags.AddAsync(tag, ct);
        await _appDbContext.SaveChangesAsync(ct);
        await SendAsync(createdTag.Entity, cancellation: ct);
    }
}
