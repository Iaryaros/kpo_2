using Xunit;

using POS.Domain.Models;

namespace POS.Tests;

public class ValueObjectsTests
{
    [Fact]
    public void MaxScore_RejectsZeroAndNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MaxScore(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new MaxScore(-5));
    }

    [Fact]
    public void MaxScore_AcceptsPositive()
    {
        Assert.Equal(10, new MaxScore(10).Value);
    }

    [Fact]
    public void Weight_RejectsZeroAndNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Weight(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new Weight(-0.5));
    }

    [Fact]
    public void Weight_AcceptsPositive()
    {
        Assert.Equal(1.5, new Weight(1.5).Value);
    }

    [Fact]
    public void Score_RejectsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Score(-1, new MaxScore(10)));
    }

    [Fact]
    public void Score_RejectsAboveMax()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Score(11, new MaxScore(10)));
    }

    [Fact]
    public void Score_AcceptsBoundaries()
    {
        var max = new MaxScore(10);
        Assert.Equal(0, new Score(0, max).Value);
        Assert.Equal(10, new Score(10, max).Value);
    }

    [Fact]
    public void Score_NormalizedIsZeroToOne()
    {
        Assert.Equal(0.5, new Score(5, new MaxScore(10)).Normalized);
    }

    [Fact]
    public void FullName_RejectsEmptyAndWhitespace()
    {
        Assert.Throws<ArgumentException>(() => new FullName(""));
        Assert.Throws<ArgumentException>(() => new FullName("   "));
    }

    [Fact]
    public void FullName_TrimsValue()
    {
        Assert.Equal("Иван Петров", new FullName("  Иван Петров  ").Value);
    }

    [Fact]
    public void FinalScore_RejectsNegative()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FinalScore(-0.1));
    }
}