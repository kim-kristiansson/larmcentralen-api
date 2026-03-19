using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;
using Larmcentralen.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Larmcentralen.Infrastructure.Repositories;

public class SolutionRepository(AppDbContext db) : Repository<Solution>(db), ISolutionRepository
{
    public async Task<List<Solution>> GetByAlarmIdAsync(int alarmId)
    {
        return await Db.Solutions
            .Where(s => s.AlarmId == alarmId)
            .OrderBy(s => s.SortOrder)
            .ToListAsync();
    }

    public async Task<Solution?> GetByIdWithDetailsAsync(int id)
    {
        return await Db.Solutions
            .Include(s => s.Alarm)
            .ThenInclude(a => a.Equipment)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}