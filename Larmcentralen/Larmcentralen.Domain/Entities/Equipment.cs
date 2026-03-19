namespace Larmcentralen.Domain.Entities;

public class Equipment
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int AreaId { get; set; }
    public Area Area { get; set; } = null!;

    public List<Alarm> Alarms { get; set; } = [];
}