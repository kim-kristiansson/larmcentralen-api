using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;
using Larmcentralen.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Larmcentralen.Infrastructure.Repositories;

public class EquipmentRepository(AppDbContext db) : Repository<Equipment>(db), IEquipmentRepository
{
    public async Task<List<Equipment>> GetByAreaIdAsync(int areaId)
    {
        return await Db.Equipment
            .Include(e => e.Area)
            .Include(e => e.Alarms)
            .Where(e => e.AreaId == areaId)
            .OrderBy(e => e.Title)
            .ToListAsync();
    }

    public async Task<Equipment?> GetByIdWithDetailsAsync(int id)
    {
        return await Db.Equipment
            .Include(e => e.Area)
            .Include(e => e.Alarms)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Equipment?> GetByTitleAsync(string title)
    {
        return await Db.Equipment
            .FirstOrDefaultAsync(e => e.Title == title);
    }
}