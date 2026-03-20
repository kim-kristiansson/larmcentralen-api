using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Larmcentralen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolutionsController(
    ISolutionService service,
    ISharePointSyncService syncService) : ControllerBase
{
    [HttpGet("by-alarm/{alarmId:int}")]
    public async Task<ActionResult<List<SolutionDto>>> GetByAlarm(int alarmId)
        => Ok(await service.GetByAlarmIdAsync(alarmId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SolutionDto>> Get(int id)
    {
        var solution = await service.GetByIdAsync(id);
        return solution is null ? NotFound() : Ok(solution);
    }

    [HttpPost]
    public async Task<ActionResult<SolutionDto>> Create(CreateSolutionDto dto)
    {
        var solution = await service.CreateAsync(dto);
        await syncService.SyncAlarmAsync(dto.AlarmId);
        return CreatedAtAction(nameof(Get), new { id = solution.Id }, solution);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSolutionDto dto)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        var updated = await service.UpdateAsync(id, dto);
        if (updated) await syncService.SyncAlarmAsync(existing.AlarmId);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null) return NotFound();

        var deleted = await service.DeleteAsync(id);
        if (deleted) await syncService.SyncAlarmAsync(existing.AlarmId);
        return deleted ? NoContent() : NotFound();
    }
}