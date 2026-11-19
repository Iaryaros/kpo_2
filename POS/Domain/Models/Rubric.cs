using System.Collections.ObjectModel;

namespace POS.Domain.Models;

public sealed class Rubric
{
    private readonly Dictionary<Guid, Criterion> _criteria = new();
    private readonly HashSet<string> _names = new(StringComparer.OrdinalIgnoreCase);

    public string CourseName { get; }
    public IReadOnlyCollection<Criterion> Criteria => new ReadOnlyCollection<Criterion>(_criteria.Values.ToList());

    public Rubric(string courseName)
    {
        if (string.IsNullOrWhiteSpace(courseName))
            throw new ArgumentException("Course name cannot be empty", nameof(courseName));
        CourseName = courseName.Trim();
    }

    public Criterion AddCriterion(string name, MaxScore max, Weight weight)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Criterion name cannot be empty", nameof(name));

        var key = name.Trim();
        if (!_names.Add(key))
            throw new InvalidOperationException($"Criterion '{key}' already exists in the rubric");

        var criterion = new Criterion(Guid.NewGuid(), key, max, weight);
        _criteria[criterion.Id] = criterion;
        return criterion;
    }

    public bool TryGetCriterion(Guid id, out Criterion? criterion)
        => _criteria.TryGetValue(id, out criterion);

    public Criterion GetCriterion(Guid id)
        => _criteria.TryGetValue(id, out var c)
            ? c
            : throw new KeyNotFoundException($"Criterion {id} not found in the rubric");

    public bool Contains(Guid id) => _criteria.ContainsKey(id);
}