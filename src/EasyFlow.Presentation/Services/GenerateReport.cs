using EasyFlow.Application.Common;
using System.Threading.Tasks;
using System.Threading;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Diagnostics;
using System;
using System.Text;
using System.IO;
using MediatR;
using EasyFlow.Domain.Entities;
using EasyFlow.Application.Sessions;

namespace EasyFlow.Presentation.Services;

public static class GenerateReportHandler
{
    public static async Task<Result<bool>> Handle(IMediator mediator, CancellationToken cancellationToken = new())
    {
        var topLevel = TopLevel.GetTopLevel(((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow);

        if (topLevel is null)
        {
            return Result<bool>.Success(false);
        }

        var dateTime = DateTime.Now;
        var dateTimeString = dateTime.ToString("MMM-dd-yyyy");

        var fileOptions = new FilePickerSaveOptions()
        {
            Title = "Choose a name and a path to save the report file",
            DefaultExtension = "csv",
            ShowOverwritePrompt = true,
            SuggestedFileName = $"EasyFlow-Report-{dateTimeString}",
            FileTypeChoices = [
                new("Report file (.csv)")
                    {
                        Patterns = [ "csv" ],
                        MimeTypes = ["csv" ]
                    }
            ]
        };

        var files = await topLevel.StorageProvider.SaveFilePickerAsync(fileOptions);

        if (files is not null)
        {
            var path = files.TryGetLocalPath();

            if (path is not null)
            {
                var result = await GenerateCsvFile(path, mediator, cancellationToken);
                return Result<bool>.Success(result);
            }
            else
            {
                Debug.WriteLine("Couldn't find the path to backup the file");
            }

        }

        return Result<bool>.Success(true);
    }

    private static async Task<bool> GenerateCsvFile(string path, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSessionsByPeriodQuery() { FilterPeriod = FilterPeriod.Years5 }, cancellationToken);

        if (!result.IsSuccess)
        {
            return false;
        }

        var sessions = result.Value!;

        var csvContent = new StringBuilder();
        csvContent.AppendLine("Date,Duration (Minutes),Tag,Session Type");

        foreach (var session in sessions)
        {
            var date = session.FinishedDate.ToString("yyyy-MM-dd");
            var duration = session.DurationMinutes.ToString();
            var tag = session.Tag?.Name ?? "N/A";
            var sessionType = session.SessionType.ToString();

            csvContent.AppendLine($"{date},{duration},{tag},{sessionType}");
        }

        await File.WriteAllTextAsync(path, csvContent.ToString(), cancellationToken);

        return true;
    }
}