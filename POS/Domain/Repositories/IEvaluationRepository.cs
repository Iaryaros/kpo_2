using POS.Domain.Models;

namespace POS.Domain.Repositories;

public interface IEvaluationRepository
{
    IReadOnlyList<Evaluation> ListByStudent(Guid studentId);
    IReadOnlyList<Evaluation> ListAll();
    void Add(Evaluation evaluation);
    void Remove(Guid id);
    Evaluation? FindById(Guid id);
}