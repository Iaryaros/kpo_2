using System.Collections.Concurrent;

using POS.Domain.Models;
using POS.Domain.Repositories;

namespace POS.Infrastructure.Persistence;

internal sealed class InMemoryStudentRepository : IStudentRepository
{
    private readonly ConcurrentDictionary<Guid, Student> _store = new();

    public IReadOnlyList<Student> List() => _store.Values.ToList();

    public Student? GetById(Guid id) => _store.TryGetValue(id, out var s) ? s : null;

    public void Add(Student student)
    {
        if (!_store.TryAdd(student.Id, student))
            throw new InvalidOperationException($"Student {student.Id} already exists");
    }
}