using Xunit;

using POS.Domain.Factories;
using POS.Domain.Models;

namespace POS.Tests;

public class EvaluationFactoryTests
{
    private static Rubric NewRubricWithOneCriterion(out Guid criterionId)
    {
        var rubric = new Rubric("КПО");
        var c = rubric.AddCriterion("Лабы", new MaxScore(10), new Weight(1));
        criterionId = c.Id;
        return rubric;
    }

    [Fact]
    public void Create_RejectsUnknownCriterion()
    {
        var factory = new EvaluationFactory(new Rubric("КПО"));
        Assert.Throws<InvalidOperationException>(
            () => factory.Create(Guid.NewGuid(), Guid.NewGuid(), 5));
    }

    [Fact]
    public void Create_RejectsScoreAboveMax()
    {
        var rubric = NewRubricWithOneCriterion(out var cid);
        var factory = new EvaluationFactory(rubric);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => factory.Create(Guid.NewGuid(), cid, 11));
    }

    [Fact]
    public void Create_RejectsNegativeScore()
    {
        var rubric = NewRubricWithOneCriterion(out var cid);
        var factory = new EvaluationFactory(rubric);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => factory.Create(Guid.NewGuid(), cid, -1));
    }

    [Fact]
    public void Create_BuildsEvaluationBoundToCriterionMax()
    {
        var rubric = NewRubricWithOneCriterion(out var cid);
        var factory = new EvaluationFactory(rubric);
        var studentId = Guid.NewGuid();
        var eval = factory.Create(studentId, cid, 7, "хорошо");
        Assert.Equal(studentId, eval.StudentId);
        Assert.Equal(cid, eval.CriterionId);
        Assert.Equal(7, eval.Score.Value);
        Assert.Equal(10, eval.Score.Max.Value);
        Assert.Equal("хорошо", eval.Comment);
    }

    [Fact]
    public void Create_RejectsNullRubric()
    {
        Assert.Throws<ArgumentNullException>(() => new EvaluationFactory(null!));
    }
}