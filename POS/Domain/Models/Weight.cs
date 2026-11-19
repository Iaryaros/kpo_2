namespace POS.Domain.Models;

public sealed record Weight
{
    public double Value { get; }

    public Weight(double value)
    {
        if (double.IsNaN(value) || value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Weight must be positive");
        Value = value;
    }

    public override string ToString() => Value.ToString("0.##");
}