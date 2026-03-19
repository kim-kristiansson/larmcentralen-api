namespace Larmcentralen.Domain.Entities;

public class Area
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public List<Equipment> Equipment { get; set; } = [];
}