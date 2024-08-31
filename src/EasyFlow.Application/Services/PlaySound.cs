using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using MediatR;

namespace EasyFlow.Application.Services;

public sealed class PlaySoundQuery : IRequest<Result<bool>>
{
    public SoundType? SoundType { get; set; }
}

public sealed class PlaySoundQueryHandler : IRequestHandler<PlaySoundQuery, Result<bool>>
{
    private readonly IPlaySoundService _playSoundService;

    public PlaySoundQueryHandler(IPlaySoundService playSoundService)
    {
        _playSoundService = playSoundService;
    }

    public async Task<Result<bool>> Handle(PlaySoundQuery request, CancellationToken cancellationToken)
    {
        var played = await _playSoundService.Play(request.SoundType!.Value);
        return played ? Result<bool>.Success(true) : Result<bool>.Failure(PlaySoundErrors.PlayFail);
    }
}

public static partial class PlaySoundErrors
{
    public static readonly Error PlayFail = new Error("PlaySound.PlayFail", "Failed to play the sound.");
}