using POS.Domain.Models;

namespace POS.Domain.Repositories;

public interface IStudentRepository
{
    IReadOnlyList<Student> List();
    Student? GetById(Guid id);
    void Add(Student student);
}