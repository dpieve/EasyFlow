using EasyFlow.Domain.Entities;
using FluentValidation;

namespace EasyFlow.Application.Tags;

public sealed class TagsValidator : AbstractValidator<Tag>
{
    public TagsValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}