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
        var tag = request.Tag!;

        if (tag.Name == string.Empty)
        {
            return Result<Tag>.Failure(TagsErrors.CanNotNameEmpty);
        }

        if (tag.Id != 0)
        {
            var success = await _tagsRepository.UpdateAsync(tag);
            return success ? Result<Tag>.Success(tag) : Result<Tag>.Failure(TagsErrors.UpdateFail);
        }

        var numTags = await _tagsRepository.CountAsync();
        if (numTags + 1 > Tag.MaxNumTags)
        {
            return Result<Tag>.Failure(TagsErrors.CanNotMoreThanMax);
        }

        var tagId = await _tagsRepository.CreateAsync(tag);
        tag.Id = tagId;

        return Result<Tag>.Success(tag);
    }
}

public static partial class TagsErrors
{
    public static readonly Error UpdateFail = new("Tag_UpdateFail",
       "Failed to update");

    public static readonly Error CanNotNameEmpty = new("Tag_CanNotEmptyName", "Empty names are not allowed");

    public static readonly Error CanNotMoreThanMax = new("Tag_CanNotMoreThanMax", $"There is a limit of {Tag.MaxNumTags}");
}