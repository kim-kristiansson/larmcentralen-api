namespace Larmcentralen.Maui.Models;

public class UpdateSolutionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public int SortOrder { get; set; }
    public string? EstimatedTime { get; set; }
}