using POS.Application.Dtos;

namespace POS.Application.Ports;

// Порт для экспорта итоговых результатов
public interface IExportPort
{
    string Format { get; }
    void Export(IReadOnlyList<FinalScoreDto> rows, string filePath);
}