using Larmcentralen.Domain.Entities;

namespace Larmcentralen.Domain.Interfaces;

public interface IAlarmRepository : IRepository<Alarm>
{
    Task<List<Alarm>> SearchAsync(string? search, int? equipmentId, int? areaId, string? severity);
    Task<Alarm?> GetByIdWithDetailsAsync(int id);
    Task<bool> ExistsAsync(string alarmCode, int equipmentId);
    Task<List<Alarm>> GetByIdsAsync(List<int> ids);
}