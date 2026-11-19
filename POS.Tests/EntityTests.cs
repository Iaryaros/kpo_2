using Xunit;

using POS.Domain.Models;

namespace POS.Tests;

// Entity and aggregate invariants.
public class EntityTests
{
    [Fact]
    public void Student_RejectsEmptyNameAndGroup()
    {
        Assert.Throws<ArgumentException>(() => Student.Create(new FullName("Иван"), ""));
        Assert.Throws<ArgumentNullException>(() => Student.Create(null!, "КПО-1"));
    }

    [Fact]
    public void Student_IdentityByGuid()
    {
        var s = Student.Create(new FullName("Иван"), "КПО-1");
        Assert.NotEqual(Guid.Empty, s.Id);
        Assert.Equal("КПО-1", s.Group);
    }

    [Fact]
    public void Criterion_RejectsEmptyName()
    {
        Assert.Throws<ArgumentException>(
            () => new Criterion(Guid.NewGuid(), "", new MaxScore(10), new Weight(1)));
    }

    [Fact]
    public void Rubric_RejectsDuplicateCriterionNames()
    {
        var rubric = new Rubric("КПО");
        rubric.AddCriterion("Лабы", new MaxScore(10), new Weight(1));
        Assert.Throws<InvalidOperationException>(
            () => rubric.AddCriterion("лабы", new MaxScore(5), new Weight(1)));
    }

    [Fact]
    public void Rubric_GetCriterion_ThrowsWhenMissing()
    {
        var rubric = new Rubric("КПО");
        Assert.Throws<KeyNotFoundException>(() => rubric.GetCriterion(Guid.NewGuid()));
    }

    [Fact]
    public void Rubric_TryGetCriterion_ReturnsFalseWhenMissing()
    {
        var rubric = new Rubric("КПО");
        Assert.False(rubric.TryGetCriterion(Guid.NewGuid(), out var c));
        Assert.Null(c);
    }

    [Fact]
    public void Rubric_RejectsEmptyCourseName()
    {
        Assert.Throws<ArgumentException>(() => new Rubric("   "));
    }

    [Fact]
    public void Evaluation_RejectsEmptyStudentId()
    {
        Assert.Throws<ArgumentException>(() =>
            new Evaluation(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(),
                new Score(5, new MaxScore(10)), DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    [Fact]
    public void Evaluation_UpdateComment_NullSafe()
    {
        var eval = new Evaluation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            new Score(5, new MaxScore(10)), DateOnly.FromDateTime(DateTime.UtcNow), "старый");
        eval.UpdateComment("   ");
        Assert.Null(eval.Comment);
    }
}