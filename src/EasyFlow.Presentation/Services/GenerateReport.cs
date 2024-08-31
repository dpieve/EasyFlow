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
using EasyFlow.Application.Sessions;
using EasyFlow.Presentation.Features.Dashboard;
using System.Globalization;

namespace EasyFlow.Presentation.Services;

public static class GenerateReportHandler
{
    public static async Task<Result<bool>> Handle(IMediator mediator, CancellationToken cancellationToken = new())
    {
        try
        {
            if (App.Current is null || App.Current.ApplicationLifetime is null)
            {
                return Result<bool>.Failure(GenerateReportErrors.Fail);
            }

            var topLevel = TopLevel.GetTopLevel(((IClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow);

            if (topLevel is null)
            {
                return Result<bool>.Failure(GenerateReportErrors.Fail);
            }

            var dateTime = DateTime.Now;
            var dateTimeString = dateTime.ToString(LanguageService.GetDateFormat());

            var fileOptions = new FilePickerSaveOptions()
            {
                Title = ConstantTranslation.ChooseWhereToSave,
                DefaultExtension = "csv",
                ShowOverwritePrompt = true,
                SuggestedFileName = $"EasyFlow-{ConstantTranslation.Report}-{dateTimeString}",
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

            return Result<bool>.Failure(GenerateReportErrors.Cancelled);
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);    
            return Result<bool>.Failure(GenerateReportErrors.Fail);
        }
    }

    private static async Task<bool> GenerateCsvFile(string path, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSessionsByPeriodQuery() { NumDays = FilterPeriod.Years5.NumDays }, cancellationToken);

        if (!result.IsSuccess)
        {
            return false;
        }

        var sessions = result.Value!;

        var csvContent = new StringBuilder();
        csvContent.AppendLine(ConstantTranslation.ReportColumns);

        foreach (var session in sessions)
        {
            var date = session.FinishedDate.ToString("yyyy-MM-dd");
            var duration = session.DurationMinutes.ToString(CultureInfo.CurrentCulture);
            var tag = session.Tag?.Name ?? "N/A";
            var sessionType = session.SessionType.ToCustomString();

            csvContent.AppendLine($"{date},{duration},{tag},{sessionType}");
        }

        await File.WriteAllTextAsync(path, csvContent.ToString(), cancellationToken);

        return true;
    }
}

public static class GenerateReportErrors
{
    public static readonly Error Fail = new("GenerateReport.Fail", "Failed to generate the report. Please try again.");
    public static readonly Error Cancelled = new("GenerateReport.Cancelled", "User cancelled the operation.");
}