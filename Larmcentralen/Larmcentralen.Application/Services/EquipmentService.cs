using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;

namespace Larmcentralen.Application.Services;

public class EquipmentService(IEquipmentRepository repo) : IEquipmentService
{
    public async Task<List<EquipmentDto>> GetAllAsync(int? areaId)
    {
        var items = areaId.HasValue
            ? await repo.GetByAreaIdAsync(areaId.Value)
            : await repo.GetAllAsync();

        return items.Select(e => new EquipmentDto(
            e.Id, e.Title, e.DisplayName, e.Category, e.Description,
            e.AreaId, e.Area.Title, e.Alarms.Count
        )).ToList();
    }

    public async Task<EquipmentDto?> GetByIdAsync(int id)
    {
        var e = await repo.GetByIdWithDetailsAsync(id);
        if (e is null) return null;

        return new EquipmentDto(
            e.Id, e.Title, e.DisplayName, e.Category, e.Description,
            e.AreaId, e.Area.Title, e.Alarms.Count
        );
    }

    public async Task<EquipmentDto> CreateAsync(CreateEquipmentDto dto)
    {
        var equipment = new Equipment
        {
            Title = dto.Title,
            DisplayName = dto.DisplayName,
            Category = dto.Category,
            Description = dto.Description,
            AreaId = dto.AreaId
        };

        await repo.AddAsync(equipment);
        await repo.SaveChangesAsync();

        var created = await repo.GetByIdWithDetailsAsync(equipment.Id);
        return new EquipmentDto(
            created!.Id, created.Title, created.DisplayName, created.Category, created.Description,
            created.AreaId, created.Area.Title, 0
        );
    }

    public async Task<bool> UpdateAsync(int id, UpdateEquipmentDto dto)
    {
        var equipment = await repo.GetByIdAsync(id);
        if (equipment is null) return false;

        equipment.Title = dto.Title;
        equipment.DisplayName = dto.DisplayName;
        equipment.Category = dto.Category;
        equipment.Description = dto.Description;
        equipment.AreaId = dto.AreaId;
        equipment.UpdatedAt = DateTime.UtcNow;

        repo.Update(equipment);
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var equipment = await repo.GetByIdAsync(id);
        if (equipment is null) return false;

        repo.Remove(equipment);
        await repo.SaveChangesAsync();
        return true;
    }
}