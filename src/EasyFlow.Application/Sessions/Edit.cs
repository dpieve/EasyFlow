using AutoMapper;
using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using FluentValidation;
using MediatR;

namespace EasyFlow.Application.Sessions;

public sealed class Edit
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
        private readonly IMapper _mapper;

        public Handler(DataContext context, IValidator<Command> validator, IMapper mapper)
        {
            _context = context;
            _validator = validator;
            _mapper = mapper;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<Unit>.Failure(SessionsErrors.BadRequest);
            }

            var found = await _context.Sessions.FindAsync(request.Session.Id, cancellationToken);
            if (found is null)
            {
                return Result<Unit>.Failure(SessionsErrors.NotFound);
            }

            _mapper.Map(request.Session, found);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;
            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(SessionsErrors.EditFail);
        }
    }

}