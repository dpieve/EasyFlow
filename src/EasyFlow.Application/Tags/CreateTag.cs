using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Tags;

public sealed class CreateTagCommand : IRequest<Result<Tag>>
{
    public Tag? Tag { get; init; }
}

public sealed class CreateTagCommandHandler : IRequestHandler<CreateTagCommand, Result<Tag>>
{
    private readonly ITagsRepository _tagsRepository;

    public CreateTagCommandHandler(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository;
    }

    async Task<Result<Tag>> IRequestHandler<CreateTagCommand, Result<Tag>>.Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tagId = await _tagsRepository.CreateAsync(request.Tag!);
        var tag = request.Tag!;
        tag.Id = tagId;
        return Result<Tag>.Success(tag);
    }
}