namespace Larmcentralen.Maui.Models;

public class SolutionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int SortOrder { get; set; }
    public string? EstimatedTime { get; set; }
    public int AlarmId { get; set; }
}