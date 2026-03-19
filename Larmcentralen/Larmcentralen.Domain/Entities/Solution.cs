namespace Larmcentralen.Domain.Entities;

public class Alarm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? AlarmCode { get; set; }
    public string Severity { get; set; } = "Låg";
    public string? Description { get; set; }
    public string? Solution { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int EquipmentId { get; set; }
    public Equipment Equipment { get; set; } = null!;
}