using EasyFlow.Application.Common;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Settings;

public sealed class ResetDbQuery : IRequest<Result<bool>>
{
}

public sealed class ResetDbQueryHandler : IRequestHandler<ResetDbQuery, Result<bool>>
{
    private readonly IDatabaseManagerRepository _databaseManagerRepository;

    public ResetDbQueryHandler(IDatabaseManagerRepository databaseManagerRepository)
    {
        _databaseManagerRepository = databaseManagerRepository;
    }

    public async Task<Result<bool>> Handle(ResetDbQuery request, CancellationToken cancellationToken)
    {
        var result = await _databaseManagerRepository.ResetAsync();
        return result ? Result<bool>.Success(result) : Result<bool>.Failure(ResetDbErrors.Fail);
    }
}

public static partial class ResetDbErrors
{
    public static readonly Error Fail = new($"ResetDb.Fail",
       "Failed to delete the data");
}