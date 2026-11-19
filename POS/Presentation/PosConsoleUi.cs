using POS.Application.Services;
using POS.Domain.Models;

namespace POS.Presentation;

public sealed class PosConsoleUi(IGradeBookFacade facade) : IPosConsoleUi
{
    public void Run()
    {
        Console.WriteLine("=== ПОС — Преподавательская Оценка Студентов ===");
        Console.WriteLine($"Стратегия итога: {facade.StrategyName}");
        Console.WriteLine($"Формат выгрузки: {facade.ExportFormat}");
        Console.WriteLine();

        var running = true;
        while (running)
        {
            PrintMenu();
            var choice = Console.ReadLine()?.Trim();
            Console.WriteLine();
            try
            {
                running = Handle(choice);
            }
            catch (Exception ex) when (ex is FormatException or OverflowException)
            {
                Console.WriteLine("Некорректный ввод числа.");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Отклонено доменом: {ex.Message}");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"Не найдено: {ex.Message}");
            }
            Console.WriteLine();
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("--- Меню ---");
        Console.WriteLine("1. Добавить студента");
        Console.WriteLine("2. Добавить критерий");
        Console.WriteLine("3. Выставить оценку");
        Console.WriteLine("4. Оценки студента");
        Console.WriteLine("5. Отменить оценку");
        Console.WriteLine("6. Посчитать итог");
        Console.WriteLine("7. Экспорт итогов в файл");
        Console.WriteLine("8. Список студентов");
        Console.WriteLine("9. Список критериев");
        Console.WriteLine("10. Демо-сценарий (сквозной прогон)");
        Console.WriteLine("0. Выход");
        Console.Write("Выбор: ");
    }

    private bool Handle(string? choice)
    {
        switch (choice)
        {
            case "0":
                Console.WriteLine("Пока.");
                return false;

            case "1":
            {
                Console.Write("ФИО: ");
                var name = Console.ReadLine() ?? "";
                Console.Write("Группа: ");
                var group = Console.ReadLine() ?? "";
                var s = facade.AddStudent(name, group);
                Console.WriteLine($"Добавлен: {s.Name} ({s.Group}) id={s.Id}");
                return true;
            }

            case "2":
            {
                Console.Write("Название критерия: ");
                var name = Console.ReadLine() ?? "";
                Console.Write("Максимум баллов (>0): ");
                var max = int.Parse(Console.ReadLine() ?? "0");
                Console.Write("Вес (>0): ");
                var w = double.Parse(Console.ReadLine() ?? "0");
                var c = facade.AddCriterion(name, max, w);
                Console.WriteLine($"Добавлен: {c} id={c.Id}");
                return true;
            }

            case "3":
            {
                Console.Write("Id студента: ");
                var sid = Guid.Parse(Console.ReadLine() ?? "");
                Console.Write("Id критерия: ");
                var cid = Guid.Parse(Console.ReadLine() ?? "");
                Console.Write("Балл: ");
                var score = double.Parse(Console.ReadLine() ?? "0");
                Console.Write("Комментарий (можно пусто): ");
                var comment = Console.ReadLine();
                var e = facade.AssignEvaluation(sid, cid, score, string.IsNullOrWhiteSpace(comment) ? null : comment);
                Console.WriteLine($"Выставлена: {e}");
                return true;
            }

            case "4":
            {
                Console.Write("Id студента: ");
                var sid = Guid.Parse(Console.ReadLine() ?? "");
                var evals = facade.ListEvaluations(sid);
                if (evals.Count == 0)
                    Console.WriteLine("Оценок нет.");
                else
                    foreach (var e in evals) Console.WriteLine($"  {e}");
                return true;
            }

            case "5":
            {
                Console.Write("Id оценки: ");
                var eid = Guid.Parse(Console.ReadLine() ?? "");
                var ok = facade.CancelEvaluation(eid);
                Console.WriteLine(ok ? "Оценка отменена." : "Оценка не найдена.");
                return true;
            }

            case "6":
            {
                Console.Write("Id студента: ");
                var sid = Guid.Parse(Console.ReadLine() ?? "");
                var final = facade.CalculateFinal(sid);
                Console.WriteLine($"Итог: {final.Value:0.##}");
                foreach (var e in facade.ListEvaluations(sid)) Console.WriteLine($"  {e}");
                return true;
            }

            case "7":
            {
                Console.Write("Путь к файлу: ");
                var path = Console.ReadLine() ?? "";
                facade.Export(path);
                Console.WriteLine($"Выгружено в {path}");
                return true;
            }

            case "8":
            {
                var list = facade.ListStudents();
                if (list.Count == 0)
                    Console.WriteLine("Студентов нет.");
                else
                    foreach (var s in list) Console.WriteLine($"  {s.Name,-30} {s.Group,-10} id={s.Id}");
                return true;
            }

            case "9":
            {
                var list = facade.ListCriteria();
                if (list.Count == 0)
                    Console.WriteLine("Критериев нет.");
                else
                    foreach (var c in list) Console.WriteLine($"  {c.Name,-30} макс={c.Max.Value,-4} вес={c.Weight.Value} id={c.Id}");
                return true;
            }

            case "10":
                RunDemo();
                return true;

            default:
                Console.WriteLine("Неизвестный пункт меню.");
                return true;
        }
    }

    // End-to-end demo: students -> criteria -> evaluations -> final -> export.
    private void RunDemo()
    {
        Console.WriteLine(">>> Демо: заводим студентов и критерии, выставляем оценки, считаем итог, выгружаем.");

        var ivan = facade.AddStudent("Иван Петров", "КПО-1");
        var maria = facade.AddStudent("Мария Сидорова", "КПО-1");
        Console.WriteLine($"Студенты: {ivan.Name} ({ivan.Id}), {maria.Name} ({maria.Id})");

        var labs = facade.AddCriterion("Лабы", 10, 1);
        var exam = facade.AddCriterion("Экзамен", 10, 2);
        Console.WriteLine($"Критерии: {labs.Name} макс={labs.Max.Value} вес={labs.Weight.Value}; {exam.Name} макс={exam.Max.Value} вес={exam.Weight.Value}");

        facade.AssignEvaluation(ivan.Id, labs.Id, 8, "ровно");
        facade.AssignEvaluation(ivan.Id, exam.Id, 6, null);
        facade.AssignEvaluation(maria.Id, labs.Id, 10, null);
        facade.AssignEvaluation(maria.Id, exam.Id, 9, "отлично");
        Console.WriteLine("Оценки выставлены.");

        var ivanFinal = facade.CalculateFinal(ivan.Id);
        var mariaFinal = facade.CalculateFinal(maria.Id);
        Console.WriteLine($"Итог {ivan.Name}: {ivanFinal.Value:0.##} (по {facade.StrategyName})");
        foreach (var e in facade.ListEvaluations(ivan.Id)) Console.WriteLine($"    {e}");
        Console.WriteLine($"Итог {maria.Name}: {mariaFinal.Value:0.##}");
        foreach (var e in facade.ListEvaluations(maria.Id)) Console.WriteLine($"    {e}");

        var path = Path.Combine(Environment.CurrentDirectory, "pos_export_demo.csv");
        facade.Export(path);
        Console.WriteLine($"Выгрузка в файл: {Path.GetFullPath(path)}");
    }
}