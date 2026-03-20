namespace Larmcentralen.Maui.Models;

public class AlarmListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AlarmCode { get; set; }
    public string Severity { get; set; } = string.Empty;
    public string EquipmentTitle { get; set; } = string.Empty;
    public string AreaTitle { get; set; } = string.Empty;
}