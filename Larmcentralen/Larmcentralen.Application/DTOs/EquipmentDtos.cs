namespace Larmcentralen.Application.DTOs;

public record EquipmentDtos(
    int Id,
    string Title,
    string? DisplayName,
    string? Category,
    string? Description,
    int AreaId,
    string AreaTitle,
    int AlarmCount
);

public record CreateEquipmentDto(
    string Title,
    string? DisplayName,
    string? Category,
    string? Description,
    int AreaId
);

public record UpdateEquipmentDto(
    string Title,
    string? DisplayName,
    string? Category,
    string? Description,
    int AreaId
);