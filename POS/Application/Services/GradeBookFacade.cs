using POS.Application.Dtos;
using POS.Application.Ports;
using POS.Domain.Factories;
using POS.Domain.Models;
using POS.Domain.Repositories;
using POS.Domain.Services;

namespace POS.Application.Services;

public sealed class GradeBookFacade(
    IStudentRepository students,
    IRubricRepository rubrics,
    IEvaluationRepository evaluations,
    IFinalScoreStrategy strategy,
    IExportPort export,
    string courseName) : IGradeBookFacade
{
    public string StrategyName => strategy.Name;
    public string ExportFormat => export.Format;

    private Rubric GetRubric()
    {
        var rubric = rubrics.Get();
        if (rubric is null)
        {
            rubric = new Rubric(courseName);
            rubrics.Save(rubric);
        }
        return rubric;
    }

    public Student AddStudent(string name, string group)
    {
        var student = Student.Create(new FullName(name), group);
        students.Add(student);
        return student;
    }

    public IReadOnlyList<Student> ListStudents() => students.List();

    public Criterion AddCriterion(string name, int max, double weight)
    {
        var rubric = GetRubric();
        var criterion = rubric.AddCriterion(name, new MaxScore(max), new Weight(weight));
        rubrics.Save(rubric);
        return criterion;
    }

    public IReadOnlyList<Criterion> ListCriteria()
        => GetRubric().Criteria.OrderBy(c => c.Name).ToList();

    public Evaluation AssignEvaluation(Guid studentId, Guid criterionId, double score, string? comment)
{
    if (students.GetById(studentId) is null)
        throw new KeyNotFoundException($"Student {studentId} not found");

    var rubric = GetRubric();
    var eval = new EvaluationFactory(rubric).Create(studentId, criterionId, score, comment);
    var existing = evaluations.ListByStudent(studentId)
        .FirstOrDefault(e => e.CriterionId == criterionId);
    if (existing is not null)
        evaluations.Remove(existing.Id);

    evaluations.Add(eval);
    return eval;
}

    public IReadOnlyList<Evaluation> ListEvaluations(Guid studentId)
        => evaluations.ListByStudent(studentId).OrderBy(e => e.Date).ToList();

    public bool CancelEvaluation(Guid evaluationId)
    {
        if (evaluations.FindById(evaluationId) is null)
            return false;
        evaluations.Remove(evaluationId);
        return true;
    }

    public FinalScore CalculateFinal(Guid studentId)
    {
        var rubric = GetRubric();
        if (students.GetById(studentId) is null)
            throw new KeyNotFoundException($"Student {studentId} not found");
        var evals = evaluations.ListByStudent(studentId);
        return strategy.Calculate(evals, rubric);
    }

    public void Export(string filePath)
    {
        var rows = students.List().Select(s => BuildFinalScoreRow(s.Id)).ToList();
        export.Export(rows, filePath);
    }

    private FinalScoreDto BuildFinalScoreRow(Guid studentId)
    {
        var rubric = GetRubric();
        var student = students.GetById(studentId)
            ?? throw new KeyNotFoundException($"Student {studentId} not found");
        var evals = evaluations.ListByStudent(studentId);
        var final = strategy.Calculate(evals, rubric);

        var evalDtos = evals
            .Select(e =>
            {
                rubric.TryGetCriterion(e.CriterionId, out var c);
                return new EvaluationDto(
                    e.Id, e.StudentId, e.CriterionId,
                    c?.Name ?? "(criterion removed)", e.Score.Value, e.Score.Max.Value,
                    e.Date, e.Comment);
            })
            .OrderBy(e => e.Date)
            .ToList();

        return new FinalScoreDto(
            student.Id, student.Name.Value, student.Group,
            strategy.Name, final.Value, evalDtos);
    }
}