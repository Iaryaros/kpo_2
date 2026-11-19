using System.Collections.Concurrent;

using POS.Domain.Models;
using POS.Domain.Repositories;

namespace POS.Infrastructure.Persistence;

internal sealed class InMemoryEvaluationRepository : IEvaluationRepository
{
    private readonly ConcurrentDictionary<Guid, Evaluation> _store = new();

    public IReadOnlyList<Evaluation> ListByStudent(Guid studentId)
        => _store.Values.Where(e => e.StudentId == studentId).ToList();

    public IReadOnlyList<Evaluation> ListAll() => _store.Values.ToList();

    public void Add(Evaluation evaluation)
    {
        if (!_store.TryAdd(evaluation.Id, evaluation))
            throw new InvalidOperationException($"Evaluation {evaluation.Id} already exists");
    }

    public void Remove(Guid id) => _store.TryRemove(id, out _);

    public Evaluation? FindById(Guid id) => _store.TryGetValue(id, out var e) ? e : null;
}