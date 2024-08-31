using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Repositories;
using MediatR;

namespace EasyFlow.Application.Settings;

public sealed class GetSettingsQuery : IRequest<Result<GeneralSettings>>
{
}

public sealed class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, Result<GeneralSettings>>
{
    private readonly IGeneralSettingsRepository _generalSettingsRepository;

    public GetSettingsQueryHandler(IGeneralSettingsRepository generalSettingsRepository)
    {
        _generalSettingsRepository = generalSettingsRepository;
    }

    public async Task<Result<GeneralSettings>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _generalSettingsRepository.GetAsync();
        if (settings.Count == 0)
        {
            return Result<GeneralSettings>.Failure(GeneralSettingsErrors.GetFail);
        }
        return Result<GeneralSettings>.Success(settings[0]);
    }
}

public static partial class GeneralSettingsErrors
{
    public static readonly Error GetFail = new($"GeneralSettings.GetFail",
       "Failed to find the settings");
}