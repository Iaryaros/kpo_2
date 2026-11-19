namespace POS.Domain.Models;

public sealed record FullName
{
    public string Value { get; }

    public FullName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Full name cannot be empty", nameof(value));
        Value = value.Trim();
    }

    public override string ToString() => Value;
}