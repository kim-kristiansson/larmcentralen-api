using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;
using Larmcentralen.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Larmcentralen.Infrastructure.Repositories;

public class AlarmRepository(AppDbContext db) : Repository<Alarm>(db), IAlarmRepository
{
    public async Task<List<Alarm>> SearchAsync(string? search, int? equipmentId, int? areaId, string? severity)
    {
        var query = Db.Alarms
            .Include(a => a.Equipment)
            .ThenInclude(e => e.Area)
            .AsQueryable();

        if (areaId.HasValue)
            query = query.Where(a => a.Equipment.AreaId == areaId.Value);

        if (equipmentId.HasValue)
            query = query.Where(a => a.EquipmentId == equipmentId.Value);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var terms = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            foreach (var term in terms)
            {
                var startsWith = $"{term}%";
                var wordStartsWith = $"% {term}%";
        
                query = query.Where(a =>
                    EF.Functions.ILike(a.Title, startsWith) ||
                    EF.Functions.ILike(a.Title, wordStartsWith) ||
                    (a.AlarmCode != null && (EF.Functions.ILike(a.AlarmCode, startsWith) || EF.Functions.ILike(a.AlarmCode, wordStartsWith))) ||
                    (a.Description != null && (EF.Functions.ILike(a.Description, startsWith) || EF.Functions.ILike(a.Description, wordStartsWith))) ||
                    EF.Functions.ILike(a.Equipment.Title, startsWith) ||
                    EF.Functions.ILike(a.Equipment.Title, wordStartsWith) ||
                    (a.Equipment.DisplayName != null && (EF.Functions.ILike(a.Equipment.DisplayName, startsWith) || EF.Functions.ILike(a.Equipment.DisplayName, wordStartsWith))) ||
                    EF.Functions.ILike(a.Equipment.Area.Title, startsWith) ||
                    EF.Functions.ILike(a.Equipment.Area.Title, wordStartsWith));
            }
        }
        
        if (!string.IsNullOrWhiteSpace(severity))
            query = query.Where(a => a.Severity == severity);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var firstTerm = search.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last();
            var startsWithPattern = $"{firstTerm}%";
    
            return await query
                .OrderByDescending(a => EF.Functions.ILike(a.Title, startsWithPattern))
                .ThenBy(a => a.Title)
                .Take(50)
                .ToListAsync();
        }

        return await query.OrderBy(a => a.Title).Take(50).ToListAsync();
    }

    public async Task<Alarm?> GetByIdWithDetailsAsync(int id)
    {
        return await Db.Alarms
            .Include(a => a.Equipment)
            .ThenInclude(e => e.Area)
            .Include(a => a.Solutions.OrderBy(s => s.SortOrder))
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> ExistsAsync(string alarmCode, int equipmentId)
    {
        return await Db.Alarms
            .AnyAsync(a => a.AlarmCode == alarmCode && a.EquipmentId == equipmentId);
    }
    
    public async Task<List<Alarm>> GetByIdsAsync(List<int> ids)
    {
        return await Db.Alarms
            .Include(a => a.Equipment)
            .ThenInclude(e => e.Area)
            .Where(a => ids.Contains(a.Id))
            .ToListAsync();
    }
}