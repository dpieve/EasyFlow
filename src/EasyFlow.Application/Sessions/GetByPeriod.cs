using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Application.Sessions;

public sealed class GetByPeriod
{
    public sealed class Query : IRequest<Result<List<Session>>>
    {
        public required int NumDays { get; init; }
    }

    public sealed class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(q => q.NumDays).GreaterThan(0);
        }
    }

    public sealed class Handler : IRequestHandler<Query, Result<List<Session>>>
    {
        private readonly DataContext _context;
        private readonly IValidator<Query> _validator;

        public Handler(DataContext context, IValidator<Query> validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<Result<List<Session>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var validatonResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validatonResult.IsValid)
            {
                return Result<List<Session>>.Failure(SessionsErrors.BadRequest);
            }

            var startDate = DateTime.Now.AddDays(-request.NumDays);

            var sessions = await _context.Sessions
                                    .Where(s => s.FinishedDate >= startDate)
                                    .Include(s => s.Tag)
                                    .ToListAsync(cancellationToken);

            return Result<List<Session>>.Success(sessions);
        }
    }
}