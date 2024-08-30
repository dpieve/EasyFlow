using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Settings;
public sealed class UpdateSettingsCommand : IRequest<Result<bool>>
{
    public GeneralSettings? GeneralSettings { get; init; }
}

public sealed class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, Result<bool>>
{
    private readonly IGeneralSettingsRepository _generalSettingsRepository;

    public UpdateSettingsCommandHandler(IGeneralSettingsRepository generalSettingsRepository)
    {
        _generalSettingsRepository = generalSettingsRepository;
    }

    public async Task<Result<bool>> Handle(UpdateSettingsCommand request, CancellationToken cancellationToken)
    {
        var settings = request.GeneralSettings!;
        var success = await _generalSettingsRepository.UpdateAsync(settings);
        return success ? Result<bool>.Success(true) : Result<bool>.Failure(GeneralSettingsErrors.UpdateFail);
    }
}

public static partial class GeneralSettingsErrors
{
    public static readonly Error UpdateFail = new($"GeneralSettings.UpdateFail",
       "Failed to update the settings");
}
