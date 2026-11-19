using POS.Domain.Models;

namespace POS.Domain.Services;

public sealed class RawSumStrategy : IFinalScoreStrategy
{
    public string Name => "Raw sum of scores";

    public FinalScore Calculate(IReadOnlyCollection<Evaluation> evaluations, Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        double sum = 0;
        foreach (var eval in evaluations)
        {
            if (!rubric.TryGetCriterion(eval.CriterionId, out var criterion) || criterion is null)
                throw new InvalidOperationException(
                    $"Evaluation {eval.Id} references criterion {eval.CriterionId} which is not in the rubric");
            sum += eval.Score.Value;
        }
        return new FinalScore(sum);
    }
}