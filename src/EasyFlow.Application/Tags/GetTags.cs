using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Tags;

public sealed class GetTagsQuery : IRequest<Result<List<Tag>>>
{
}

public sealed class GetTagsQueryHandler : IRequestHandler<GetTagsQuery, Result<List<Tag>>>
{
    private readonly ITagsRepository _tagsRepository;

    public GetTagsQueryHandler(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository;
    }

    public async Task<Result<List<Tag>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagsRepository.GetAsync();
        return Result<List<Tag>>.Success(tags);
    }
}