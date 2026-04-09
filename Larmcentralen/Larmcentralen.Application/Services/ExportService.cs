using Larmcentralen.Application.Interfaces;

namespace Larmcentralen.Application.Services;

public class ExportService(IAlarmService alarmService, ISolutionService solutionService)
{
    public async Task<(byte[] Bytes, string FileName)?> ExportAlarmToDocxAsync(int alarmId)
    {
        return ([], "foo");
    }
}