namespace Larmcentralen.Maui.Models;

public class AlarmDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AlarmCode { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EquipmentId { get; set; }
    public string EquipmentTitle { get; set; } = string.Empty;
    public string AreaTitle { get; set; } = string.Empty;
    public List<SolutionDto> Solutions { get; set; } = [];
}