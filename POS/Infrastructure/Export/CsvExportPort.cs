using System.Globalization;
using System.Text;

using POS.Application.Dtos;
using POS.Application.Ports;

namespace POS.Infrastructure.Export;

// CSV adapter for IExportPort
internal sealed class CsvExportPort : IExportPort
{
    public string Format => "CSV";
    public void Export(IReadOnlyList<FinalScoreDto> rows, string filePath)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be empty", nameof(filePath));

        var sb = new StringBuilder();
        sb.AppendLine("StudentId;StudentName;Group;Strategy;FinalScore;Evaluations");
        foreach (var r in rows)
        {
            var evals = r.Evaluations.Count == 0
                ? "-"
                : string.Join(" | ", r.Evaluations.Select(e => $"{e.CriterionName}={e.Score}/{e.Max}"));
            sb.AppendLine(string.Join(';',
                r.StudentId.ToString(),
                Quote(r.StudentName),
                Quote(r.Group),
                Quote(r.StrategyName),
                r.Value.ToString("0.##", CultureInfo.InvariantCulture),
                Quote(evals)));
        }
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
    }

    private static string Quote(string s)
    {
        if (s.IndexOfAny(new[] { ';', '"', '\n', '\r' }) < 0) return s;
        return "\"" + s.Replace("\"", "\"\"") + "\"";
    }
}