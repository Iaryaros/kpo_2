using POS.Application.Ports;
using POS.Application.Services;
using POS.Domain.Services;
using POS.Infrastructure.Export;
using POS.Infrastructure.Persistence;
using Xunit;

namespace POS.Tests;

public class GradeBookFacadeTests
{
    private static IGradeBookFacade NewFacade(IExportPort? export = null)
        => new GradeBookFacade(
            new InMemoryStudentRepository(),
            new InMemoryRubricRepository(),
            new InMemoryEvaluationRepository(),
            new WeightedSumStrategy(),
            export ?? new CsvExportPort(),
            "КПО");

    [Fact]
    public void FullScenario_StudentsCriteriaEvaluationsFinalExport()
    {
        var facade = NewFacade();
        var ivan = facade.AddStudent("Иван Петров", "КПО-1");
        var maria = facade.AddStudent("Мария Сидорова", "КПО-1");
        var labs = facade.AddCriterion("Лабы", 10, 1);
        var exam = facade.AddCriterion("Экзамен", 10, 2);
        facade.AssignEvaluation(ivan.Id, labs.Id, 8, "ровно");
        facade.AssignEvaluation(ivan.Id, exam.Id, 6, null);
        facade.AssignEvaluation(maria.Id, labs.Id, 10, null);
        facade.AssignEvaluation(maria.Id, exam.Id, 9, "отлично");
        var ivanFinal = facade.CalculateFinal(ivan.Id);
        Assert.Equal(6.666666666666667, ivanFinal.Value, 4);
        Assert.Equal(2, facade.ListEvaluations(ivan.Id).Count);
        var mariaFinal = facade.CalculateFinal(maria.Id);
        Assert.Equal(9.333333333333334, mariaFinal.Value, 4);
        var path = Path.Combine(Path.GetTempPath(), $"pos_export_{Guid.NewGuid():N}.csv");
        facade.Export(path);
        Assert.True(File.Exists(path));
        var content = File.ReadAllText(path);
        Assert.Contains("Иван Петров", content);
        Assert.Contains("Мария Сидорова", content);
        File.Delete(path);
    }

    [Fact]
    public void AssignEvaluation_RejectsScoreAboveCriterionMax()
    {
        var facade = NewFacade();
        var s = facade.AddStudent("Тест Тестов", "КПО-1");
        var c = facade.AddCriterion("Лабы", 5, 1);
        Assert.Throws<ArgumentOutOfRangeException>(
            () => facade.AssignEvaluation(s.Id, c.Id, 6, null));
    }

    [Fact]
    public void AssignEvaluation_RejectsUnknownCriterion()
    {
        var facade = NewFacade();
        var s = facade.AddStudent("Тест Тестов", "КПО-1");
        Assert.Throws<InvalidOperationException>(
            () => facade.AssignEvaluation(s.Id, Guid.NewGuid(), 5, null));
    }

    [Fact]
    public void AddCriterion_RejectsDuplicateName()
    {
        var facade = NewFacade();
        facade.AddCriterion("Лабы", 10, 1);
        Assert.Throws<InvalidOperationException>(
            () => facade.AddCriterion("Лабы", 10, 1));
    }

    [Fact]
    public void CancelEvaluation_RemovesIt()
    {
        var facade = NewFacade();
        var s = facade.AddStudent("Тест Тестов", "КПО-1");
        var c = facade.AddCriterion("Лабы", 10, 1);
        var e = facade.AssignEvaluation(s.Id, c.Id, 7, null);
        Assert.True(facade.CancelEvaluation(e.Id));
        Assert.Empty(facade.ListEvaluations(s.Id));
    }

    [Fact]
    public void CancelEvaluation_UnknownId_ReturnsFalse()
    {
        Assert.False(NewFacade().CancelEvaluation(Guid.NewGuid()));
    }

    [Fact]
    public void JsonExport_ProducesValidJson()
    {
        var facade = NewFacade(new JsonExportPort());
        var s = facade.AddStudent("Иван Петров", "КПО-1");
        var c = facade.AddCriterion("Лабы", 10, 1);
        facade.AssignEvaluation(s.Id, c.Id, 8, null);
        var path = Path.Combine(Path.GetTempPath(), $"pos_export_{Guid.NewGuid():N}.json");
        facade.Export(path);
        var content = File.ReadAllText(path);
        Assert.Contains("Иван Петров", content);
        Assert.StartsWith("[", content.TrimStart());
        Assert.EndsWith("]", content.TrimEnd());

        File.Delete(path);
    }
}