namespace Larmcentralen.Maui.Models;

public class EquipmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public int AreaId { get; set; }
    public string AreaTitle { get; set; } = string.Empty;
}