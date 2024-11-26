using EasyFlow.Domain.Entities;
using FluentValidation;

namespace EasyFlow.Application.Sessions;
public sealed class SessionValidator : AbstractValidator<Session>
{
    public SessionValidator()
    {
        RuleFor(x => x.Description).NotEmpty().NotNull();
    }
}
