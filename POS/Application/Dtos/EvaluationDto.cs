namespace POS.Application.Dtos;

public sealed record EvaluationDto(
    Guid Id,
    Guid StudentId,
    Guid CriterionId,
    string CriterionName,
    double Score,
    int Max,
    DateOnly Date,
    string? Comment)
{
    public override string ToString() => $"[{Date}] {CriterionName}: {Score}/{Max}" + (Comment is null ? "" : $" — {Comment}");
}