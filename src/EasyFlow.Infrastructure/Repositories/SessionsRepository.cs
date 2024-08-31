using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using EasyFlow.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Infrastructure.Repositories;

public sealed class SessionsRepository : ISessionsRepository
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SessionsRepository(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<int> CreateAsync(Session session)
    {
        var context = await _contextFactory.CreateDbContextAsync();

        var existingTag = await context.Tags.FirstAsync(t => t.Id == session.TagId);

        session.Tag = existingTag;

        _ = context.Sessions.AddAsync(session);
        return await context.SaveChangesAsync();
    }

    public async Task<List<Session>> GetAllAsync()
    {
        var context = await _contextFactory.CreateDbContextAsync();
        return await context.Sessions.Include(s => s.Tag).ToListAsync();
    }

    public async Task<int> UpdateAsync(Session session)
    {
        var context = await _contextFactory.CreateDbContextAsync();

        var existingTag = await context.Tags.FirstAsync(t => t.Id == session.TagId);

        session.Tag = existingTag;

        _ = context.Sessions.Update(session);
        return await context.SaveChangesAsync();
    }
}