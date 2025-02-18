using AutoMapper;
using EasyFlow.Application.Common;
using EasyFlow.Application.Settings;
using EasyFlow.Infrastructure.Common;
using FluentValidation;
using MediatR;

namespace EasyFlow.Application.Tags;

public sealed class Edit
{
    public sealed class Command : IRequest<Result<Unit>>
    {
        public required Domain.Entities.Tag Tag { get; init; }
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
        private readonly IMapper _mapper;
        private readonly IValidator<Command> _validator;

        public Handler(DataContext context, IMapper mapper, IValidator<Command> validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return Result<Unit>.Failure(SettingsErrors.BadRequest);
            }

            var found = await _context.Tags.FindAsync(request.Tag.Id, cancellationToken);

            if (found is null)
            {
                return Result<Unit>.Failure(SettingsErrors.NotFound);
            }

            _mapper.Map(request.Tag, found);

            var success = await _context.SaveChangesAsync(cancellationToken) > 0;

            return success ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(TagsErrors.EditFail);
        }
    }
}