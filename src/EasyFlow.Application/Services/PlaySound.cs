using EasyFlow.Application.Common;
using EasyFlow.Application.Settings;
using EasyFlow.Domain.Entities;
using EasyFlow.Domain.Services;
using EasyFlow.Infrastructure.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EasyFlow.Application.Services;

public sealed class PlaySound
{
    public sealed class Command : IRequest<Result<Unit>>
    {
        public required SoundType SoundType { get; set; }
    }

    public sealed class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IPlaySoundService _playSoundService;

        public Handler(DataContext context, IPlaySoundService playSoundService)
        {
            _context = context;
            _playSoundService = playSoundService;
        }

        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var settings = await _context.GeneralSettings.FirstAsync();

            if (settings is null)
            {
                return Result<Unit>.Failure(SettingsErrors.NotFound);
            }

            var breakSounds = settings.IsBreakSoundEnabled;

            if (request.SoundType == SoundType.Break && !breakSounds)
            {
                return Result<Unit>.Failure(PlaySoundErrors.BreakSoundDisabled);
            }

            var workSounds = settings.IsWorkSoundEnabled;

            if (request.SoundType == SoundType.Work && !workSounds)
            {
                return Result<Unit>.Failure(PlaySoundErrors.FocusSoundDisabled);
            }

            var volume = settings.SoundVolume;
            if (volume == 0)
            {
                return Result<Unit>.Failure(PlaySoundErrors.Muted);
            }

            var played = await _playSoundService.Play(request.SoundType, volume);
            return played ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure(PlaySoundErrors.BadRequest);
        }
    }
}

public static partial class PlaySoundErrors
{
    public static readonly Error BadRequest = new("BadRequest", "Failed to play the sound.");
    public static readonly Error BreakSoundDisabled = new("BadRequest", "Can't play Break Sound because it is Disabled.");
    public static readonly Error FocusSoundDisabled = new("BadRequest", "Can't play Focus Sound because it is Disabled.");
    public static readonly Error Muted = new("BadRequest", "Can't play sounds because volume = 0.");
}