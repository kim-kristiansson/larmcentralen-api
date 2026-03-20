namespace Larmcentralen.Application.Interfaces;

public interface ISharePointSyncService
{
    Task SyncAlarmAsync(int alarmId);
}