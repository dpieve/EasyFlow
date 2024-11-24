using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using EasyFlow.Application.Common;
using EasyFlow.Desktop;
using EasyFlow.Domain.Entities;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EasyFlow.Desktop.Services;

public static class BackupDbQueryHandler
{
    public static async Task<Result<bool>> Handle(CancellationToken cancellationToken = new())
    {
        try
        {
            if (Avalonia.Application.Current is null || Avalonia.Application.Current.ApplicationLifetime is null)
            {
                return Result<bool>.Failure(BackupDbErrors.Fail);
            }

            var appLifeTime = (IClassicDesktopStyleApplicationLifetime)Avalonia.Application.Current.ApplicationLifetime;

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

            Trace.TraceInformation("BackupDb");
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "BackupDb {Error}", ex.Message);
            return Result<bool>.Failure(BackupDbErrors.Fail);
        }
    }
}

public static partial class BackupDbErrors
{
    public static readonly Error Fail = new($"BackupDb.Fail",
       "Failed to backup the db");
}