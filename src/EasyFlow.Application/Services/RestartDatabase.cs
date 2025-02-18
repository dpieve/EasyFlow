using EasyFlow.Application.Common;
using EasyFlow.Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EasyFlow.Application.Services;

public sealed class RestartDatabase
{
    public sealed class Command : IRequest<Result<Unit>>
    {
    }

    public sealed class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly ILogger<Handler> _logger;

        public Handler(DataContext context, ILogger<Handler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var result = await _context.Database.EnsureDeletedAsync(cancellationToken);

            if (!result)
            {
                _logger.LogInformation("Attempt to delete a database that doesn't exist.");
            }

            await _context.Database.MigrateAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value);
        }
    }
}