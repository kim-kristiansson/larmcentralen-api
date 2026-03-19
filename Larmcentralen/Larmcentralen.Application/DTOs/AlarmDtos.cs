namespace Larmcentralen.Application.DTOs;

public record AlarmDto(
    int Id,
    string Title,
    string? AlarmCode,
    string Severity,
    string? Description,
    int EquipmentId,
    string EquipmentTitle,
    string AreaTitle,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    List<SolutionDto> Solutions
);

public record AlarmListDto(
    int Id,
    string Title,
    string? AlarmCode,
    string Severity,
    string EquipmentTitle,
    string AreaTitle
);

public record CreateAlarmDto(
    string Title,
    string? AlarmCode,
    string Severity,
    string? Description,
    int EquipmentId
);

public record UpdateAlarmDto(
    string Title,
    string? AlarmCode,
    string Severity,
    string? Description,
    int EquipmentId
);