using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Larmcentralen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolutionsController(ISolutionService service) : ControllerBase
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
        return CreatedAtAction(nameof(Get), new { id = solution.Id }, solution);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSolutionDto dto)
        => await service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await service.DeleteAsync(id) ? NoContent() : NotFound();
}