namespace Larmcentralen.Maui.Models;

public class CreateAlarmDto
{
    public string Title { get; set; } = string.Empty;
    public string? AlarmCode { get; set; }
    public string Severity { get; set; } = "Låg";
    public string? Description { get; set; }
    public int EquipmentId { get; set; }
}