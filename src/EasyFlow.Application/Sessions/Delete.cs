using EasyFlow.Application.Common;
using EasyFlow.Infrastructure.Common;
using MediatR;

namespace EasyFlow.Application.Sessions;

public sealed class Delete
{
    public sealed class Command : IRequest<Result<Unit>>
    {
        public int SessionId { get; init; }
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
            var session = await _context.Sessions.FindAsync(request.SessionId, cancellationToken);
            if (session is null)
            {
                return Result<Unit>.Failure(SessionsErrors.NotFound);
            }

            _ = _context.Sessions.Remove(session);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(SessionsErrors.DeleteFail);
        }
    }
}