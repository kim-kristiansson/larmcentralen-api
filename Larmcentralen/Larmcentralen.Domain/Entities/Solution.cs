namespace Larmcentralen.Domain.Entities;

public class Solution
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }              // Markdown
    public int SortOrder { get; set; } = 10;
    public string? EstimatedTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public int AlarmId { get; set; }
    public Alarm Alarm { get; set; } = null!;
}