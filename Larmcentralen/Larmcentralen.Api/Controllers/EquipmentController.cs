using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Larmcentralen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EquipmentController(IEquipmentService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<EquipmentDto>>> GetAll([FromQuery] int? areaId)
        => Ok(await service.GetAllAsync(areaId));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EquipmentDto>> Get(int id)
    {
        var equipment = await service.GetByIdAsync(id);
        return equipment is null ? NotFound() : Ok(equipment);
    }

    [HttpPost]
    public async Task<ActionResult<EquipmentDto>> Create(CreateEquipmentDto dto)
    {
        var equipment = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = equipment.Id }, equipment);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateEquipmentDto dto)
        => await service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await service.DeleteAsync(id) ? NoContent() : NotFound();
}