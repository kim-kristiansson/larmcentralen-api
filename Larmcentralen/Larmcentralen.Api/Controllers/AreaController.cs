using Larmcentralen.Application.DTOs;
using Larmcentralen.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Larmcentralen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AreasController(IAreaService service) : ControllerBase
{
    private readonly IAreaService _service = service;

    [HttpGet]
    public async Task<ActionResult<List<AreaDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AreaDto>> Get(int id)
    {
        var area = await _service.GetByIdAsync(id);
        return area is null ? NotFound() : Ok(area);
    }

    [HttpPost]
    public async Task<ActionResult<AreaDto>> Create(CreateAreaDto dto)
    {
        var area = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = area.Id }, area);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateAreaDto dto)
        => await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _service.DeleteAsync(id) ? NoContent() : NotFound();
}