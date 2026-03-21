using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;

namespace Larmcentralen.Application.Services;

public class AlarmService(IAlarmRepository repo) : IAlarmService
{
    public async Task<List<AlarmListDto>> SearchAsync(string? search, int? equipmentId, int? areaId, string? severity, int skip = 0, int take = 10)
    {
        var alarms = await repo.SearchAsync(search, equipmentId, areaId, severity, skip, take);

        return alarms.Select(a => new AlarmListDto(
            a.Id, a.Title, a.AlarmCode, a.Severity,
            a.Equipment.Title, a.Equipment.Area.Title
        )).ToList();
    }

    public async Task<AlarmDto?> GetByIdAsync(int id)
    {
        var a = await repo.GetByIdWithDetailsAsync(id);
        if (a is null) return null;

        return new AlarmDto(
            a.Id, a.Title, a.AlarmCode, a.Severity, a.Description,
            a.EquipmentId, a.Equipment.Title, a.Equipment.Area.Title,
            a.CreatedAt, a.UpdatedAt,
            a.Solutions.Select(s => new SolutionDto(
                s.Id, s.Title, s.Content, s.SortOrder, s.EstimatedTime,
                s.AlarmId, s.CreatedAt, s.UpdatedAt
            )).ToList()
        );
    }

    public async Task<AlarmDto> CreateAsync(CreateAlarmDto dto)
    {
        var alarm = new Alarm
        {
            Title = dto.Title,
            AlarmCode = dto.AlarmCode,
            Severity = dto.Severity,
            Description = dto.Description,
            EquipmentId = dto.EquipmentId
        };

        await repo.AddAsync(alarm);
        await repo.SaveChangesAsync();

        var created = await repo.GetByIdWithDetailsAsync(alarm.Id);
        return new AlarmDto(
            created!.Id, created.Title, created.AlarmCode, created.Severity, created.Description,
            created.EquipmentId, created.Equipment.Title, created.Equipment.Area.Title,
            created.CreatedAt, created.UpdatedAt, []
        );
    }

    public async Task<bool> UpdateAsync(int id, UpdateAlarmDto dto)
    {
        var alarm = await repo.GetByIdAsync(id);
        if (alarm is null) return false;

        alarm.Title = dto.Title;
        alarm.AlarmCode = dto.AlarmCode;
        alarm.Severity = dto.Severity;
        alarm.Description = dto.Description;
        alarm.EquipmentId = dto.EquipmentId;
        alarm.UpdatedAt = DateTime.UtcNow;

        repo.Update(alarm);
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alarm = await repo.GetByIdAsync(id);
        if (alarm is null) return false;

        repo.Remove(alarm);
        await repo.SaveChangesAsync();
        return true;
    }
    
    public async Task<List<AlarmListDto>> GetByIdsAsync(List<int> ids)
    {
        var alarms = await repo.GetByIdsAsync(ids);

        return ids
            .Select(id => alarms.FirstOrDefault(a => a.Id == id))
            .Where(a => a is not null)
            .Select(a => new AlarmListDto(
                a!.Id, a.Title, a.AlarmCode, a.Severity,
                a.Equipment.Title, a.Equipment.Area.Title
            )).ToList();
    }
}