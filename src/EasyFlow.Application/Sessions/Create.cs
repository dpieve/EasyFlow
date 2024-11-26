using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Application.Sessions;

public sealed class Create
{
    public sealed class Command : IRequest<Result<Unit>>
    {
        public required Session Session { get; init; }
    }

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Session).NotNull().SetValidator(new SessionValidator());
        }
    }

    public sealed class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IValidator<Command> _validator;

        public Handler(DataContext context, IValidator<Command> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.Session.Description))
            {
                request.Session.Description = "-";
            }

            var validatorResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validatorResult.IsValid)
            {
                return Result<Unit>.Failure(SessionsErrors.BadRequest);
            }

            var session = request.Session;
            
            var existingTag = await _context.Tags.FirstAsync(t => t.Id == session.TagId, cancellationToken);
            session.Tag = existingTag;

            _context.Sessions.Add(session);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(SessionsErrors.CreateFail);
        }
    }
}


