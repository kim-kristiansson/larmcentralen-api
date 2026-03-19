using Larmcentralen.Application.DTOs;

namespace Larmcentralen.Application.Interfaces;

public interface IAreaService
{
    Task<List<AreaDto>> GetAllAsync();
    Task<AreaDto?> GetByIdAsync(int id);
    Task<AreaDto> CreateAsync(CreateAreaDto dto);
    Task<bool> UpdateAsync(int id, UpdateAreaDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IEquipmentService
{
    Task<List<EquipmentDto>> GetAllAsync(int? areaId);
    Task<EquipmentDto?> GetByIdAsync(int id);
    Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto);
    Task<bool> UpdateAsync(int id, UpdateEquipmentDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface IAlarmService
{
    Task<List<AlarmListDto>> SearchAsync(string? search, int? equipmentId, string? severity);
    Task<AlarmDto?> GetByIdAsync(int id);
    Task<AlarmDto> CreateAsync(CreateAlarmDto dto);
    Task<bool> UpdateAsync(int id, UpdateAlarmDto dto);
    Task<bool> DeleteAsync(int id);
}

public interface ISolutionService
{
    Task<List<SolutionDto>> GetByAlarmIdAsync(int alarmId);
    Task<SolutionDto?> GetByIdAsync(int id);
    Task<SolutionDto> CreateAsync(CreateSolutionDto dto);
    Task<bool> UpdateAsync(int id, UpdateSolutionDto dto);
    Task<bool> DeleteAsync(int id);
}