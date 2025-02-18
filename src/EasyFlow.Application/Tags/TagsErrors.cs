using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;

namespace EasyFlow.Application.Tags;

public static class TagsErrors
{
    public static readonly Error BadRequest = new("BadRequest",
       "Invalid request");

    public static readonly Error NotFound = new("NotFound",
       "Tag was not found");

    public static readonly Error CanNotMoreThanMax = new("CanNotMoreThanMax",
        $"There is a limit of {Tag.MaxNumTags}");

    public static readonly Error CreateFail = new("CreateFail",
        "Failed to create the tag");

    public static readonly Error CanNotDeleteSelectedTag = new(@"Tag_CanNotDeleteSelectedTag",
       "You can't delete the selected tag");

    public static readonly Error DeleteFail = new(@"Tag_CouldNotDelete",
        "It couldn't delete the tag");

    public static readonly Error EditFail = new("EditFail",
        "Failed to update the tag");
}