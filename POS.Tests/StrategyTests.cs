using Xunit;

using POS.Domain.Models;
using POS.Domain.Services;

namespace POS.Tests;

public class StrategyTests
{
    private static (Rubric rubric, Criterion a, Criterion b) NewRubricTwoCriteria()
    {
        var rubric = new Rubric("КПО");
        var a = rubric.AddCriterion("Лабы", new MaxScore(10), new Weight(1));
        var b = rubric.AddCriterion("Экзамен", new MaxScore(10), new Weight(1));
        return (rubric, a, b);
    }

    private static Evaluation Eval(Guid sid, Criterion c, double score)
        => new(Guid.NewGuid(), sid, c.Id, new Score(score, c.Max), DateOnly.FromDateTime(DateTime.UtcNow));

    [Fact]
    public void WeightedSum_EqualWeights_AveragesNormalizedScores()
    {
        var (rubric, a, b) = NewRubricTwoCriteria();
        var sid = Guid.NewGuid();
        var evals = new[] { Eval(sid, a, 8), Eval(sid, b, 6) };
        var strategy = new WeightedSumStrategy();
        // (8 + 6) / 2 = 7
        var result = strategy.Calculate(evals, rubric);
        Assert.Equal(7.0, result.Value, 5);
    }

    [Fact]
    public void WeightedSum_DifferentWeights_WeightedAverage()
    {
        var rubric = new Rubric("КПО");
        var a = rubric.AddCriterion("Лабы", new MaxScore(10), new Weight(2));
        var b = rubric.AddCriterion("Экзамен", new MaxScore(5), new Weight(1));
        var sid = Guid.NewGuid();
        var evals = new[] { Eval(sid, a, 8), Eval(sid, b, 4) };
        var strategy = new WeightedSumStrategy();
        var result = strategy.Calculate(evals, rubric);
        Assert.Equal(8.0, result.Value, 5);
    }

    [Fact]
    public void WeightedSum_NoEvaluations_ReturnsZero()
    {
        var (rubric, _, _) = NewRubricTwoCriteria();
        var result = new WeightedSumStrategy().Calculate(Array.Empty<Evaluation>(), rubric);
        Assert.Equal(0, result.Value);
    }

    [Fact]
    public void Strategy_RejectsEvaluationForCriterionNotInRubric()
    {
        var rubric = new Rubric("КПО");
        var orphanCriterion = new Criterion(Guid.NewGuid(), "Удалённый", new MaxScore(10), new Weight(1));
        var eval = new Evaluation(Guid.NewGuid(), Guid.NewGuid(), orphanCriterion.Id,
            new Score(5, orphanCriterion.Max), DateOnly.FromDateTime(DateTime.UtcNow));
        Assert.Throws<InvalidOperationException>(() => new WeightedSumStrategy().Calculate(new[] { eval }, rubric));
    }

    [Fact]
    public void RawSum_SumsRawScores()
    {
        var (rubric, a, b) = NewRubricTwoCriteria();
        var sid = Guid.NewGuid();
        var evals = new[] { Eval(sid, a, 8), Eval(sid, b, 6) };
        var result = new RawSumStrategy().Calculate(evals, rubric);
        Assert.Equal(14, result.Value);
    }

    [Fact]
    public void Strategies_AreInterchangeable_ProduceDifferentButValidResults()
    {
        var (rubric, a, b) = NewRubricTwoCriteria();
        var sid = Guid.NewGuid();
        var evals = new[] { Eval(sid, a, 8), Eval(sid, b, 6) };
        var weighted = new WeightedSumStrategy().Calculate(evals, rubric).Value;
        var raw = new RawSumStrategy().Calculate(evals, rubric).Value;
        Assert.NotEqual(weighted, raw);
        Assert.True(weighted >= 0 && raw >= 0);
    }
}