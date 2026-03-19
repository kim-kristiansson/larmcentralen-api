using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;

namespace Larmcentralen.Application.Services;

public class AreaService(IAreaRepository repo) : IAreaService
{
    public async Task<List<AreaDto>> GetAllAsync()
    {
        var areas = await repo.GetAllAsync();
        return areas.Select(a => new AreaDto(
            a.Id, a.Title, a.DisplayName, a.Description, a.Location,
            a.Equipment.Count
        )).ToList();
    }

    public async Task<AreaDto?> GetByIdAsync(int id)
    {
        var area = await repo.GetByIdWithEquipmentAsync(id);
        if (area is null) return null;

        return new AreaDto(
            area.Id, area.Title, area.DisplayName, area.Description, area.Location,
            area.Equipment.Count
        );
    }

    public async Task<AreaDto> CreateAsync(CreateAreaDto dto)
    {
        var area = new Area
        {
            Title = dto.Title,
            DisplayName = dto.DisplayName,
            Description = dto.Description,
            Location = dto.Location
        };

        await repo.AddAsync(area);
        await repo.SaveChangesAsync();

        return new AreaDto(area.Id, area.Title, area.DisplayName, area.Description, area.Location, 0);
    }

    public async Task<bool> UpdateAsync(int id, UpdateAreaDto dto)
    {
        var area = await repo.GetByIdAsync(id);
        if (area is null) return false;

        area.Title = dto.Title;
        area.DisplayName = dto.DisplayName;
        area.Description = dto.Description;
        area.Location = dto.Location;
        area.UpdatedAt = DateTime.UtcNow;

        repo.Update(area);
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var area = await repo.GetByIdAsync(id);
        if (area is null) return false;

        repo.Remove(area);
        await repo.SaveChangesAsync();
        return true;
    }
}