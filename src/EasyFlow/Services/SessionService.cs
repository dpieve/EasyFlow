using EasyFlow.Common;
using EasyFlow.Data;
using EasyFlow.Features.Dashboard;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyFlow.Services;

public interface ISessionService
{
    public Task<Result<int, Error>> CreateAsync(Session session);

    public Task<Result<int, Error>> DeleteAsync(Session session);

    public Task<Result<int, Error>> UpdateAsync(Session session);

    public Result<List<Session>, Error> GetAll();
    public Task<Result<List<Session>, Error>> GetSessionsByPeriod(FilterPeriod filterPeriod);
}

public sealed class SessionService : ISessionService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SessionService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Result<int, Error>> CreateAsync(Session session)
    {
        var context = await _contextFactory.CreateDbContextAsync();
        _ = await context.Sessions.AddAsync(session);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return SessionServiceErrors.NoEntityModified;
        }

        return result;
    }

    public async Task<Result<int, Error>> DeleteAsync(Session session)
    {
        var context = await _contextFactory.CreateDbContextAsync();
        _ = context.Sessions.Remove(session);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return SessionServiceErrors.NoEntityModified;
        }
        return result;
    }

    public Result<List<Session>, Error> GetAll()
    {
        var context = _contextFactory.CreateDbContext();
        return context.Sessions.ToList();
    }

    public async Task<Result<List<Session>, Error>> GetSessionsByPeriod(FilterPeriod filterPeriod)
    {
        var context = await _contextFactory.CreateDbContextAsync();
        var currentDate = DateTime.Now;
        var startDate = currentDate.AddDays(-filterPeriod.NumDays);
        var sessions = await context.Sessions
                                .Where(s => s.FinishedDate >= startDate)
                                .ToListAsync();
        return sessions;
    }

    public async Task<Result<int, Error>> UpdateAsync(Session session)
    {
        var context = await _contextFactory.CreateDbContextAsync();
        context.Update(session);
        var result = await context.SaveChangesAsync();
        if (result == 0)
        {
            return SessionServiceErrors.NoEntityModified;
        }
        return result;
    }
}

public static class SessionServiceErrors
{
    public static readonly Error NoEntityModified = new("Session.NoEntityModified",
       "Session was not modified.");

    public static readonly Error NotFound = new("Session.NotFound",
      "No session found.");
}