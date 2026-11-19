using Microsoft.Extensions.DependencyInjection;
using POS.Application.Ports;
using POS.Application.Services;
using POS.Domain.Repositories;
using POS.Domain.Services;
using POS.Infrastructure.Export;
using POS.Infrastructure.Persistence;
using POS.Presentation;

var services = new ServiceCollection();

// Domain ports
services.AddSingleton<IStudentRepository, InMemoryStudentRepository>();
services.AddSingleton<IRubricRepository, InMemoryRubricRepository>();
services.AddSingleton<IEvaluationRepository, InMemoryEvaluationRepository>();

// Domain Service
services.AddSingleton<IFinalScoreStrategy, WeightedSumStrategy>();
//services.AddSingleton<IFinalScoreStrategy, RawSumStrategy>();

// Export adapter
services.AddSingleton<IExportPort, CsvExportPort>();

// Facade
services.AddSingleton<string>("КПО");
services.AddSingleton<IGradeBookFacade, GradeBookFacade>();

// Presentation
services.AddSingleton<IPosConsoleUi, PosConsoleUi>();

var serviceProvider = services.BuildServiceProvider();
var ui = serviceProvider.GetRequiredService<IPosConsoleUi>();
ui.Run();