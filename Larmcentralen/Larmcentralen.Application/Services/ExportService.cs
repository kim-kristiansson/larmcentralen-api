using Larmcentralen.Application.Interfaces;

namespace Larmcentralen.Application.Services;

public class ExportService(IAlarmService alarmService, ISolutionService solutionService)
{
    private readonly MarkdownToWordConverter _converter = new();

    public async Task<(byte[] Bytes, string FileName)?> ExportAlarmToDocxAsync(int alarmId)
    {
        var alarm = await alarmService.GetByIdAsync(alarmId);
        if (alarm is null) return null;

        var solutions = await solutionService.GetByAlarmIdAsync(alarmId);

        var md = "";
        foreach (var solution in solutions)
        {
            md += $"## {solution.Title}\n\n";
            if (!string.IsNullOrWhiteSpace(solution.EstimatedTime))
                md += $"*Uppskattad tid: {solution.EstimatedTime}*\n\n";
            if (!string.IsNullOrWhiteSpace(solution.Content))
                md += $"{solution.Content}\n\n";
            md += "---\n\n";
        }

        var bytes = _converter.Convert(md, alarm.Title);
        var fileName = $"{alarm.Title.Replace(" ", "_")}.docx";

        return (bytes, fileName);
    }
}