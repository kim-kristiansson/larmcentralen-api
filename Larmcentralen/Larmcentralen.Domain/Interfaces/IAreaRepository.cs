using Larmcentralen.Domain.Entities;

namespace Larmcentralen.Domain.Interfaces;

public interface IAreaRepository : IRepository<Area>
{
    Task<Area?> GetByIdWithEquipmentAsync(int id);
}