namespace POS.Domain.Models;

public sealed record MaxScore
{
    public int Value { get; }

    public MaxScore(int value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Max score must be positive");
        Value = value;
    }

    public override string ToString() => Value.ToString();
}