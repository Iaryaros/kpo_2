using POS.Domain.Models;

namespace POS.Domain.Factories;

public sealed class EvaluationFactory
{
    private readonly Rubric _rubric;

    public EvaluationFactory(Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        _rubric = rubric;
    }

    public Evaluation Create(Guid studentId, Guid criterionId, double score, string? comment = null, DateOnly? date = null)
    {
        if (!_rubric.TryGetCriterion(criterionId, out var criterion) || criterion is null)
            throw new InvalidOperationException(
                $"Cannot create evaluation: criterion {criterionId} is not in the rubric");

        var safeScore = new Score(score, criterion.Max);
        var evalDate = date ?? DateOnly.FromDateTime(DateTime.UtcNow);

        return new Evaluation(Guid.NewGuid(), studentId, criterionId, safeScore, evalDate, comment);
    }
}