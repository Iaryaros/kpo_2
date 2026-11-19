using POS.Domain.Models;

namespace POS.Domain.Repositories;

public interface IRubricRepository
{
    Rubric? Get();
    void Save(Rubric rubric);
}