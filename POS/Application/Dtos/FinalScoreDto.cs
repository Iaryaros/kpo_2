namespace POS.Application.Dtos;

// Итог по студенту: оценка, имя, стратегия расчёта, значение итога и разбивка по оценкам.
public sealed record FinalScoreDto(
    Guid StudentId,
    string StudentName,
    string Group,
    string StrategyName,
    double Value,
    IReadOnlyList<EvaluationDto> Evaluations)
{
    public override string ToString() => $"{StudentName} ({Group}) — итог {Value:0.##} [{StrategyName}]";
}