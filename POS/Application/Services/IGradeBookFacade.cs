using POS.Domain.Models;

namespace POS.Application.Services;

public interface IGradeBookFacade
{
    string StrategyName { get; }
    string ExportFormat { get; }

    Student AddStudent(string name, string group);
    IReadOnlyList<Student> ListStudents();

    Criterion AddCriterion(string name, int max, double weight);
    IReadOnlyList<Criterion> ListCriteria();

    Evaluation AssignEvaluation(Guid studentId, Guid criterionId, double score, string? comment);
    IReadOnlyList<Evaluation> ListEvaluations(Guid studentId);

    bool CancelEvaluation(Guid evaluationId);

    FinalScore CalculateFinal(Guid studentId);

    void Export(string filePath);
}