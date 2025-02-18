using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Application.Settings;

public sealed class Get
{
    public sealed class Query : IRequest<Result<GeneralSettings>>
    {
    }

    public sealed class Handler : IRequestHandler<Query, Result<GeneralSettings>>
    {
        private readonly DataContext _context;

        public Handler(DataContext dataContext)
        {
            _context = dataContext;
        }

        public async Task<Result<GeneralSettings>> Handle(Query request, CancellationToken cancellationToken)
        {
            var settings = await _context.GeneralSettings
                                    .Include(gs => gs.SelectedTag)
                                    .ToListAsync(cancellationToken);

            if (settings.Count == 0)
            {
                return Result<GeneralSettings>.Failure(SettingsErrors.NotFound);
            }

            return Result<GeneralSettings>.Success(settings[0]);
        }
    }
}