using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Application.Tags;

public sealed class Get
{
    public sealed class Query : IRequest<Result<List<Tag>>>
    {
    }

    public sealed class Handler : IRequestHandler<Query, Result<List<Tag>>>
    {
        private readonly DataContext _context;
        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Tag>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var tags = await _context.Tags.Include(t => t.Sessions).ToListAsync();
            return Result<List<Tag>>.Success(tags);
        }
    }
}
