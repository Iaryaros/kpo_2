namespace POS.Domain.Models;

public sealed record FinalScore
{
    public double Value { get; }

    public FinalScore(double value)
    {
        if (double.IsNaN(value) || value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Final score cannot be negative or NaN");
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}