using EasyFlow.Application.Common;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Application.Tags;

public sealed class Delete
{
    public sealed class Command : IRequest<Result<Unit>>
    {
        public required Tag Tag { get; init; }
    }

    public sealed class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var settings = await _context.GeneralSettings.FirstOrDefaultAsync();

            if (settings is null)
            {
                return Result<Unit>.Failure(SettingsErrors.NotFound);
            }

            if (settings.SelectedTagId == request.Tag.Id)
            {
                return Result<Unit>.Failure(TagsErrors.CanNotDeleteSelectedTag);
            }

            var found = await _context.Tags.FindAsync(request.Tag.Id, cancellationToken);
            if (found is null)
            {
                return Result<Unit>.Failure(TagsErrors.NotFound);
            }

            _ = _context.Tags.Remove(found);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(TagsErrors.DeleteFail);
        }
    }
}