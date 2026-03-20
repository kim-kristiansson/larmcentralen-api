namespace Larmcentralen.Maui.Models;

public class CreateSolutionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int SortOrder { get; set; } = 10;
    public string? EstimatedTime { get; set; }
    public int AlarmId { get; set; }
}