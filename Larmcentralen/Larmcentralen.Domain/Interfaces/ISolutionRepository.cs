using Larmcentralen.Domain.Entities;

namespace Larmcentralen.Domain.Interfaces;

public interface ISolutionRepository : IRepository<Solution>
{
    Task<List<Solution>> GetByAlarmIdAsync(int alarmId);
    Task<Solution?> GetByIdWithDetailsAsync(int id);
}