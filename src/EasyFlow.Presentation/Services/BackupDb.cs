using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using EasyFlow.Application.Common;
using EasyFlow.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace EasyFlow.Presentation.Services;

public static class BackupDbQueryHandler 
{
    public static async Task<Result<bool>> Handle(CancellationToken cancellationToken = new())
    {
        try
        {
            if (App.Current is null || App.Current.ApplicationLifetime is null)
            {
                return Result<bool>.Failure(BackupDbErrors.Fail);
            }

            var appLifeTime = (IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime;
            
            if (appLifeTime is null)
            {
                return Result<bool>.Failure(BackupDbErrors.Fail);
            }

            var topLevel = TopLevel.GetTopLevel(appLifeTime.MainWindow);

            if (topLevel is null)
            {
                return Result<bool>.Failure(BackupDbErrors.Fail);
            }

            var fileOptions = new FilePickerSaveOptions()
            {
                Title = ConstantTranslation.ChooseWhereToSave,
                DefaultExtension = "ds",
                ShowOverwritePrompt = true,
                SuggestedFileName = "EasyFlow",
                FileTypeChoices = [
                    new("Database file (.ds)")
                    {
                        Patterns = [ "ds" ],
                        MimeTypes = ["ds" ]
                    }
                ]
            };

            var files = await topLevel.StorageProvider.SaveFilePickerAsync(fileOptions);

            if (files is null)
            {
                return Result<bool>.Failure(BackupDbErrors.Fail);
            }

            var newDbPath = files.TryGetLocalPath();

            if (newDbPath is null)
            {
                return Result<bool>.Failure(BackupDbErrors.Fail);
            }

            var currentDbPath = Paths.DbFullPath;

            if (!File.Exists(currentDbPath))
            {
                return Result<bool>.Failure(BackupDbErrors.Fail);
            }

            File.Copy(currentDbPath, newDbPath, overwrite: true);

            return Result<bool>.Success(true);
        }
        catch (Exception)
        {
            return Result<bool>.Failure(BackupDbErrors.Fail);
        }
    }
}

public static partial class BackupDbErrors
{
    public static readonly Error Fail = new($"BackupDb.Fail",
       "Failed to backup the db");
}
