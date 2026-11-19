namespace POS.Domain.Models;

public sealed record Score
{
    public double Value { get; }
    public MaxScore Max { get; }

    public Score(double value, MaxScore max)
    {
        ArgumentNullException.ThrowIfNull(max);
        if (double.IsNaN(value) || value < 0)
            throw new ArgumentOutOfRangeException(nameof(value), "Score cannot be negative or NaN");
        if (value > max.Value)
            throw new ArgumentOutOfRangeException(nameof(value), $"Score {value} exceeds maximum {max.Value}");
        Value = value;
        Max = max;
    }

    // Normalized to 0..1, convenient for final-score strategies.
    public double Normalized => Max.Value == 0 ? 0 : Value / Max.Value;

    public override string ToString() => $"{Value}/{Max.Value}";
}