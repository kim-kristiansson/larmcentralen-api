using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;
using Larmcentralen.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Larmcentralen.Infrastructure.Repositories;

public class AreaRepository(AppDbContext db) : Repository<Area>(db), IAreaRepository
{
    public async Task<Area?> GetByIdWithEquipmentAsync(int id)
    {
        return await Db.Areas
            .Include(a => a.Equipment)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}