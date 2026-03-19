using Larmcentralen.Domain.Entities;

namespace Larmcentralen.Domain.Interfaces;

public interface IEquipmentRepository : IRepository<Equipment>
{
    Task<List<Equipment>> GetByAreaIdAsync(int areaId);
    Task<Equipment?> GetByIdWithDetailsAsync(int id);
    Task<Equipment?> GetByTitleAsync(string title);
}