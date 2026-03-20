using Larmcentralen.Application.Interfaces;
using Larmcentralen.Application.Services;

namespace Larmcentralen.Api.Services;

public class SharePointSyncService(
    ExportService exportService,
    SharePointUploadService uploadService,
    ILogger<SharePointSyncService> logger) : ISharePointSyncService
{
    public async Task SyncAlarmAsync(int alarmId)
    {
        try
        {
            var result = await exportService.ExportAlarmToDocxAsync(alarmId);
            if (result is null) return;

            var (bytes, fileName) = result.Value;
            await uploadService.UploadAsync(bytes, fileName, "Larmcentralen");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "SharePoint sync failed for alarm {AlarmId}", alarmId);
        }
    }
}