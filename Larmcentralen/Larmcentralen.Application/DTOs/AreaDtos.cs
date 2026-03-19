namespace Larmcentralen.Application.DTOs;

public record AreaDtos(
    int Id,
    string Title,
    string? DisplayName,
    string? Description,
    string? Location,
    int EquipmentCount
);

public record CreateAreaDto(
    string Title,
    string? DisplayName,
    string? Description,
    string? Location
);

public record UpdateAreaDto(
    string Title,
    string? DisplayName,
    string? Description,
    string? Location
);