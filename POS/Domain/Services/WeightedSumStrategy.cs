using POS.Domain.Models;

namespace POS.Domain.Services;

/// <summary>
/// Weighted sum of normalized scores, scaled to 0..10:
/// final = sum(score_i / max_i * weight_i) / sum(weight_i) * 10.
/// </summary>
public sealed class WeightedSumStrategy : IFinalScoreStrategy
{
    public string Name => "Weighted sum (normalized, 0..10)";

    public FinalScore Calculate(IReadOnlyCollection<Evaluation> evaluations, Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        if (evaluations.Count == 0)
            return new FinalScore(0);

        double weighted = 0;
        double totalWeight = 0;

        foreach (var eval in evaluations)
        {
            if (!rubric.TryGetCriterion(eval.CriterionId, out var criterion) || criterion is null)
                throw new InvalidOperationException(
                    $"Evaluation {eval.Id} references criterion {eval.CriterionId} which is not in the rubric");

            weighted += eval.Score.Normalized * criterion.Weight.Value;
            totalWeight += criterion.Weight.Value;
        }

        if (totalWeight <= 0)
            return new FinalScore(0);

        return new FinalScore(weighted / totalWeight * 10.0);
    }
}