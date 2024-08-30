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
    private readonly IGeneralSettingsRepository _generalSettingsRepository;

    public DeleteTagCommandHandler(ITagsRepository tagsRepository, IGeneralSettingsRepository generalSettingsRepository)
    {
        _tagsRepository = tagsRepository;
        _generalSettingsRepository = generalSettingsRepository;
    }

    public async Task<Result<bool>> Handle(DeleteTagCommand request, CancellationToken cancellationToken)
    {
        var tag = request.Tag!;

        var allSettings = await _generalSettingsRepository.GetAsync();
        var settings = allSettings[0];
        if (settings.SelectedTagId == tag.Id)
        {
            return Result<bool>.Failure(TagsErrors.CanNotDeleteSelectedTag);
        }

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
    public static readonly Error DeleteFail = new(@"Tag_CouldNotDelete",
       "It couldn't delete the tag");

    public static readonly Error CanNotDeleteSelectedTag = new(@"Tag_CanNotDeleteSelectedTag",
       "You can't delete the selected tag");
}