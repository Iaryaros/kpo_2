using POS.Domain.Models;
using POS.Domain.Repositories;

namespace POS.Infrastructure.Persistence;

internal sealed class InMemoryRubricRepository : IRubricRepository
{
    private Rubric? _rubric;
    private readonly object _gate = new();

    public Rubric? Get()
    {
        lock (_gate) return _rubric;
    }

    public void Save(Rubric rubric)
    {
        ArgumentNullException.ThrowIfNull(rubric);
        lock (_gate) _rubric = rubric;
    }
}