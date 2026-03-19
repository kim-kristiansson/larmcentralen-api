namespace Larmcentralen.Application.DTOs;

public record SolutionDto(
    int Id,
    string Title,
    string? Content,
    int SortOrder,
    string? EstimatedTime,
    int AlarmId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);

public record CreateSolutionDto(
    string Title,
    string? Content,
    int SortOrder,
    string? EstimatedTime,
    int AlarmId
);

public record UpdateSolutionDto(
    string Title,
    string? Content,
    int SortOrder,
    string? EstimatedTime
);