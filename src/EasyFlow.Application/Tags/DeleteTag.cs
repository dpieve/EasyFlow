using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Tags;
public sealed class DeleteTagCommand : IRequest<Result<bool>>
{
    public Tag? Tag { get; init; }
}

public sealed class DeleteTagCommandHandler : IRequestHandler<DeleteTagCommand, Result<bool>>
{
    private readonly ITagsRepository _tagsRepository;

    public DeleteTagCommandHandler(ITagsRepository tagsRepository)
    {
        _tagsRepository = tagsRepository;
    }

    public async Task<Result<bool>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = request.Tag!;

        var result = await _tagsRepository.DeleteAsync(tag);
        if (!result)
        {
            return Result<bool>.Failure(TagsErrors.DeleteFail);
        }

        return Result<bool>.Success(true);
    }
}

public static partial class TagsErrors
{
    public static readonly Error DeleteFail = new($"Tag.DeleteFail",
       "Failed to delete the tag");
}