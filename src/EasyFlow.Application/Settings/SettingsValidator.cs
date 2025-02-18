using FluentValidation;

namespace EasyFlow.Application.Settings;

public sealed class SettingsValidator
    : AbstractValidator<Domain.Entities.GeneralSettings>
{
    public SettingsValidator()
    {
        RuleFor(x => x.SelectedTag).NotNull();
    }
}