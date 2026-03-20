using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Larmcentralen.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Larmcentralen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlarmsController(IAlarmService service, ISolutionService solutionService) : ControllerBase
{
    [HttpGet]
    [HttpGet]
    public async Task<ActionResult<List<AlarmListDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? equipmentId,
        [FromQuery] string? severity)
        => Ok(await service.SearchAsync(search, equipmentId, severity));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AlarmDto>> Get(int id)
    {
        var alarm = await service.GetByIdAsync(id);
        return alarm is null ? NotFound() : Ok(alarm);
    }

    [HttpPost]
    public async Task<ActionResult<AlarmDto>> Create(CreateAlarmDto dto)
    {
        var alarm = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = alarm.Id }, alarm);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateAlarmDto dto)
        => await service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await service.DeleteAsync(id) ? NoContent() : NotFound();
    
    [HttpGet("{id}/export")]
    public async Task<IActionResult> ExportToWord(int id, [FromServices] ExportService exportService)
    {
        var result = await exportService.ExportAlarmToDocxAsync(id);
        if (result is null) return NotFound();

        var (bytes, fileName) = result.Value;
        return File(bytes, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", fileName);
    }
}