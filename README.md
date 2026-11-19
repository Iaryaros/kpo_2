ПОС — ДЗ №2 по КПО

Консольное приложение для оценки студентов: студенты, критерии с весами, оценки, итоговый балл, выгрузка в CSV и JSON.

Запуск
dotnet build POS.slnx
dotnet run --project POS/POS.csproj
dotnet test POS.slnx

Что умеет
В меню 10 пунктов: добавить студента, добавить критерий, выставить оценку, оценки студента, отменить оценку, посчитать итог, экспорт итогов, список студентов, список критериев, демо-сценарий. Демо (пункт 10) заводит двух студентов и два критерия (Лабы и Экзамен), выставляет оценки, считает итог и сохраняет pos_export_demo.csv.

Как устроено

Score — Value Object, в конструкторе проверяет 0 ≤ value ≤ max. Поэтому 12 из 10 не создать.
EvaluationFactory — единственное место создания оценки: проверяет критерий в рубрикаторе и балл в диапазоне.
Итог не хранится — пересчитывается каждый раз из текущих оценок через стратегию.

Доменная модель (DDD)

Entities — Student, Criterion, Evaluation, Rubric (идентичность по Guid).
Value Objects — FullName, MaxScore, Weight, Score, FinalScore (инвариант в конструкторе).
Aggregate root — Rubric: хранит критерии, следит за уникальностью названий. Создать критерий минуя рубрикатор нельзя.
Repositories — IStudentRepository, IRubricRepository, IEvaluationRepository (порты в Domain, реализация в Infrastructure).
Domain Service — IFinalScoreStrategy: итог считается из оценок и рубрикатора, не принадлежит одной сущности.

Инварианты

MaxScore и Weight — только положительные.
Score — от 0 до максимума критерия.
FullName — непустое, обрезается.
Rubric — уникальные названия критериев (без учёта регистра).
EvaluationFactory — критерий должен быть в рубрикаторе.
Стратегия бросает InvalidOperationException, если оценка ссылается на чужой критерий.

Архитектура

Domain — модели, порты репозиториев, доменные сервисы, фабрика.
Application — фасад, порт IExportPort, DTO для экспорта.
Infrastructure — in-memory репозитории, CSV/JSON экспорт (internal sealed).
Presentation — консольный UI.
Program.cs — composition root, собирает граф через DI.
Зависимости внутрь: Presentation → Application → Domain. Infrastructure реализует порты, Program всё склеивает. Домен ничего не знает о файлах, консоли и формате выгрузки.

GoF-паттерны
Factory — EvaluationFactory: одна точка создания оценки со всеми инвариантами.
Facade — GradeBookFacade: единая точка входа для консоли, прячет репозитории, фабрику, стратегию и экспортёр.
Strategy — IFinalScoreStrategy с двумя реализациями: WeightedSumStrategy (взвешенное среднее нормализованных баллов) и RawSumStrategy (сумма сырых баллов). Смена стратегии — одна строка в Program.cs, итог пересчитывается по-другому.

DI и тесты
DI — Microsoft.Extensions.DependencyInjection. Сменить CsvExportPort на JsonExportPort — одна строка.
Тесты — xUnit, 38 штук: инварианты VO, сущностей и агрегата, фабрика, обе стратегии, end-to-end через фасад с экспортом в CSV и JSON.

1.Самый явный паттерн — Strategy. Переключается одной строкой в Program.cs, фасад и UI не меняются. Тест Strategies_AreInterchangeable_ProduceDifferentButValidResults подтверждает.
2.Осознанно ограничен — Facade. GradeBookFacade совпадает с Application Service: точка входа одна, отдельный структурный приём не нужен.
3.При расширении: новый критерий — через Rubric.AddCriterion; пересдача — новое состояние Evaluation или Retake; несколько курсов — RubricRepository по CourseName; апелляция — отдельный Use Case поверх Evaluation.
4.Тесты на инварианты: Score_RejectsAboveMax, Rubric_RejectsDuplicateCriterionNames, Create_RejectsUnknownCriterion, Strategy_RejectsEvaluationForCriterionNotInRubric.
5.Тесты стратегий: WeightedSum_EqualWeights_AveragesNormalizedScores (7.0), WeightedSum_DifferentWeights_WeightedAverage (8.0), RawSum_SumsRawScores (14), Strategies_AreInterchangeable_ProduceDifferentButValidResults.
Роль ИИ
ИИ использовался для комментариев в коде, README, тестов, а также исправлении ошибок.
