namespace POS.Domain.Models;

public sealed class Criterion
{
    public Guid Id { get; }
    public string Name { get; }
    public MaxScore Max { get; }
    public Weight Weight { get; }

    public Criterion(Guid id, string name, MaxScore max, Weight weight)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Criterion id cannot be empty", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Criterion name cannot be empty", nameof(name));
        ArgumentNullException.ThrowIfNull(max);
        ArgumentNullException.ThrowIfNull(weight);

        Id = id;
        Name = name.Trim();
        Max = max;
        Weight = weight;
    }

    public override string ToString() => $"{Name} (max {Max}, weight {Weight})";
}