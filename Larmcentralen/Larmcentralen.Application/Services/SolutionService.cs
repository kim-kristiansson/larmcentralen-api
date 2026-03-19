using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Larmcentralen.Domain.Entities;
using Larmcentralen.Domain.Interfaces;

namespace Larmcentralen.Application.Services;

public class SolutionService(ISolutionRepository repo) : ISolutionService
{
    public async Task<List<SolutionDto>> GetByAlarmIdAsync(int alarmId)
    {
        var solutions = await repo.GetByAlarmIdAsync(alarmId);

        return solutions.Select(s => new SolutionDto(
            s.Id, s.Title, s.Content, s.SortOrder, s.EstimatedTime,
            s.AlarmId, s.CreatedAt, s.UpdatedAt
        )).ToList();
    }

    public async Task<SolutionDto?> GetByIdAsync(int id)
    {
        var s = await repo.GetByIdWithDetailsAsync(id);
        if (s is null) return null;

        return new SolutionDto(
            s.Id, s.Title, s.Content, s.SortOrder, s.EstimatedTime,
            s.AlarmId, s.CreatedAt, s.UpdatedAt
        );
    }

    public async Task<SolutionDto> CreateAsync(CreateSolutionDto dto)
    {
        var solution = new Solution
        {
            Title = dto.Title,
            Content = dto.Content,
            SortOrder = dto.SortOrder,
            EstimatedTime = dto.EstimatedTime,
            AlarmId = dto.AlarmId
        };

        await repo.AddAsync(solution);
        await repo.SaveChangesAsync();

        return new SolutionDto(
            solution.Id, solution.Title, solution.Content, solution.SortOrder,
            solution.EstimatedTime, solution.AlarmId, solution.CreatedAt, solution.UpdatedAt
        );
    }

    public async Task<bool> UpdateAsync(int id, UpdateSolutionDto dto)
    {
        var solution = await repo.GetByIdAsync(id);
        if (solution is null) return false;

        solution.Title = dto.Title;
        solution.Content = dto.Content;
        solution.SortOrder = dto.SortOrder;
        solution.EstimatedTime = dto.EstimatedTime;
        solution.UpdatedAt = DateTime.UtcNow;

        repo.Update(solution);
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var solution = await repo.GetByIdAsync(id);
        if (solution is null) return false;

        repo.Remove(solution);
        await repo.SaveChangesAsync();
        return true;
    }
}