using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Application.Tags;

public sealed class Create
{
    public sealed class Command : IRequest<Result<Unit>>
    {
        public required Tag Tag { get; init; }
    }

    public sealed class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Tag).NotNull().SetValidator(new TagsValidator());
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
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<Unit>.Failure(TagsErrors.BadRequest);
            }

            var numTags = await _context.Tags.CountAsync(cancellationToken);
            if (numTags > Tag.MaxNumTags)
            {
                return Result<Unit>.Failure(TagsErrors.CanNotMoreThanMax);
            }

            _ = await _context.Tags.AddAsync(request.Tag);

            var result = await _context.SaveChangesAsync() > 0;

            return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(TagsErrors.CreateFail);
        }
    }
}